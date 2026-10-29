using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Domain.Entities;
using Taxi.Domain.Enums;
using Taxi.Infrastructure.Repos.RideMatchingRepos;

namespace Taxi.Infrastructure.Repos.RideMatchingRepository
{
    public class DriverMatchingRepos : IDriverMatchingRepos
    {
        private readonly AppDbContext _dbContext;
        public DriverMatchingRepos(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> TryAcceptRide(Guid userId, Guid rideId)
        {
            await using var transaction =
                await _dbContext.Database.BeginTransactionAsync();

            var ride = await _dbContext.Rides
                .AsNoTracking()
                .Where(x => x.Id == rideId)
                .Select(x => new
                {
                    x.Id,
                    x.Status,
                    x.RideClass
                })
                .FirstOrDefaultAsync();

            if (ride == null || ride.Status != RideStatus.Searching)
                return false;

            var driver = await _dbContext.Drivers
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(x => new
                {
                    x.Id,
                    x.IsOnline,
                    x.IsAvailable,
                    x.ActiveVehicleId,
                    VehicleClass = x.ActiveVehicle == null
                        ? (VehicleClass?)null
                        : x.ActiveVehicle.Class
                })
                .FirstOrDefaultAsync();

            if (driver == null ||
                !driver.IsOnline ||
                !driver.IsAvailable ||
                driver.ActiveVehicleId == null ||
                driver.VehicleClass == null ||
                driver.VehicleClass != ride.RideClass)
            {
                return false;
            }

            // Атомарно резервуємо водія.
            int driverUpdated = await _dbContext.Drivers
                .Where(x =>
                    x.Id == driver.Id &&
                    x.IsOnline &&
                    x.IsAvailable &&
                    x.ActiveVehicleId == driver.ActiveVehicleId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.IsAvailable, false));

            if (driverUpdated != 1)
                return false;

            // Атомарно закріплюємо замовлення за цим водієм.
            int rideUpdated = await _dbContext.Rides
                .Where(x =>
                    x.Id == rideId &&
                    x.Status == RideStatus.Searching &&
                    x.RideClass == driver.VehicleClass)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.DriverId, (Guid?)driver.Id)
                    .SetProperty(x => x.VehicleId, driver.ActiveVehicleId)
                    .SetProperty(x => x.Status, RideStatus.Accepted)
                    .SetProperty(x => x.AcceptedAtUtc, DateTime.UtcNow));

            if (rideUpdated != 1)
                return false;

            await transaction.CommitAsync();
            return true;
        }
        public async Task<List<Guid>> GetDueRideIds(
    DateTime nowUtc,
    int batchSize = 100)
        {
            return await _dbContext.Rides
                .AsNoTracking()
                .Where(x =>
                    x.Status == RideStatus.Searching &&
                    x.NextSearchAtUtc != null &&
                    x.NextSearchAtUtc <= nowUtc)
                .OrderBy(x => x.NextSearchAtUtc)
                .Select(x => x.Id)
                .Take(batchSize)
                .ToListAsync();
        }

        public async Task<Ride?> GetRideSnapshot(Guid rideId)
        {
            return await _dbContext.Rides
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == rideId);
        }

        public async Task<bool> TryAdvanceSearchStage(
            Guid rideId,
            int expectedRadius,
            int newRadius,
            DateTime nextSearchAtUtc)
        {
            int updated = await _dbContext.Rides
                .Where(x =>
                    x.Id == rideId &&
                    x.Status == RideStatus.Searching &&
                    x.SearchRadiusMeters == expectedRadius)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.SearchRadiusMeters, newRadius)
                    .SetProperty(x => x.NextSearchAtUtc, nextSearchAtUtc));

            return updated == 1;
        }

        public async Task<bool> TryMarkNoDriver(
            Guid rideId,
            DateTime nowUtc)
        {
            int updated = await _dbContext.Rides
                .Where(x =>
                    x.Id == rideId &&
                    x.Status == RideStatus.Searching &&
                    x.SearchRadiusMeters == 5000 &&
                    x.NextSearchAtUtc != null &&
                    x.NextSearchAtUtc <= nowUtc)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Status, RideStatus.NoDriver)
                    .SetProperty(x => x.NextSearchAtUtc, (DateTime?)null));

            return updated == 1;
        }
    }
}

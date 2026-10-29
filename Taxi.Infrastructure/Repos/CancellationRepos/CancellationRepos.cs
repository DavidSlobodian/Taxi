using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Domain.Enums;

namespace Taxi.Infrastructure.Repos.CancellationRepos
{
    public class CancellationRepos : ICancellationRepos
    {
        private readonly AppDbContext _dbContext;
        public CancellationRepos(AppDbContext dbContext) 
        {
            _dbContext = dbContext;
        }
        public async Task<bool> TryCancelRide(Guid rideId, CancellationReasons reason, Guid? driverId = null)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            var ride = await _dbContext.Rides
                .AsNoTracking()
                .Where(x => x.Id == rideId)
                .Select(x => new
                {
                    x.Id,
                    x.Status
                }).FirstOrDefaultAsync();

            if (ride == null || ride.Status == Domain.Enums.RideStatus.Completed || ride.Status == Domain.Enums.RideStatus.Cancelled || ride.Status == Domain.Enums.RideStatus.NoDriver)
            {
                return false;
            }

            if(driverId != null)
            {
                var driver = await _dbContext.Drivers
                .AsNoTracking()
                .Where(x => x.Id == driverId)
                .Select(x => new
                {
                    x.Id,
                    x.IsOnline,
                    x.IsAvailable
                }).FirstOrDefaultAsync();

                if (driver == null || !driver.IsOnline)
                    return false;

                //Атомарно звільняєм водія
                int driverUpdated = await _dbContext.Drivers
                    .Where(x => x.Id == driver.Id && x.IsOnline == driver.IsOnline && x.IsAvailable == driver.IsAvailable)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.IsAvailable, true));

                if (driverUpdated != 1)
                    return false;
            }
            

            //Атомарно скасовуєм поїздку
            int rideUpdated = await _dbContext.Rides
                .Where(x => x.Id == ride.Id && x.Status == ride.Status)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Status, Domain.Enums.RideStatus.Cancelled)
                    .SetProperty(x => x.CancellationReason, reason)
                    .SetProperty(x => x.FinishedAtUtc, DateTime.UtcNow));

            if (rideUpdated != 1)
                return false;

            await _dbContext.Database.CommitTransactionAsync();
            return true;
        }
    }
}

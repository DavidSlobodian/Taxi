using Microsoft.AspNetCore.Internal;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs.OutputDTO;
using Taxi.App.Services.LocationService.DriverLocationService;
using Taxi.App.Services.LocationService.RideLocationService;
using Taxi.Domain.Entities;
using Taxi.Domain.Enums;
using Taxi.Infrastructure;
using Taxi.Infrastructure.Repos;
using Taxi.Infrastructure.Repos.RideMatchingRepos;

namespace Taxi.App.Services.DriverMatchingService
{
    public class DriverMatchingService : IDriverMatchingService
    {
        private readonly IDriverMatchingRepos _matchingRepo;
        private readonly IDriverLocationService _driverLocService;
        private readonly IRideLocationService _rideLocService;
        private readonly IConnectionMultiplexer _redis;
        private readonly IHubContext<DriverHub> _hub;
        private readonly IGenericRepository<Driver> _driverRepos;
        private readonly IGenericRepository<Ride> _rideRepos;
        private readonly IGenericRepository<Vehicle> _vehRepos;
        private readonly AppDbContext _dbContext;

        private readonly int[] _radii = { 2000, 4000, 5000 };

        public DriverMatchingService(
            IDriverMatchingRepos matchingRepo,
            IDriverLocationService driverLocService,
            IConnectionMultiplexer redis,
            IHubContext<DriverHub> hub,
            IGenericRepository<Driver> driverRepos,
            IGenericRepository<Ride> rideRepos,
            IGenericRepository<Vehicle> vehRepos,
            IRideLocationService rideLocService,
            AppDbContext dbContext)
        {
            _matchingRepo = matchingRepo;
            _driverLocService = driverLocService;
            _redis = redis;
            _hub = hub;
            _driverRepos = driverRepos;
            _rideRepos = rideRepos;
            _vehRepos = vehRepos;
            _rideLocService = rideLocService;
            _dbContext = dbContext;
        }

        public async Task<List<NearRidesDTO>> GetAllNearRides(Guid userId, double radius = 5000)
        {
            Driver driver = await _driverRepos.GetByPredicate(
                x => x.UserId == userId);

            if (driver == null || !driver.IsOnline)
                return new List<NearRidesDTO>();

            if (driver.ActiveVehicleId == null)
                return new List<NearRidesDTO>();

            var nearRides = await _rideLocService.GetNearRides(userId, radius);

            var nearRidesIds = nearRides
                .Select(x => x.RideId)
                .ToList();

            var rides = await _rideRepos.GetManyByPredicate(
                x => nearRidesIds.Contains(x.Id) &&
                     x.Status == RideStatus.Searching);

            // Швидкий пошук геоданих за ID замовлення.
            var nearRidesById = nearRides.ToDictionary(x => x.RideId);

            //Console.WriteLine($"Redis rides: {nearRides.Count}");
            //Console.WriteLine($"SQL searching rides: {rides.Count}");
            //Console.WriteLine($"Driver: {driver.Id}");
            //Console.WriteLine($"Active vehicle: {driver.ActiveVehicleId}");

            List<NearRidesDTO> results = new();

            foreach (var ride in rides)
            {
                if (!nearRidesById.TryGetValue(ride.Id, out var nearRide))
                    continue;

                if (nearRide.DistanceToPickUpM > ride.SearchRadiusMeters)
                    continue;

                // Перевіряємо активний автомобіль через його FK.
                var vehicle = await _vehRepos.GetByPredicate(
                    x => x.Id == driver.ActiveVehicleId &&
                         x.DriverId == driver.Id);

                if (vehicle == null ||
                    vehicle.Class != ride.RideClass)
                    continue;

                nearRide.DistanceToPickUpM = double.Parse(nearRide.DistanceToPickUpM.Value.ToString("F0"));
                nearRide.PickupLatitude = ride.PickupLatitude;
                nearRide.PickupLongitude = ride.PickupLongtitude;
                nearRide.DestinationLatitude = ride.DestinationLatitude;
                nearRide.DestinationLongitude = ride.DestinationLongtitude;
                nearRide.RideDistanceKm = ride.Distance;
                nearRide.VehicleClass = ride.RideClass;
                nearRide.PricePerKm = ride.PricePerKm;
                nearRide.TotalPrice = ride.TotalPrice;

                results.Add(nearRide);
            }

            return results;
        }

        public async Task ProcessPendingMatches()
        {
            var now = DateTime.UtcNow;
            var rideIds = await _matchingRepo.GetDueRideIds(now);

            foreach (var rideId in rideIds)
            {
                var ride = await _matchingRepo.GetRideSnapshot(rideId);

                if (ride == null || ride.Status != RideStatus.Searching)
                    continue;

                if (ride.SearchRadiusMeters >= 5000)
                {
                    bool ended = await _matchingRepo.TryMarkNoDriver(
                        ride.Id, now);

                    if (ended)
                    {
                        var db = _redis.GetDatabase();

                        await db.GeoRemoveAsync(
                            "rides:locations", ride.Id.ToString());

                        await db.KeyDeleteAsync(
                            $"rides:search-radius:{ride.Id}");

                        await _hub.Clients
                            .Group("Drivers")
                            .SendAsync("RideUnavailable", ride.Id);
                    }

                    continue;
                }

                int nextRadius = ride.SearchRadiusMeters switch
                {
                    0 => 2000,
                    2000 => 4000,
                    4000 => 5000,
                    _ => 5000
                };

                bool advanced = await _matchingRepo.TryAdvanceSearchStage(
                    ride.Id,
                    ride.SearchRadiusMeters,
                    nextRadius,
                    now.AddSeconds(50));

                if (!advanced)
                    continue;

                // Підтримуємо ключ доступності в Redis для меню водія.
                var redisDb = _redis.GetDatabase();

                await redisDb.StringSetAsync(
                    $"rides:search-radius:{ride.Id}",
                    nextRadius);

                var drivers = await _driverLocService.GetNearDrivers(
                    ride.PickupLongtitude,
                    ride.PickupLatitude,
                    nextRadius);

                string notifiedKey = $"rides:notified:{ride.Id}";

                foreach (var nearby in drivers)
                {
                    var driverId = nearby.DriverId;
                    var driver = await _driverRepos.GetById(driverId);

                    var vehicle = await _vehRepos.GetByPredicate(
                        x => x.Id == driver.ActiveVehicleId &&
                             x.DriverId == driver.Id);

                    if (vehicle == null ||
                        vehicle.Class != ride.RideClass)
                        continue;

                    // Redis Set не дозволяє розсилати одному водієві
                    // ту саму пропозицію на кожному етапі повторно.
                    bool firstNotification = await redisDb.SetAddAsync(
                        notifiedKey,
                        driver.UserId.ToString());

                    if (!firstNotification)
                        continue;

                    await _hub.Clients
                        .User(driver.UserId.ToString())
                        .SendAsync("RideAvailable", new
                        {
                            RideId = ride.Id,
                            PickupLatitude = ride.PickupLatitude,
                            PickupLongitude = ride.PickupLongtitude,
                            DestinationLatitude = ride.DestinationLatitude,
                            DestinationLongitude = ride.DestinationLongtitude,
                            DistanceKm = ride.Distance,
                            PricePerKm = ride.PricePerKm,
                            TotalPrice = ride.TotalPrice
                        });
                }

                await redisDb.KeyExpireAsync(
                    notifiedKey, TimeSpan.FromMinutes(30));
            }
        }

        
    }
}

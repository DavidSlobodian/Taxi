using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using Taxi.App.DTOs;
using Taxi.App.Models;
using Taxi.Domain.Entities;
using Taxi.Domain.Exceptions;
using Taxi.Infrastructure.Repos;

namespace Taxi.App.Services.LocationService.DriverLocationService
{
    public class DriverLocationService : IDriverLocationService
    {
        private readonly IConnectionMultiplexer _multiplexer;
        private readonly IDatabase _db;
        private readonly IGenericRepository<Driver> _driverRepo;
        public DriverLocationService(IConnectionMultiplexer multiplexer, IGenericRepository<Driver> driverRepo)
        {
            _multiplexer = multiplexer;
            _db = _multiplexer.GetDatabase();
            _driverRepo = driverRepo;
        }
        public async Task UpdateLocation(Guid userId, double latitude, double longitude)
        {
            Driver driver = await _driverRepo.GetByPredicate(x => x.UserId == userId);
            if(!driver.IsOnline)
            {
                throw new BadRequestException("The driver is offline");
            }
            await _db.GeoAddAsync("drivers:locations", longitude, latitude, $"{driver.Id}");
        }
        public async Task<LocationDTO?> GetLocation(Guid userId)
        {
            Driver driver = await _driverRepo.GetByPredicate(x => x.UserId == userId);
            if (!driver.IsOnline)
            {
                throw new BadRequestException("The driver is offline");
            }
            var pos = await _db.GeoPositionAsync("drivers:locations", $"{driver.Id}");

            if(pos == null)
            {
                return null;
            }
            return new LocationDTO { Latitude = pos.Value.Latitude, Longitude = pos.Value.Longitude };
        }

        public async Task<bool> RemoveLocation(Guid userId)
        {
            Driver driver = await _driverRepo.GetByPredicate(x => x.UserId == userId);
            return await _db.GeoRemoveAsync("drivers:locations", $"{driver.Id}");
        }
        public async Task<List<NearDriversModel>> GetNearDrivers(double longitude, double latitude, double radius = 2000)
        {
            var distances = await _db.GeoSearchAsync("drivers:locations", longitude, latitude, new GeoSearchCircle(radius, GeoUnit.Meters), order: Order.Ascending);
            Console.WriteLine($"Found rides: {distances.Length}");
            var results = new List<NearDriversModel>();
            foreach (var distance in distances)
            {
                results.Add(new NearDriversModel
                {
                    DriverId = Guid.Parse(distance.Member),
                    DistanceMeter = distance.Distance
                });
            }
            return results;
        }
        public async Task<int> CountNearDrivers(double longitude, double latitude)
        {
            return (await GetNearDrivers(longitude, latitude)).Count;
        }
    }
}

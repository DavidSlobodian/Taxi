using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs;
using Taxi.App.DTOs.OutputDTO;
using Taxi.App.Services.LocationService.DriverLocationService;
using Taxi.Domain.Entities;
using Taxi.Infrastructure.Repos;

namespace Taxi.App.Services.LocationService.RideLocationService
{
    public class RideLocationService : IRideLocationService
    {
        private readonly IConnectionMultiplexer _multiplexer;
        private readonly IDatabase _db;
        private readonly IDriverLocationService _driverLocationService;
        public RideLocationService(IConnectionMultiplexer multiplexer, IDriverLocationService driverLocSrvc)
        {
            _multiplexer = multiplexer;
            _db = _multiplexer.GetDatabase();
            _driverLocationService = driverLocSrvc;
        }
        public async Task<bool> AddLocation(Guid rideId, double longitude, double latitude)
        {
            var res = await _db.GeoAddAsync("rides:locations", longitude, latitude, $"{rideId}");
            var count = await _db.SortedSetLengthAsync("rides:locations");

            var position = await _db.GeoPositionAsync(
                "rides:locations",
                rideId.ToString());

            Console.WriteLine($"Redis rides count: {count}");
            Console.WriteLine($"Ride ID: {rideId}");
            Console.WriteLine($"Ride position: {position}");
            return res;
        }

        public async Task<int> CountNearRides(double longitude, double latitude, double radius = 2000)
        {
            var res = await _db.GeoSearchAsync("rides:locations", longitude, latitude, new GeoSearchCircle(radius, GeoUnit.Meters), order: Order.Ascending);
            return res.Length;
        }
        public async Task<List<NearRidesDTO>> GetNearRides(Guid id, double radius = 2000)
        {
            LocationDTO? location = await _driverLocationService.GetLocation(id) ?? throw new Exception("Location error. The Driver isn't found");
            Console.WriteLine(await _db.SortedSetLengthAsync("rides:locations"));
            var distances = await _db.GeoSearchAsync("rides:locations", location.Longitude, location.Latitude, new GeoSearchCircle(radius, GeoUnit.Meters), order: Order.Ascending);
            Console.WriteLine($"Driver coordinates: {location.Longitude}, {location.Latitude}");
            Console.WriteLine($"Found rides: {distances.Length}");
            var results = new List<NearRidesDTO>();
            foreach (var distance in distances)
            {
                results.Add(new NearRidesDTO
                {
                    RideId = Guid.Parse(distance.Member),
                    DistanceToPickUpM = distance.Distance
                });
            }
            return results;
        }

        public async Task<bool> RemoveLocation(Guid rideId)
        {
            return await _db.GeoRemoveAsync("rides:locations", $"{rideId}");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs.OutputDTO;
using Taxi.Domain.Entities;

namespace Taxi.App.Services.LocationService.RideLocationService
{
    public interface IRideLocationService
    {
        Task<bool> AddLocation(Guid rideId, double longitude, double latitude);
        Task<bool> RemoveLocation(Guid rideId);
        Task<int> CountNearRides(double longitude, double latitude, double radius=2000);
        Task<List<NearRidesDTO>> GetNearRides(Guid id, double radis = 2000);

    }
}

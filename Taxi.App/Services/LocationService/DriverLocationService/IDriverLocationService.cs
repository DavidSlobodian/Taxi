using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs;
using Taxi.App.Models;

namespace Taxi.App.Services.LocationService.DriverLocationService
{
    public interface IDriverLocationService
    {
        Task UpdateLocation(Guid userId, double latitude, double longitude);
        Task<LocationDTO?> GetLocation(Guid userId);
        Task<bool> RemoveLocation(Guid userId);
        Task<List<NearDriversModel>> GetNearDrivers(double longitude, double latitude, double radius=2000);
        Task<int> CountNearDrivers(double longitude, double latitude);
    }
}

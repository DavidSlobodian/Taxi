using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs.OutputDTO;
using Taxi.Domain.Entities;

namespace Taxi.App.Services.VehicleService.VehicleManageService
{
    public interface IVehicleManageService
    {
        Task<List<VehicleOutput>> GetVehicles(Guid userId);
        Task<bool> SetActiveVehicle(Guid userId, Guid vehicleId);
        Task EditVehicle(Guid userId, Guid vehicleId, string newLicensePlate);
        Task DeleteVehicle(Guid vehicleId, Guid userId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs.InputDTO;
using Taxi.App.DTOs.OutputDTO;

namespace Taxi.App.Services.VehicleService.VehicleApplicationService
{
    public interface IVehicleApplicationService
    {
        Task<Guid> ApplyVehicle(VehicleDTO vehicleInfo, Guid userId);

        //Roles = admin
        Task<VehicleOutput> ReviewApplication(Guid applicId);
        Task<List<VehicleOutput>> GetAllVehicleApplics();
        Task<Guid> Approve(Guid vehicleApplicId); 
        Task Reject(Guid vehicleApplicId, string reason);
    }
}

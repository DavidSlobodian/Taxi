using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs.InputDTO;
using Taxi.App.DTOs.OutputDTO;
using Taxi.Domain.Entities;
using Taxi.Domain.Enums;

namespace Taxi.App.Services.RideService
{
    public interface IRideService
    {
        Task<Guid> CreateRide(Guid userId, RideDTO rideInfo);
        Task<bool> AcceptRide(Guid userId, Guid rideId);
        Task<bool> DriverArrived(Guid userId, Guid rideId);
        Task<bool> InProgress(Guid userId, Guid rideId);
        Task<bool> FinishRide(Guid userId, Guid rideId);
        Task<CancellationDTO> CancelRide(Guid userId, Guid rideId, CancellationReasons reason);
        Task<List<RideHistoryDTO>> GetHistoryOfRides(Guid userId);
    }
}

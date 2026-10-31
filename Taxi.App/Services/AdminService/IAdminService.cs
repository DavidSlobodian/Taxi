using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs;
using Taxi.App.DTOs.InputDTO;
using Taxi.App.DTOs.OutputDTO;
using Taxi.Domain.Entities;
using Taxi.Domain.Enums;

namespace Taxi.App.Services.AdminService
{
    public interface IAdminService
    {
        Task<DashboardDTO> GetDashboard(DateTime? dateFrom = null, DateTime? dateTo = null);
        Task<UserInfoDTO> GetUserById(Guid searchedUserId);
        Task<List<UserInfoDTO>> GetUsersByFilter(Guid? searchedUserId = null, string? firstname = null, string? lastname = null, string? phoneNumber = null, string? email = null, Roles? role = null, bool? isBlocked = null);
        Task<List<RideHistoryDTO>> GetUserRides(Guid userId);

        Task<BlockDTO> BlockUser(Guid adminId, Guid blockUserId);
        Task<BlockDTO> UnblockUser(Guid unblockUserId);

        Task<RideOutput> GetRide(Guid rideId);
        Task<List<RideOutput>> GetFinishedRides();
        Task<List<RideOutput>> GetActiveRides();
        Task<CancellationDTO> CancelRide(Guid rideId);

        Task<List<(Guid driverUserId, bool isAvailable)>> GetDriversOnline();
        Task<DriverDTO> GetDriverInfo(Guid driverUserId);
        Task<(Guid driverUserId, LocationDTO)> GetCurrentDriverLocation(Guid driverUserId);

        Task<ComplaintDTO> ChangeComplaintStatus(Guid userId, Guid complaintId, ComplaintStatus status);

        Task ExportAllRides();
        Task ExportAllUsers();
    }
}

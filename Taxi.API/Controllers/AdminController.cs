using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Taxi.API.Responses.ExceptionResponses;
using Taxi.App.DTOs;
using Taxi.App.DTOs.InputDTO;
using Taxi.App.DTOs.OutputDTO;
using Taxi.App.Services.AdminService;
using Taxi.App.Services.PricingService.PricingSettingsService;
using Taxi.Domain.Entities;
using Taxi.Domain.Enums;
using Taxi.Domain.Exceptions;
using Taxi.Domain.Responses;

namespace Taxi.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IPricingSettingsService _priceControl;
        private readonly IAdminService _adminService;
        public AdminController(IPricingSettingsService priceControl, IAdminService admService)
        {
            _priceControl = priceControl;
            _adminService = admService;
        }
        //==================== Dashboard & Users =========================
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(ApiResponse<DashboardDTO>), 200)]
        public async Task<IActionResult> GetDashboard(DateTime? dateFrom, DateTime? dateTo)
        {
            var result = await _adminService.GetDashboard(dateFrom, dateTo);
            return Ok(ApiResponse<DashboardDTO>.Ok(result));
        }
        [HttpGet("users")]
        [ProducesResponseType(typeof(ApiResponse<List<UserInfoDTO>>), 200)]
        public async Task<ActionResult<List<UserInfoDTO>>> GetUsersByFilter([FromQuery] Guid? searchedUserId, [FromQuery] string? firstname, [FromQuery] string? lastname,
        [FromQuery] string? phoneNumber, [FromQuery] string? email, [FromQuery] Roles? role, [FromQuery] bool? isBlocked)
        {
            var result = await _adminService.GetUsersByFilter(searchedUserId, firstname, lastname, phoneNumber, email, role, isBlocked);
            return Ok(ApiResponse<List<UserInfoDTO>>.Ok(result));
        }

        [HttpGet("users/{userId}/rides")]
        [ProducesResponseType(typeof(ApiResponse<List<RideHistoryDTO>>), 200)]
        public async Task<IActionResult> GetUserRides(Guid userId)
        {
            var result = await _adminService.GetUserRides(userId);
            return Ok(ApiResponse<List<RideHistoryDTO>>.Ok(result));
        }
        // ==================== Blocking ====================

        [HttpPost("users/{userId}/block")]
        [ProducesResponseType(typeof(ApiResponse<BlockDTO>), 200)]
        [ProducesResponseType(typeof(NotFoundExcResponse), 404)]
        [ProducesResponseType(typeof(ForbiddenExcResponse), 403)] // already blocked
        public async Task<IActionResult> BlockUser(Guid userId)
        {
            // Витягуємо ID адміністратора, який виконує блокування, з клеймів
            var adminId = GetUserIdFromClaims();
            if (adminId == null) return Unauthorized();

            var result = await _adminService.BlockUser(userId, adminId.Value);
            return Ok(ApiResponse<BlockDTO>.Ok(result));
        }

        [HttpPost("users/{unblockUserId}/unblock")]
        [ProducesResponseType(typeof(ApiResponse<BlockDTO>), 200)]
        [ProducesResponseType(typeof(NotFoundExcResponse), 404)]
        public async Task<IActionResult> UnblockUser(Guid unblockUserId)
        {
            var result = await _adminService.UnblockUser(unblockUserId);
            return Ok(ApiResponse<BlockDTO>.Ok(result));
        }

        // ==================== Rides ====================

        [HttpGet("rides/{rideId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<RideOutput>), 200)]
        [ProducesResponseType(typeof(NotFoundExcResponse), 404)]
        public async Task<IActionResult> GetRide(Guid rideId)
        {
            var result = await _adminService.GetRide(rideId);

            return Ok(ApiResponse<RideOutput>.Ok(result));
        }

        [HttpGet("rides/finished")]
        [ProducesResponseType(typeof(ApiResponse<List<RideOutput>>), 200)]
        public async Task<IActionResult> GetFinishedRides()
        {
            var result = await _adminService.GetFinishedRides();

            return Ok(ApiResponse<List<RideOutput>>.Ok(result));
        }

        [HttpGet("rides/active")]
        [ProducesResponseType(typeof(ApiResponse<List<RideOutput>>), 200)]

        public async Task<IActionResult> GetActiveRides()
        {
            var result = await _adminService.GetActiveRides();

            return Ok(ApiResponse<List<RideOutput>>.Ok(result));
        }

        [HttpPost("rides/{rideId:guid}/cancel")]
        [ProducesResponseType(typeof(ApiResponse<CancellationDTO>), 200)]
        [ProducesResponseType(typeof(NotFoundExcResponse), 404)]
        public async Task<IActionResult> CancelRide(Guid rideId)
        {
            var result = await _adminService.CancelRide(rideId);

            return Ok(ApiResponse<CancellationDTO>.Ok(result));
        }

        // ==================== Drivers ====================

        [HttpGet("drivers/online")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> GetDriversOnline()
        {
            var drivers = await _adminService.GetDriversOnline();

            var response = drivers.Select(d => new
            {
                DriverUserId = d.driverUserId,
                IsAvailable = d.isAvailable
            }).ToList();

            return Ok(ApiResponse<List<object>>.Ok(response.Cast<object>().ToList()));
        }

        [HttpGet("drivers/{driverUserId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<DriverDTO>), 200)]
        [ProducesResponseType(typeof(NotFoundExcResponse), 404)]
        public async Task<IActionResult> GetDriverInfo(
            Guid driverUserId)
        {
            var result = await _adminService.GetDriverInfo(driverUserId);

            return Ok(ApiResponse<DriverDTO>.Ok(result));
        }

        [HttpGet("drivers/{driverUserId:guid}/location")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(NotFoundExcResponse), 404)]
        public async Task<IActionResult> GetCurrentDriverLocation(Guid driverUserId)
        {
            var (driverId, location) =
                await _adminService.GetCurrentDriverLocation(driverUserId);

            var response = new
            {
                DriverUserId = driverId,
                Location = location
            };

            return Ok(ApiResponse<object>.Ok(response));
        }

        // ==================== Complaints & Exports ====================

        [HttpPatch("users/{userId:guid}/complaints/{complaintId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ComplaintDTO>), 200)]
        public async Task<IActionResult> ChangeComplaintStatus(Guid userId, Guid complaintId, [FromQuery] ComplaintStatus status)
        {
            var result = await _adminService.ChangeComplaintStatus(
                userId,
                complaintId,
                status);

            return Ok(ApiResponse<ComplaintDTO>.Ok(result));
        }

        [HttpGet("export/rides")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> ExportAllRides()
        {
            await _adminService.ExportAllRides();

            return Accepted(
                ApiResponse<object>.Ok(new
                {
                    message = "Export started or completed."
                }));
        }

        [HttpGet("export/users")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> ExportAllUsers()
        {
            await _adminService.ExportAllUsers();

            return Accepted(
                ApiResponse<object>.Ok(new
                {
                    message = "Export started or completed."
                }));
        }

        // ==================== Tariffs ====================

        [HttpGet("tarrifs")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> GetAllTarifs()
        {
            var result = await _priceControl.GetTariffs();

            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpPut("update-tarrifs")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> UpdateTarrif(VehicleClass vehicleClass,double? baseFare,double? waitingPricePerMin)
        {
            var result = await _priceControl.UpdateTariff(
                vehicleClass,
                baseFare,
                waitingPricePerMin);

            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpGet("price-controls")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> GetAllPriceControls()
        {
            var result = await _priceControl.GetPricingSettings();

            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpPut("upd-price-controls")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> UpdatePriceControl(double? avgFuelConsumption,double? avgFuelPrice)
        {
            var result = await _priceControl.UpdatePricingSettings(
                avgFuelConsumption,
                avgFuelPrice);

            return Ok(ApiResponse<object>.Ok(result));
        }

        // ==================== Helper Methods ====================

        private Guid? GetUserIdFromClaims()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(userIdClaim, out var userId))
                return userId;

            return null;
        }
    }
}

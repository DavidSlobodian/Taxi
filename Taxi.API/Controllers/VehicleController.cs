using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Taxi.API.Responses.ExceptionResponses;
using Taxi.App.DTOs.OutputDTO;
using Taxi.App.Services.VehicleService.VehicleManageService;
using Taxi.Domain.Responses;

namespace Taxi.API.Controllers
{
    [Route("api/manage-vehicles")]
    [Authorize(Policy = "AccountCheck", Roles = "Driver")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleManageService _vehManagService;
        public VehicleController(IVehicleManageService vehManag)
        {
            _vehManagService = vehManag;
        }
        [HttpGet("getall")]
        public async Task<IActionResult> GetAllVehicles()
        {
            Guid userId = GetUserId();
            return Ok(ApiResponse<List<VehicleOutput>>.Ok(await _vehManagService.GetVehicles(userId)));
        }
        [HttpPut("setactive/{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(NotFoundExcResponse), 404)]
        [ProducesResponseType(typeof(ConflictExcResponse), 409)]
        [ProducesResponseType(typeof(DataSavingExcResponse), 500)]
        public async Task<IActionResult> SetActiveCar(Guid id)
        {
            return Ok(ApiResponse<bool>.Ok(await _vehManagService.SetActiveVehicle(GetUserId(), id)));
        }
        [HttpPut("edit-vehicle")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(NotFoundExcResponse), 404)]
        [ProducesResponseType(typeof(ConflictExcResponse), 409)]
        [ProducesResponseType(typeof(DataSavingExcResponse), 500)]
        [ProducesResponseType(typeof(ValidationExcResponse), 422)]
        //[ProducesResponseType(typeof(InvalidFormatExcResponse), 400)]
        public async Task<IActionResult> EditVehicle(Guid vehicleId, string newLicensePlate)
        {
            Guid userId = GetUserId();
            await _vehManagService.EditVehicle(userId, vehicleId, newLicensePlate);
            return Ok(ApiResponse<string>.Ok("The vehicle has been updated"));
        }
        [HttpDelete("delete-vehicle")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(NotFoundExcResponse), 404)]
        [ProducesResponseType(typeof(ConflictExcResponse), 409)]
        [ProducesResponseType(typeof(DataSavingExcResponse), 500)]
        public async Task<IActionResult> DeleteVehicle(Guid vehicleId)
        {
            Guid userId = GetUserId();
            await _vehManagService.DeleteVehicle(vehicleId, userId);
            return Ok(ApiResponse<string>.Ok("The vehicle has been deleted"));
        }
        private Guid GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(claim, out var userId))
            {
                throw new UnauthorizedAccessException();
            }

            return userId;
        }
    }
}

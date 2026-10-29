using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Taxi.API.Responses.ExceptionResponses;
using Taxi.App.DTOs.InputDTO;
using Taxi.App.DTOs.OutputDTO;
using Taxi.App.Services.VehicleService.VehicleApplicationService;
using Taxi.App.Validators.VehicleValidators;
using Taxi.Domain.Responses;

namespace Taxi.API.Controllers
{
    [Route("api/vehicle-apply")]
    [ApiController]
    public class VehicleApplyController : ControllerBase
    {
        private readonly VehicleValidator _validator;
        private readonly IVehicleApplicationService _vehService;
        public VehicleApplyController(VehicleValidator validator, IVehicleApplicationService vehService)
        {
            _validator = validator;
            _vehService = vehService;
        }
        [HttpPost("apply")]
        [Authorize(Roles = "Driver")]
        [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
        [ProducesResponseType(typeof(ConflictExcResponse), 409)]
        [ProducesResponseType(typeof(DataSavingExcResponse), 500)]
        public async Task<IActionResult> Apply(VehicleDTO vehicleInfo)
        {
            var validationResult = await _validator.ValidateAsync(vehicleInfo);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }
            return Ok(ApiResponse<Guid>.Ok(await _vehService.ApplyVehicle(vehicleInfo, userId)));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("review-applic/{id}")]
        [ProducesResponseType(typeof(ApiResponse<VehicleOutput>), 200)]
        [ProducesResponseType(typeof(NotFoundExcResponse), 404)]
        public async Task<IActionResult> ReviewApplication(Guid id) 
        { 
            return Ok(ApiResponse<VehicleOutput>.Ok(await _vehService.ReviewApplication(id)));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-all-applics")]
        [ProducesResponseType(typeof(ApiResponse<List<VehicleOutput>>), 200)]
        public async Task<IActionResult> GetAllApplics()
        {
            return Ok(ApiResponse<List<VehicleOutput>>.Ok(await _vehService.GetAllVehicleApplics()));
        }

        [HttpPost("approve/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
        [ProducesResponseType(typeof(NotFoundExcResponse), 404)]
        [ProducesResponseType(typeof(ConflictExcResponse), 409)]
        [ProducesResponseType(typeof(DataSavingExcResponse), 500)]
        public async Task<IActionResult> Approve(Guid id)
        {    
            return Ok(ApiResponse<Guid>.Ok(await _vehService.Approve(id)));
        }
        [HttpPost("reject/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(NotFoundExcResponse), 404)]
        [ProducesResponseType(typeof(ConflictExcResponse), 409)]
        [ProducesResponseType(typeof(DataSavingExcResponse), 500)]
        public async Task<IActionResult> Reject(Guid id, string reason)
        {
            await _vehService.Reject(id, reason);
            return Ok(ApiResponse<string>.Ok("The application rejected successfully"));
        }
    }
}

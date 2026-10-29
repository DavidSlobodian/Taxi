using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Taxi.API.Responses.ExceptionResponses;
using Taxi.App.DTOs.InputDTO;
using Taxi.App.DTOs.OutputDTO;
using Taxi.App.Services.DriverService.DriverApplicationService;
using Taxi.App.Validators.DriverValidators;
using Taxi.Domain.Entities;
using Taxi.Domain.Responses;

namespace Taxi.API.Controllers
{
    [Route("api/driver-applications")]
    [ApiController]
    [Authorize]
    public class DriverApplyController : ControllerBase
    {
        private readonly DriverApplicationValidator _driverValid;
        private readonly IDriverApplicationService _driverApplyService;
        public DriverApplyController(DriverApplicationValidator driverValid, IDriverApplicationService driverService)
        {
            _driverValid = driverValid;
            _driverApplyService = driverService;
        }
        [HttpPost("apply")]
        [ProducesResponseType(typeof(ValidationExcResponse), 422)]
        public async Task<IActionResult> ApplyForDriver(DriverApplicationDTO driverInfo)
        {
            var validationResult = await _driverValid.ValidateAsync(driverInfo);

            if (!validationResult.IsValid)
            {
                return UnprocessableEntity(validationResult.Errors);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            return Ok(ApiResponse<Guid>.Ok(await _driverApplyService.Apply(userId, driverInfo)));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/review-applic/{applicationId}")]
        [ProducesResponseType(typeof(ApiResponse<DriverApplicOutput>), 200)]
        [ProducesResponseType(typeof(NotFoundExcResponse), 404)]
        public async Task<IActionResult> ReviewApplication(Guid applicationId)
        {
            return Ok(ApiResponse<DriverApplicOutput>.Ok(await _driverApplyService.ReviewApplication(applicationId)));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/get-all-applics")]
        public async Task<IActionResult> GetAllApplications()
        {
            return Ok(ApiResponse<List<DriverApplicOutput>>.Ok(await _driverApplyService.GetAllApplications()));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("approve/{id}")]
        [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
        [ProducesResponseType(typeof(NotFoundExcResponse), 404)]
        [ProducesResponseType(typeof(ConflictExcResponse), 422)]
        public async Task<IActionResult> Approve(Guid id)
        {
            return Ok(ApiResponse<Guid>.Ok(await _driverApplyService.Approve(id)));
        }

        // тільки Admin
        [Authorize(Roles = "Admin")]
        [HttpPost("reject/{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(NotFoundExcResponse), 404)]
        [ProducesResponseType(typeof(ConflictExcResponse), 422)]
        public async Task<IActionResult> Reject(Guid id, string reason)
        {
            await _driverApplyService.Reject(id, reason);
            return Ok(ApiResponse<string>.Ok("Ride rejected successfully."));
        }
    }
}

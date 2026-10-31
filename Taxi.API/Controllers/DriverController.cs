using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Taxi.API.Responses.ExceptionResponses;
using Taxi.App.DTOs;
using Taxi.App.Services.LocationService.DriverLocationService;
using Taxi.Domain.Responses;

namespace Taxi.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "AccountCheck", Roles = "Driver")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly IDriverLocationService _locationService;
        public DriverController(IDriverLocationService lctSrvc)
        {
            _locationService = lctSrvc;
        }
        [HttpGet("location")]
        [ProducesResponseType(typeof(ApiResponse<LocationDTO>), 200)]
        [ProducesResponseType(typeof(BadRequestExcResponse), 400)]

        public async Task<IActionResult> GetLocation()
        {
            return Ok(ApiResponse<LocationDTO?>.Ok(await _locationService.GetLocation(GetUserId())));
        }
        [HttpPut("location")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(BadRequestExcResponse), 400)]
        public async Task<IActionResult> UpdateLocation(double latitude, double longitude)
        {
            await _locationService.UpdateLocation(GetUserId(), latitude, longitude);
            return Ok(ApiResponse<string>.Ok("Location updated successfully."));
        }
        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                throw new UnauthorizedAccessException();
            }
            return userId;
        }
    }
}

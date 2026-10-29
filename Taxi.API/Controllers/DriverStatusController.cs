using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;
using System.Threading.Tasks;
using Taxi.API.Responses.ExceptionResponses;
using Taxi.App.Services.DriverService.DriverStatusService;
using Taxi.Domain.Responses;

namespace Taxi.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Driver")]
    [ApiController]
    public class DriverStatusController : ControllerBase
    {
        private readonly IDriverStatusService _driverStatusService;
        public DriverStatusController(IDriverStatusService driverStSrvc)
        {
            _driverStatusService = driverStSrvc;
        }

        [HttpPut("goonline")]
        [ProducesResponseType(typeof(ForbiddenExcResponse), 403)]
        public async Task<IActionResult> GoOnline()
        {
            return Ok(ApiResponse<bool>.Ok(await _driverStatusService.GoOnline(GetUserId())));
        }
        [HttpPut("gooffline")]
        public async Task<IActionResult> GoOffline()
        {
            return Ok(ApiResponse<bool>.Ok(await _driverStatusService.GoOffline(GetUserId())));
        }
        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException();
            }
            return userId;
        }
    }
}

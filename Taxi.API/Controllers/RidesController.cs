using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taxi.API.Responses.ExceptionResponses;
using Taxi.App.DTOs.InputDTO;
using Taxi.App.DTOs.OutputDTO;
using Taxi.App.Services.DriverMatchingService;
using Taxi.App.Services.RideService;
using Taxi.Domain.Enums;
using Taxi.Domain.Responses;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RidesController : ControllerBase
{
    private readonly IRideService _rideService;
    private readonly IDriverMatchingService _matchingService;

    public RidesController(
        IRideService rideService,
        IDriverMatchingService matchingService)
    {
        _rideService = rideService;
        _matchingService = matchingService;
    }

    // POST: api/rides
    // Пасажир створює поїздку.
    [HttpPost]
    [Authorize(Roles = "Passenger")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(DataSavingExcResponse), 500)]
    public async Task<IActionResult> CreateRide([FromBody] RideDTO dto)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var ride = await _rideService.CreateRide(userId, dto);
        return Ok(ApiResponse<Guid>.Ok(ride));
    }

    // GET: api/rides/available?radiusMeters=5000
    // Водій отримує доступні йому поїздки.
    [HttpGet("available")]
    [Authorize(Roles = "Driver")]
    [ProducesResponseType(typeof(ApiResponse<List<NearRidesDTO>>), 200)]
    public async Task<IActionResult> GetAvailableRides(
        [FromQuery] double radiusMeters = 5000)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        if (radiusMeters <= 0 || radiusMeters > 5000)
            return BadRequest("Radius must be between 0 and 5000 meters.");

        var rides = await _matchingService.GetAllNearRides(userId, radiusMeters);

        return Ok(ApiResponse<List<NearRidesDTO>?>.Ok(rides));
    }

    // POST: api/rides/{rideId}/accept
    // Водій приймає поїздку.
    [HttpPost("{rideId:guid}/accept")]
    [Authorize(Roles = "Driver")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ConflictExcResponse), 409)]
    public async Task<IActionResult> AcceptRide(Guid rideId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var accepted = await _rideService.AcceptRide(userId, rideId);

        if (!accepted)
        {
            return Conflict(new
            {
                message = "The ride is no longer available"
            });
        }

        return Ok(ApiResponse<object>.Ok(new
        {
            rideId,
            status = RideStatus.Accepted
        }));
    }

    // PUT: api/rides/{rideId}/driver-arrived
    [HttpPut("{rideId:guid}/driver-arrived")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(NotFoundExcResponse), 404)]
    [ProducesResponseType(typeof(ConflictExcResponse), 409)]
    [ProducesResponseType(typeof(BadRequestExcResponse), 400)]
    [ProducesResponseType(typeof(DataSavingExcResponse), 500)]
    public async Task<IActionResult> DriverArrived(Guid rideId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var arrived = await _rideService.DriverArrived(userId, rideId);
        return Ok(ApiResponse<bool>.Ok(arrived));
    }
    // PUT: api/rides/{rideId}/start-ride
    [HttpPut("{rideId:guid}/start-ride")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(NotFoundExcResponse), 404)]
    [ProducesResponseType(typeof(ConflictExcResponse), 409)]
    [ProducesResponseType(typeof(DataSavingExcResponse), 500)]
    public async Task<IActionResult> StartRide(Guid rideId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var start = await _rideService.InProgress(userId, rideId);
        return Ok(ApiResponse<bool>.Ok(start));
    }

    // PUT: api/rides/{rideId}/finish-ride
    [HttpPut("{rideId:guid}/finish-ride")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(NotFoundExcResponse), 404)]
    [ProducesResponseType(typeof(ConflictExcResponse), 409)]
    [ProducesResponseType(typeof(BadRequestExcResponse), 400)]
    [ProducesResponseType(typeof(DataSavingExcResponse), 500)]
    public async Task<IActionResult> FinishRide(Guid rideId)
    {
        if(!TryGetUserId(out var userId))
            return Unauthorized();

        var finish = await _rideService.FinishRide(userId, rideId);
        return Ok(ApiResponse<bool>.Ok(finish));
    }

    // PUT: api/rides/{rideId}/cancel
    [HttpPut("{rideId:guid}/cancel")]
    [ProducesResponseType(typeof(ApiResponse<CancellationDTO>), 200)]
    [ProducesResponseType(typeof(NotFoundExcResponse), 404)]
    public async Task<IActionResult> CancelRide(Guid rideId, CancellationReasons reason)
    {
        if(!TryGetUserId(out var userId))
            return Unauthorized();

        var cancelled = await _rideService.CancelRide(userId, rideId, reason);
        return Ok(ApiResponse<CancellationDTO?>.Ok(cancelled));
    }
    // GET: api/rides/get-history
    [HttpGet("get-history")]
    [ProducesResponseType(typeof(ApiResponse<List<RideHistoryDTO>>), 200)]
    public async Task<IActionResult> GetRidesHistory()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();
        var list = await _rideService.GetHistoryOfRides(userId);
        return Ok(ApiResponse<List<RideHistoryDTO>?>.Ok(list));
    }

    private bool TryGetUserId(out Guid userId)
    {
        var userIdClaim = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        return Guid.TryParse(userIdClaim, out userId);
    }
}

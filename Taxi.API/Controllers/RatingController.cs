using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Taxi.API.Responses.ExceptionResponses;
using Taxi.App.DTOs.InputDTO;
using Taxi.App.DTOs.OutputDTO;
using Taxi.App.Models;
using Taxi.App.Services.RatingService;
using Taxi.Domain.Responses;

namespace Taxi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AccountCheck")]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;
        public RatingController(IRatingService ratingSrvc)
        {
            _ratingService = ratingSrvc;
        }
        //POST: api/rating/rate
        [HttpPost("rate")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(NotFoundExcResponse), 404)]
        [ProducesResponseType(typeof(ValidationExcResponse), 422)]
        [ProducesResponseType(typeof(ForbiddenExcResponse), 403)]
        [ProducesResponseType(typeof(ConflictExcResponse), 409)]
        public async Task<IActionResult> Rate(RatingDTO ratingInfo)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            var res = await _ratingService.Rate(userId, ratingInfo);
            return Ok(ApiResponse<bool>.Ok(res));
        }
        //GET: api/rating/summary
        [HttpGet("summary")]
        [ProducesResponseType(typeof(ApiResponse<RatingReview>), 200)]
        public async Task<IActionResult> GetSummary()
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }
            var res = await _ratingService.GetUserReview(userId);
            return Ok(ApiResponse<RatingReview>.Ok(res));
        }
        //GET: api/rating/ratinginfo
        [HttpGet("ratinginfo")]
        [ProducesResponseType(typeof(ApiResponse<List<RatingInfoDTO>>), 200)]
        public async Task<IActionResult> GetAllRatings()
        {
            if(!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }
            var res = await _ratingService.GetRatings(userId);
            return Ok(ApiResponse<List<RatingInfoDTO>>.Ok(res));
        }
        private bool TryGetUserId(out Guid userId)
        {
            var userIdClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return Guid.TryParse(userIdClaim, out userId);
        }
    }
}

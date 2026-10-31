using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Taxi.API.Responses.ExceptionResponses;
using Taxi.App.Services.PaymentService;
using Taxi.Domain.Responses;

namespace Taxi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public UserController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [Authorize(Policy = "AccountCheck", Roles = "Passenger")]
        [HttpPut("add-balance/{amount}")]
        [ProducesResponseType(typeof(DataSavingExcResponse), 500)]
        public async Task<IActionResult> AddBalance(double amount)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }
            return Ok(ApiResponse<bool>.Ok(await _paymentService.AddUserBalance(userId, amount)));
        }
        private bool TryGetUserId(out Guid userId)
        {
            var userIdClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return Guid.TryParse(userIdClaim, out userId);
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Taxi.App.Services.AuthService;
using Taxi.App.Validators.UserValidators;
using Taxi.App.DTOs.InputDTO;
using Taxi.Domain.Responses;
using Taxi.Domain.Exceptions;
using Taxi.API.Responses.ExceptionResponses;
using Taxi.App.DTOs.OutputDTO;

namespace Taxi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly RegistrationValidator _regValid;
        private readonly LoginValidator _loginValid;
        public AuthController(IAuthService authService, RegistrationValidator regValid, LoginValidator loginValid)
        {
            _authService = authService;
            _regValid = regValid;
            _loginValid = loginValid;
        }

        [HttpPost("signup")]
        [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
        [ProducesResponseType(typeof(ForbiddenExcResponse), 403)]
        public async Task<IActionResult> SignUpAsync(RegistrationDto user)
        {
            var validationResult = await _regValid.ValidateAsync(user);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            return Ok(ApiResponse<Guid>.Ok(await _authService.SignUp(user)));
        }
        [HttpPost("signin")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(NotFoundExcResponse), 404)]
        [ProducesResponseType(typeof(UnauthorizedException), 401)]
        public async Task<IActionResult> SignInAsync(LoginDto user)
        {
            var validationResult = await _loginValid.ValidateAsync(user);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            return Ok(ApiResponse<string>.Ok(await _authService.SignIn(user)));
        }
    }
}

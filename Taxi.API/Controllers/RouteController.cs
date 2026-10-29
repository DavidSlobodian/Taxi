using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Taxi.App.DTOs;
using Taxi.App.DTOs.InputDTO;
using Taxi.App.Models;
using Taxi.App.Services.PricingService.PriceCalculationService;
using Taxi.App.Services.PricingService.RouteService;
using Taxi.Domain.Enums;
using Taxi.Domain.Responses;

namespace Taxi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly IRouteService _route;
        private readonly IPriceCalculationService _priceCalculationService;
        public RouteController(IRouteService route, IPriceCalculationService prClSrvc)
        {
            _route = route;
            _priceCalculationService = prClSrvc;
        }
        [HttpPost]
        public async Task<IActionResult> GetRoute(RouteDTO routeInfo)
        {
            return Ok(ApiResponse<RouteInfo>.Ok(await _route.GetRouteInfo(routeInfo)));
        }
        [HttpPost("calculate-price")]
        public async Task<IActionResult> GetPrice(RideDTO rideInfo)
        {
            return Ok(ApiResponse<PriceInfo>.Ok(await _priceCalculationService.CalculatePrice(rideInfo)));
        }
    }
}

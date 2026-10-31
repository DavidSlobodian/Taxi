using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs;
using Taxi.App.Models;

namespace Taxi.App.Services.PricingService.RouteService
{
    public interface IRouteService
    {
        Task<RouteInfo> GetRouteInfo(RouteDTO routeInfo); 
    }
}

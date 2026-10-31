using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs.InputDTO;
using Taxi.App.Models;

namespace Taxi.App.Services.PricingService.PriceCalculationService
{
    public interface IPriceCalculationService
    {
        Task<PriceInfo> CalculatePrice(RideDTO rideInfo);
    }
}

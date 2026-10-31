using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.App.Services.PricingService.WeatherService
{
    public interface IWeatherService
    {
        Task<bool> IsBadWeather(double latitude, double longitude);
    }
}

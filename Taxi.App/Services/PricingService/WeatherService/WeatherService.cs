using OpenMeteo;
using OpenMeteo.Weather.Forecast.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Taxi.App.Services.PricingService.WeatherService
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<bool> IsBadWeather(double latitude, double longitude)
        {
            string url = $"https://api.open-meteo.com/v1/forecast?latitude={latitude.ToString(CultureInfo.InvariantCulture)}&longitude={longitude.ToString(CultureInfo.InvariantCulture)}&current=weather_code";

            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var document = JsonDocument.Parse(content);

            var weatherCode = document.RootElement
                .GetProperty("current")
                .GetProperty("weather_code")
                .GetInt32();

            if (weatherCode != 0 || weatherCode != 1 || weatherCode != 2 || weatherCode != 3)
            {
                return true;
            }
            return false;
            
        }
    }
}

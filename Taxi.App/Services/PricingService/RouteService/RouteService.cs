using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Taxi.App.DTOs;
using Taxi.App.Models;

namespace Taxi.App.Services.PricingService.RouteService
{
    public class RouteService : IRouteService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        public RouteService(HttpClient client, IConfiguration config) 
        {
            _client = client;
            _configuration = config;
        }
        public async Task<RouteInfo> GetRouteInfo(RouteDTO routeInfo)
        {
            var apiKey = _configuration["OpenRouteService:Key"];

            var url = $"https://api.heigit.org/openrouteservice/v2/directions/driving-car?"
                + $"api_key={apiKey}"
                + $"&start={routeInfo.PickupLongtitude.ToString().Replace(',','.')},{routeInfo.PickupLatitude.ToString().Replace(',', '.')}"
                + $"&end={routeInfo.DestinationLongtitude.ToString().Replace(',','.')},{routeInfo.DestinationLatitude.ToString().Replace(',', '.')}";

            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
           
            var json = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(json);

            var summary = document.RootElement
                .GetProperty("features")[0]
                .GetProperty("properties")
                .GetProperty("summary");

            double distance = summary.GetProperty("distance").GetDouble();
            double durationsec = summary.GetProperty("duration").GetDouble();

            return new RouteInfo { DistanceKm = distance / 1000, DurationMinutes = durationsec / 60 };
        }
    }
}

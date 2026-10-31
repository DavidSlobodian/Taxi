using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs;
using Taxi.App.DTOs.InputDTO;
using Taxi.App.Models;
using Taxi.App.Services.LocationService.DriverLocationService;
using Taxi.App.Services.LocationService.RideLocationService;
using Taxi.App.Services.PricingService.PricingSettingsService;
using Taxi.App.Services.PricingService.RouteService;
using Taxi.App.Services.PricingService.WeatherService;
using Taxi.Domain.Entities;
using Taxi.Infrastructure.Repos;

namespace Taxi.App.Services.PricingService.PriceCalculationService
{
    public class PriceCalculationService : IPriceCalculationService
    {
        private readonly IRouteService _routeService;
        private readonly IWeatherService _weatherService;
        private readonly IPricingSettingsService _pricingSetService;
        private readonly IGenericRepository<Tariff> _tariffRepo;
        private readonly IDriverLocationService _driverGeoService;
        private readonly IGenericRepository<Driver> _driverRepo;
        private readonly IRideLocationService _rideGeoService;
        
        public PriceCalculationService(IRouteService routeService, IPricingSettingsService pricingSetService, 
            IWeatherService weatherService, IGenericRepository<Tariff> tariffRepo, 
            IDriverLocationService driverGeoService, IGenericRepository<Driver> driverRepo,
            IRideLocationService rideGeoService)
        {
            _routeService = routeService;
            _pricingSetService = pricingSetService;
            _weatherService = weatherService;
            _tariffRepo = tariffRepo;
            _driverGeoService = driverGeoService;
            _driverRepo = driverRepo;
            _rideGeoService = rideGeoService;
        }

        public async Task<PriceInfo> CalculatePrice(RideDTO rideInfo)
        {
            RouteInfo routeInfo = await _routeService.GetRouteInfo(new RouteDTO
            {
                PickupLatitude = rideInfo.PickupLatitude,
                PickupLongtitude = rideInfo.PickupLongtitude,
                DestinationLatitude = rideInfo.DestinationLatitude,
                DestinationLongtitude = rideInfo.DestinationLongtitude
            });
            PricingSettings pricingSet = await _pricingSetService.GetPricingSettings();
            Tariff tariff = await _tariffRepo.GetByPredicate(x => x.Class == rideInfo.RequestedClass);

            double distanceKm = routeInfo.DistanceKm;
            var FuelCostPerKm = (pricingSet.AverageFuelPrice * pricingSet.AverageFuelCons) / 100;


            var CostPerKm = FuelCostPerKm + tariff.BasePricePerKm;
            if(await _weatherService.IsBadWeather(rideInfo.PickupLatitude, rideInfo.PickupLongtitude))
            {
                CostPerKm = CostPerKm * 1.05;
            }
            if (DateTime.Now.Hour >= 7 && DateTime.Now.Hour <= 10 || DateTime.Now.Hour >= 17 && DateTime.Now.Hour <= 20)
            {
                CostPerKm = CostPerKm * 1.05;
            }

            int rides = await _rideGeoService.CountNearRides(rideInfo.PickupLongtitude, rideInfo.PickupLatitude) + 1;
            int drivers = await _driverGeoService.CountNearDrivers(rideInfo.PickupLongtitude, rideInfo.PickupLatitude);

            double coefficient = CountCoefficient(rides, drivers);
            CostPerKm *= coefficient;

            return new PriceInfo
            {
                DistanceInKm = distanceKm,
                PricePerKm = Math.Round(CostPerKm, 2),
                TotalPrice = Math.Round(tariff.BaseFare + (CostPerKm * distanceKm), 2)
            };
        }

        private static double CountCoefficient(int rides, int drivers)
        {
            if (drivers == 0) return 1.5;

            double ratio = (double)rides / drivers;
            if (ratio <= 1)
                return 1;
            else if (ratio <= 1.5)
                return 1.1;
            else if (ratio <= 2)
                return 1.2;
            else if (ratio <= 3)
                return 1.35;
            else if(ratio >= 3)
                return 1.5;
            return 1;
        }

        //попит - кількість замовлень в даному радіусі, пропозиція - кількість таксистів в даному радіусі
        /*private async Task<List<Driver>> GetAvailableNearDrivers(double longitude, double latitude)
        {
            Dictionary<string, double?> results = await _driverGeoService.GetNearDrivers(longitude, latitude);
            List<Guid> driverIds = results.Keys.Select(Guid.Parse).ToList();

            return await _driverRepo.GetManyByPredicate(x => driverIds.Contains(x.Id) && x.IsOnline && x.IsAvailable);
        }*/
    }
}

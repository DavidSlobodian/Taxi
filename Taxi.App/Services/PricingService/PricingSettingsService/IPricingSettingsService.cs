using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Domain.Entities;
using Taxi.Domain.Enums;

namespace Taxi.App.Services.PricingService.PricingSettingsService
{
    public interface IPricingSettingsService
    {
        Task<List<Tariff>> GetTariffs();
        Task<bool> UpdateTariff(VehicleClass vehClass, double? BaseFare, double? WaitingPricePerMin);

        Task<PricingSettings> GetPricingSettings();
        Task<bool> UpdatePricingSettings(double? AvgFuelConsumption, double? AvgFuelPrice);
    }
}

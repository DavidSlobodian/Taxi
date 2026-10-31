using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Domain.Entities;
using Taxi.Domain.Enums;
using Taxi.Infrastructure;
using Taxi.Infrastructure.Repos;

namespace Taxi.App.Services.PricingService.PricingSettingsService
{
    public class PricingSettingsService : IPricingSettingsService
    {
        private readonly IGenericRepository<Tariff> _tariffRepo;
        private readonly IGenericRepository<PricingSettings> _pricingRepo;
        private readonly AppDbContext _dbContext;
        public PricingSettingsService(IGenericRepository<Tariff> tariffRepo, IGenericRepository<PricingSettings> pricingRepo, AppDbContext dbContext)
        {
            _pricingRepo = pricingRepo;
            _tariffRepo = tariffRepo;
            _dbContext = dbContext;
        }
        public async Task<PricingSettings> GetPricingSettings()
        {
            return (await _pricingRepo.GetAll()).First();
        }

        public async Task<List<Tariff>> GetTariffs()
        {
            return (await _tariffRepo.GetAll()).ToList();
        }

        public async Task<bool> UpdatePricingSettings(double? AvgFuelConsumption, double? AvgFuelPrice)
        {
            PricingSettings pricingSettings = (await _pricingRepo.GetAll()).First();
            if(AvgFuelConsumption.HasValue)
            {
                pricingSettings.AverageFuelCons = AvgFuelConsumption.Value;
            }
            if (AvgFuelPrice.HasValue)
            {
                pricingSettings.AverageFuelPrice = AvgFuelPrice.Value;
            }
            _pricingRepo.Update(pricingSettings);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateTariff(VehicleClass vehClass, double? BaseFare, double? WaitingPricePerMin)
        {
            Tariff tariff = await _tariffRepo.GetByPredicate(x => x.Class == vehClass);
            if (BaseFare.HasValue)
            {
                tariff.BaseFare = BaseFare.Value;
            }
            if (WaitingPricePerMin.HasValue)
            {
                tariff.WaitingPricePerMin = WaitingPricePerMin.Value;
            }
            _tariffRepo.Update(tariff);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}

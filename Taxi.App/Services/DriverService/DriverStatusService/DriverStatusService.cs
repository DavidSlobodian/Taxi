using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.Services.LocationService.DriverLocationService;
using Taxi.Domain.Entities;
using Taxi.Domain.Exceptions;
using Taxi.Infrastructure;
using Taxi.Infrastructure.Repos;

namespace Taxi.App.Services.DriverService.DriverStatusService
{
    public class DriverStatusService : IDriverStatusService
    {
        private readonly IGenericRepository<Driver> _driverRepo;
        private readonly IGenericRepository<Vehicle> _vehicleRepo;
        private readonly AppDbContext _dbContext;
        private readonly IDriverLocationService _locationService;
        public DriverStatusService(IGenericRepository<Driver> driverRepo, IGenericRepository<Vehicle> vehicleRepo, AppDbContext dbContext, IDriverLocationService lctSrvc)
        {
            _driverRepo = driverRepo;
            _vehicleRepo = vehicleRepo;
            _dbContext = dbContext;
            _locationService = lctSrvc;
        }
        public async Task<bool> GoOnline(Guid userId)
        {
            Driver driver = await _driverRepo.GetByPredicate(x => x.UserId == userId);
            if(driver.ActiveVehicleId == null)
            {
                throw new ForbiddenException("The driver doesn't have an active vehicle");
            }
            driver.IsAvailable = true;
            driver.IsOnline = true;
            _driverRepo.Update(driver);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> GoOffline(Guid userId)
        {
            Driver driver = await _driverRepo.GetByPredicate(x => x.UserId == userId);
            driver.IsAvailable = false;
            driver.IsOnline = false;
            _driverRepo.Update(driver);
            await _locationService.RemoveLocation(userId);
            await _dbContext.SaveChangesAsync();
            return true;
        }

    }
}

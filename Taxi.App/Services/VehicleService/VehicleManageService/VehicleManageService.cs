using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Taxi.App.DTOs;
using Taxi.App.DTOs.OutputDTO;
using Taxi.Domain.Entities;
using Taxi.Domain.Exceptions;
using Taxi.Infrastructure;
using Taxi.Infrastructure.Repos;

namespace Taxi.App.Services.VehicleService.VehicleManageService
{
    public class VehicleManageService : IVehicleManageService
    {
        private readonly IGenericRepository<Vehicle> _vehicleRepos;
        private readonly IGenericRepository<Driver> _driverRepos;
        private readonly AppDbContext _dbContext;
        public VehicleManageService(IGenericRepository<Vehicle> vehRepos, IGenericRepository<Driver> driverRepos, AppDbContext dbContext)
        {
            _driverRepos = driverRepos;
            _vehicleRepos = vehRepos;
            _dbContext = dbContext;
        }
        public async Task<List<VehicleOutput>> GetVehicles(Guid userId)
        {
            Driver driver = await _driverRepos.GetByPredicate(x => x.User.Id == userId);
            List<VehicleOutput> response = new();
            foreach (var vehicle in await _vehicleRepos.GetManyByPredicate(x => x.DriverId == driver.Id))
            {
                response.Add(new VehicleOutput
                {
                    Id = vehicle.Id,
                    Color = vehicle.Color,
                    LicensePlate = vehicle.LicensePlate,
                    Mark = vehicle.Mark,
                    Model = vehicle.Model,
                    Year = vehicle.Year,
                    Class = vehicle.Class,
                    VerificationStatus = Domain.Enums.VehicleVerificationStatus.Approved,
                    RejectionReason = null
                });
            }
            return response;
            
        }
        public async Task<bool> SetActiveVehicle(Guid userId, Guid vehicleId)
        {
            Driver driver = await _driverRepos.GetByPredicate(x => x.UserId == userId);
            Vehicle veh = await _vehicleRepos.GetById(vehicleId);
            if (veh == null)
            {
                throw new NotFoundException("vehicle");
            }
            if (veh.DriverId != driver.Id)
            {
                throw new ConflictException("vehicle doesn't belong to this user");
            }
            driver.ActiveVehicleId = vehicleId;
            _driverRepos.Update(driver);
            if (await _dbContext.SaveChangesAsync() == 0)
                throw new DataSavingException("database");
            return true;
        }
        public async Task EditVehicle(Guid userId, Guid vehicleId, string newLicensePlate)
        {
            Driver driver = await _driverRepos.GetByPredicate(x => x.User.Id == userId);
            Vehicle veh = await _vehicleRepos.GetById(vehicleId);
            if (veh == null)
            {
                throw new NotFoundException("vehicle");
            }
            if (veh.DriverId != driver.Id)
            {
                throw new ConflictException("vehicle doesn't belong to this user");
            }
            string pattern = @"^[ABCDEHIKMOPTXabcdehikmoptx]{2}\d{4}[ABCDEGHIKMOPTXYZabcdeghikmoptxyz]{2}$";
            if (!Regex.Match(newLicensePlate, pattern).Success)
            {
                throw new ValidationException("license plate");
            }
            if(await _vehicleRepos.GetByPredicate(x => x.LicensePlate == newLicensePlate) != null)
            {
                throw new ValidationException("license plate");
            }

            veh.LicensePlate = newLicensePlate;
            _vehicleRepos.Update(veh);
            if (await _dbContext.SaveChangesAsync() == 0)
                throw new DataSavingException("database");

        }
        public async Task DeleteVehicle(Guid vehicleId, Guid userId)
        {
            Driver driver = await _driverRepos.GetByPredicate(x => x.UserId == userId);
            Vehicle veh = await _vehicleRepos.GetById(vehicleId);
            if (veh == null)
            {
                throw new NotFoundException("vehicle");
            }
            if (veh.DriverId != driver.Id)
            {
                throw new ConflictException("vehicle doesn't belong to this user");
            }
            driver.ActiveVehicleId = null;
            driver.ActiveVehicle = null;
            _driverRepos.Update(driver);
            _vehicleRepos.Delete(veh);
            if (await _dbContext.SaveChangesAsync() == 0)
                throw new DataSavingException("database");
        }
    }
}

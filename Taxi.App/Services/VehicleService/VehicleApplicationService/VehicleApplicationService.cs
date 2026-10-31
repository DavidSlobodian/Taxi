using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs.InputDTO;
using Taxi.App.DTOs.OutputDTO;
using Taxi.Domain.Entities;
using Taxi.Domain.Exceptions;
using Taxi.Infrastructure;
using Taxi.Infrastructure.Repos;

namespace Taxi.App.Services.VehicleService.VehicleApplicationService
{
    public class VehicleApplicationService : IVehicleApplicationService
    {
        private readonly IGenericRepository<Driver> _driverRepos;
        private readonly IGenericRepository<VehicleApplication> _vehicleApplicRepos;
        private readonly IGenericRepository<Vehicle> _vehicleRepos;
        private readonly AppDbContext _dbContext;
        public VehicleApplicationService(IGenericRepository<VehicleApplication> vehicleApplicRepos, IGenericRepository<Driver> driverRepos, 
            AppDbContext dbContext, IGenericRepository<Vehicle> vehicleRepos)
        {
            _vehicleApplicRepos = vehicleApplicRepos;
            _driverRepos = driverRepos;
            _dbContext = dbContext;
            _vehicleRepos = vehicleRepos;
        }

        public async Task<Guid> ApplyVehicle(VehicleDTO vehicleInfo, Guid userId)
        {
            if (await _vehicleRepos.GetByPredicate(x => x.LicensePlate == vehicleInfo.LicensePlate) != null)
            {
                throw new ConflictException("Car already exists");
            }
            if(await _vehicleApplicRepos.GetByPredicate(x => x.LicensePlate == vehicleInfo.LicensePlate && x.VerificationStatus == Domain.Enums.VehicleVerificationStatus.Pending) != null)
            {
                throw new ConflictException("Application already exists");
            }

            Driver driver = await _driverRepos.GetByPredicate(x => x.UserId == userId);
            VehicleApplication vehicle = new()
            {
                DriverId = driver.Id,
                Mark = vehicleInfo.Mark,
                Model = vehicleInfo.Model,
                Year = vehicleInfo.Year,
                LicensePlate = vehicleInfo.LicensePlate.Trim().ToUpperInvariant(),
                Color = vehicleInfo.Color.Trim().ToLowerInvariant(),
                VerificationStatus = Domain.Enums.VehicleVerificationStatus.Pending,
                Class = vehicleInfo.Class,
            };
            Guid applicId = _vehicleApplicRepos.Add(vehicle);
            if(await _dbContext.SaveChangesAsync() == 0)
                throw new DataSavingException("Error saving vehicle application");
            return applicId;
        }
        public async Task<VehicleOutput> ReviewApplication(Guid applicId)
        {
            VehicleApplication applic = await _vehicleApplicRepos.GetById(applicId);
            if (applic == null) throw new NotFoundException("application");
            return new()
            {
                Id = applic.Id,
                Mark = applic.Mark,
                Model = applic.Model,
                Color = applic.Color,
                Class = applic.Class,
                LicensePlate = applic.LicensePlate,
                Year = applic.Year,
                VerificationStatus= applic.VerificationStatus,
                RejectionReason = applic.RejectReason
            };
        }
        public async Task<List<VehicleOutput>> GetAllVehicleApplics()
        {
            List<VehicleApplication> applics = (await _vehicleApplicRepos.GetAll()).ToList();
            List<VehicleOutput> result = new();
            foreach(var applic in applics)
            {
                result.Add(new()
                {
                    Id = applic.Id,
                    Mark = applic.Mark,
                    Model = applic.Model,
                    Color = applic.Color,
                    Class = applic.Class,
                    LicensePlate = applic.LicensePlate,
                    Year = applic.Year,
                    VerificationStatus = applic.VerificationStatus,
                    RejectionReason = applic.RejectReason
                });
            }
            return result;
        }

        public async Task<Guid> Approve(Guid vehicleApplicId)
        {
            VehicleApplication applic = await _vehicleApplicRepos.GetById(vehicleApplicId);
            if (applic == null)
            {
                throw new NotFoundException("application");
            }
            if(applic.VerificationStatus == Domain.Enums.VehicleVerificationStatus.Rejected)
            {
                throw new ConflictException("The application is rejected");
            }
            Vehicle vehicle = new()
            {
                DriverId = applic.DriverId,
                Mark = applic.Mark,
                Model = applic.Model,
                Year = applic.Year,
                Color = applic.Color,
                CreatedAt = DateTime.UtcNow,
                LicensePlate = applic.LicensePlate,
                Class = applic.Class
            };
            Guid id = _vehicleRepos.Add(vehicle);
            applic.VerificationStatus = Domain.Enums.VehicleVerificationStatus.Approved;
            _vehicleApplicRepos.Update(applic);

            if (await _dbContext.SaveChangesAsync() == 0)
                throw new DataSavingException("Error approving vehicle application");
            return id;
        }

        public async Task Reject(Guid vehicleApplicId, string reason)
        {
            VehicleApplication applic = await _vehicleApplicRepos.GetById(vehicleApplicId);
            if (applic == null)
            {
                throw new NotFoundException("application");
            }
            if(applic.VerificationStatus == Domain.Enums.VehicleVerificationStatus.Approved)
            {
                throw new ConflictException("The application is approved");
            }

            applic.VerificationStatus = Domain.Enums.VehicleVerificationStatus.Rejected;
            applic.RejectReason = reason;
            _vehicleApplicRepos.Update(applic);
            if (await _dbContext.SaveChangesAsync() == 0)
                throw new DataSavingException("Error rejecting vehicle application");
        }
    }
}

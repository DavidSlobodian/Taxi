using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs.InputDTO;
using Taxi.App.DTOs.OutputDTO;
using Taxi.Domain.Entities;
using Taxi.Domain.Enums;
using Taxi.Domain.Exceptions;
using Taxi.Infrastructure;
using Taxi.Infrastructure.Repos;

namespace Taxi.App.Services.DriverService.DriverApplicationService
{
    public class DriverApplicationService : IDriverApplicationService
    {
        private readonly IGenericRepository<Driver> _driverRepos;
        private readonly IGenericRepository<User> _userRepos;
        private readonly IGenericRepository<DriverApplication> _driverApplyRepos;
        private readonly AppDbContext _dbContext;
        public DriverApplicationService(IGenericRepository<Driver> driverRepos, 
            IGenericRepository<User> userRepos,
            IGenericRepository<DriverApplication> driverApplyRepos,
            AppDbContext dbContext)
        {
            _driverRepos = driverRepos;
            _userRepos = userRepos;
            _driverApplyRepos = driverApplyRepos;
            _dbContext = dbContext;
        }
        public async Task<Guid> Apply(Guid userId, DriverApplicationDTO driverInfo)
        {

            Driver driver = await _driverRepos.GetByPredicate(x => x.UserId == userId);
            if (driver != null)
            {
                throw new ConflictException("Driver already exists");
            }

            var existingApplication = await _driverApplyRepos.GetByPredicate(x =>
                x.UserId == userId &&
                (x.Status == DriverApplicationStatus.Pending ||
                x.Status == DriverApplicationStatus.Approved));

            if (existingApplication != null)
            {
                throw new ConflictException("Application already exists");
            }

            DriverApplication newApply = new()
            {
                UserId = userId,
                LicenseExpire = driverInfo.LicenseExpire,
                LicenseNumber = driverInfo.LicenseNumber,
                Status = Domain.Enums.DriverApplicationStatus.Pending,
                HasB = true
            };
            Guid id = _driverApplyRepos.Add(newApply);
            await _dbContext.SaveChangesAsync();
            return id;
        }

        public async Task<DriverApplicOutput> ReviewApplication(Guid applicationId)
        {
            DriverApplication applic = await _driverApplyRepos.GetById(applicationId);
            if (applic == null)
            {
                throw new NotFoundException("Application doesn't exist");
            }
            return new()
            {
                ApplicationId = applicationId,
                LicenseExpire = applic.LicenseExpire,
                LicenseNumber = applic.LicenseNumber,
                HasB = applic.HasB,
                Status = applic.Status,
                RejectionReason = applic.RejectReason
            };
        }

        public async Task<List<DriverApplicOutput>> GetAllApplications()
        {
            List<DriverApplication> applics = (await _driverApplyRepos.GetAll()).ToList();
            List<DriverApplicOutput> results = new();
            foreach(var applic in applics)
            {
                results.Add(new()
                {
                    ApplicationId = applic.Id,
                    LicenseExpire = applic.LicenseExpire,
                    LicenseNumber = applic.LicenseNumber,
                    HasB = applic.HasB,
                    Status = applic.Status,
                    RejectionReason = applic.RejectReason
                });
            }
            return results;
        }

        public async Task<Guid> Approve(Guid applicationId)
        {
            DriverApplication applic = await _driverApplyRepos.GetById(applicationId);
            if (applic == null)
            {
                throw new NotFoundException("application");
            }
            if (applic.Status != DriverApplicationStatus.Pending)
            {
                throw new ConflictException("application is not pending");
            }

            Guid id =  _driverRepos.Add(new Driver
            {
                UserId = applic.UserId,
                LicenseNumber = applic.LicenseNumber,
                Rating = 0,
            });

            applic.Status = DriverApplicationStatus.Approved;
             _driverApplyRepos.Update(applic);

            User user = await _userRepos.GetById(applic.UserId);
            user.ChangeRole(Roles.Driver);
            user.UpdatedAt = DateTime.UtcNow;
             _userRepos.Update(user);

            await _dbContext.SaveChangesAsync();
            return id;
        }

        public async Task Reject(Guid applicationId, string reason)
        {
            DriverApplication applic = await _driverApplyRepos.GetById(applicationId);
            if (applic == null)
            {
                throw new NotFoundException("application");
            }
            if (applic.Status != DriverApplicationStatus.Pending)
            {
                throw new ConflictException("application is not pending");
            }

            applic.Status = DriverApplicationStatus.Rejected;
            applic.RejectReason = reason;
            _driverApplyRepos.Update(applic);
            await _dbContext.SaveChangesAsync();
        }



    }
}

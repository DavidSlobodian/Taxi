using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs.InputDTO;
using Taxi.App.DTOs.OutputDTO;

namespace Taxi.App.Services.DriverService.DriverApplicationService
{
    public interface IDriverApplicationService
    {
        Task<Guid> Apply(Guid userId, DriverApplicationDTO driverInfo);
        //Roles = admin
        Task<DriverApplicOutput> ReviewApplication(Guid applicationId);
        Task<List<DriverApplicOutput>> GetAllApplications();
        Task<Guid> Approve(Guid applicationId);
        Task Reject(Guid applicationId, string reason);
    }
}

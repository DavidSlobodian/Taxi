using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.App.Services.DriverService.DriverStatusService
{
    public interface IDriverStatusService
    {
        public Task<bool> GoOnline(Guid userId);
        public Task<bool> GoOffline(Guid userId);
    }
}

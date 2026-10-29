using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Domain.Enums;

namespace Taxi.Infrastructure.Repos.CancellationRepos
{
    public interface ICancellationRepos
    {
        Task<bool> TryCancelRide(Guid rideId, CancellationReasons reason, Guid? driverId = null);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Domain.Entities;

namespace Taxi.Infrastructure.Repos.RideMatchingRepos
{
    public interface IDriverMatchingRepos
    {
        Task<List<Guid>> GetDueRideIds(DateTime nowUtc, int batchSize = 100);

        Task<Ride?> GetRideSnapshot(Guid rideId);

        Task<bool> TryAdvanceSearchStage(Guid rideId, int expectedRadius, int newRadius, DateTime nextSearchAtUtc);

        Task<bool> TryMarkNoDriver(Guid rideId, DateTime nowUtc);

        Task<bool> TryAcceptRide(Guid userId, Guid rideId);
    }
}

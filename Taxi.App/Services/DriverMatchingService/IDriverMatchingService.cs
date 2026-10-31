using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs.OutputDTO;
using Taxi.Domain.Entities;

namespace Taxi.App.Services.DriverMatchingService
{
    public interface IDriverMatchingService
    {
        Task ProcessPendingMatches();
        Task<List<NearRidesDTO>> GetAllNearRides(Guid userId, double radius=5000);
    }
}

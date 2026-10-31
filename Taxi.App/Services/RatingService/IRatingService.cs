using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs.InputDTO;
using Taxi.App.DTOs.OutputDTO;
using Taxi.App.Models;
using Taxi.Domain.Enums;

namespace Taxi.App.Services.RatingService
{
    public interface IRatingService
    {
        Task<bool> Rate(Guid userId, RatingDTO ratingInfo);
        Task<RatingReview> GetUserReview(Guid userId);
        Task<List<RatingInfoDTO>> GetRatings(Guid userId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs.InputDTO;
using Taxi.App.DTOs.OutputDTO;
using Taxi.App.Models;
using Taxi.Domain.Entities;
using Taxi.Domain.Enums;
using Taxi.Domain.Exceptions;
using Taxi.Infrastructure;
using Taxi.Infrastructure.Repos;

namespace Taxi.App.Services.RatingService
{
    public class RatingService : IRatingService
    {
        private readonly IGenericRepository<User> _userRepos;
        private readonly IGenericRepository<Ride> _rideRepos;
        private readonly IGenericRepository<Driver> _driverRepos;
        private readonly AppDbContext _dbContext;
        private readonly IGenericRepository<Rating> _ratingRepos;
        public RatingService(IGenericRepository<User> userRepos, IGenericRepository<Driver> driverRepos, AppDbContext dbContext, 
            IGenericRepository<Ride> rideRepos, IGenericRepository<Rating> ratingRepos)
        {
            _userRepos = userRepos;
            _driverRepos = driverRepos;
            _dbContext = dbContext;
            _rideRepos = rideRepos;
            _ratingRepos = ratingRepos;
        }
        public async Task<bool> Rate(Guid userId, RatingDTO ratingInfo)
        {
            Ride ride = await _rideRepos.GetById(ratingInfo.RideId);
            if (ride == null)
            {
                throw new NotFoundException("ride");
            }
            if(ride.Status != RideStatus.Completed)
            {
                throw new ConflictException("ride isn't completed");
            }
            User user = await _userRepos.GetById(userId);
            Driver driver = await _driverRepos.GetByPredicate(x => x.Id == ride.DriverId);

            if (ride.PassengerId != userId && driver.UserId != userId)
            {
                throw new ConflictException("This ride doesn't belong to this user");
            }
            if(ratingInfo.Score > 5)
            {
                throw new ValidationException("score is bigger than 5");
            }

            Rating rating = new();
            if (await _ratingRepos.GetByPredicate(x => x.RideId == ratingInfo.RideId && x.FromUserId == userId) != null)
            {
                throw new ConflictException("ride already exists");
            }

            if (user.Role == Roles.Passenger)
            {
                //check tags
                if(ratingInfo.Tags != null && ratingInfo.Tags.Count > 0)
                {
                    if(ratingInfo.Tags.Count(x => (int)x < 10) > 0)
                    {
                        throw new ValidationException("Wrong tags used. You're rating a driver");
                    }
                    if(ratingInfo.Score > 3 && ratingInfo.Tags.Count(x => (int)x >= 16 && (int)x <= 23) > 2)
                    {
                        throw new ValidationException("Too much bad tags with a good score");
                    }
                    else if(ratingInfo.Score < 3 && ratingInfo.Tags.Count(x => (int)x >= 10 && (int)x <= 15) > 2)
                    {
                        throw new ValidationException("Too much good tags with a bad score");
                    }
                }
                rating.FromUserId = userId;
                rating.ToUserId = driver.UserId;
            }
            else if (user.Role == Roles.Driver)
            {
                if (ratingInfo.Tags != null && ratingInfo.Tags.Count > 0)
                {
                    if (ratingInfo.Tags.Count(x => (int)x > 10) > 0)
                    {
                        throw new ValidationException("Wrong tags used. You're rating a passenger");
                    }
                    if (ratingInfo.Score > 3 && ratingInfo.Tags.Count(x => (int)x >= 5 && (int)x <= 9) > 2)
                    {
                        throw new ValidationException("Too much bad tags with a good score");
                    }
                    else if (ratingInfo.Score < 3 && ratingInfo.Tags.Count(x => (int)x >= 0 && (int)x <= 4) > 2)
                    {
                        throw new ValidationException("Too much good tags with a bad score");
                    }
                }
                rating.FromUserId = userId;
                rating.ToUserId = ride.PassengerId;
            }
            rating.RideId = ratingInfo.RideId;
            rating.Tags = ratingInfo.Tags;
            rating.Comment = ratingInfo.Comment;
            rating.Score = ratingInfo.Score;
            
           _ratingRepos.Add(rating);
            return await _dbContext.SaveChangesAsync() != 0;
        }
        public async Task<RatingReview> GetUserReview(Guid userId)
        {
            RatingReview result = new();
            List<Rating> Ratings = await _ratingRepos.GetManyByPredicate(x => x.ToUserId == userId);
            if(Ratings != null && Ratings.Count > 0)
            {
                result.UserId = userId;
                result.AmountOfRatings = Ratings.Count;
                result.AverageRating = Ratings.Sum(x => x.Score) / Ratings.Count;
                result.Tags = result.Tags = Ratings.SelectMany(x => x.Tags).ToList();
            }
            return result;
        }        
        public async Task<List<RatingInfoDTO>> GetRatings(Guid userId)
        {
            List<RatingInfoDTO> result = new();
            List<Rating> Ratings = await _ratingRepos.GetManyByPredicate(x => x.ToUserId == userId);
            if (Ratings != null && Ratings.Count > 0)
            {
                foreach (var rating in Ratings)
                {
                    result.Add(new()
                    {
                        UserId = userId,
                        FromUserId = rating.FromUserId,
                        RideId = rating.RideId,
                        Score = rating.Score,
                        Comment = rating.Comment,
                        Tags = rating.Tags
                    });
                }
            }
            else
                return new();
            return result;
        }
    }
}

using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.Models;
using Taxi.App.Services.LocationService.RideLocationService;
using Taxi.App.Services.PricingService.PriceCalculationService;
using Taxi.Domain.Entities;
using Taxi.Infrastructure.Repos;
using Taxi.Domain.Entities;
using Taxi.Domain.Enums;
using Taxi.Infrastructure.Repos.CancellationRepos;
using Microsoft.AspNetCore.SignalR;
using Taxi.Infrastructure.Repos.RideMatchingRepos;
using System.Runtime.InteropServices;
using Taxi.App.Services.LocationService.DriverLocationService;
using Taxi.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Taxi.App.Services.PaymentService;
using Taxi.App.DTOs.OutputDTO;
using Taxi.App.DTOs.InputDTO;
using Taxi.Domain.Exceptions;

namespace Taxi.App.Services.RideService
{
    public class RideService : IRideService
    {
        private readonly IGenericRepository<User> _userRepos;
        private readonly IGenericRepository<Ride> _rideRepos;
        private readonly IPriceCalculationService _priceCalcService;
        private readonly IRideLocationService _rideLocation;
        private readonly IGenericRepository<Driver> _driverRepos;
        private readonly ICancellationRepos _cancelRepos;
        private readonly IConnectionMultiplexer _redis;
        private readonly IDriverMatchingRepos _matchingRepo;
        private readonly IHubContext<DriverHub> _hub;
        private readonly IDriverLocationService _driverLoc;
        private readonly AppDbContext _dbContext;
        private readonly IPaymentService _paymentService;
        public RideService(IGenericRepository<User> userRepos, IGenericRepository<Ride> rideRepos,
            IPriceCalculationService priceCalcSrvc, IRideLocationService rideLocation, IGenericRepository<Driver> driverRepos,
            ICancellationRepos cancelRepos, IConnectionMultiplexer redis, IDriverMatchingRepos matchingRepo, IHubContext<DriverHub> hub,
            IDriverLocationService driverLoc, AppDbContext dbContext, IPaymentService paymentService)
        {
            _userRepos = userRepos;
            _rideRepos = rideRepos;
            _priceCalcService = priceCalcSrvc;
            _rideLocation = rideLocation;
            _driverRepos = driverRepos;
            _cancelRepos = cancelRepos;
            _redis = redis;
            _matchingRepo = matchingRepo;
            _hub = hub;
            _driverLoc = driverLoc;
            _dbContext = dbContext;
            _paymentService = paymentService;
        }
        public async Task<Guid> CreateRide(Guid userId, RideDTO rideInfo)
        {
            PriceInfo price = await _priceCalcService.CalculatePrice(rideInfo);
            Ride ride = new()
            {
                PassengerId = userId,
                PickupLatitude = rideInfo.PickupLatitude,
                PickupLongtitude = rideInfo.PickupLongtitude,
                DestinationLatitude = rideInfo.DestinationLatitude,
                DestinationLongtitude = rideInfo.DestinationLongtitude,
                Status = Domain.Enums.RideStatus.Searching,
                //Distance = 2,
                //PricePerKm = 50,
                //TotalPrice = 100,
                RideClass = rideInfo.RequestedClass,
                PaymentMethod = rideInfo.PaymentMethod,
                Distance = double.Parse(price.DistanceInKm.ToString("F0")),
                PricePerKm = double.Parse(price.PricePerKm.ToString("F0")),
                TotalPrice = double.Parse(price.TotalPrice.ToString("F0")),
                SearchRadiusMeters = 0,
                NextSearchAtUtc = DateTime.UtcNow
            };
            Guid id = _rideRepos.Add(ride);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex) 
            { throw new DataSavingException("database"); }

            await _rideLocation.AddLocation(id, rideInfo.PickupLongtitude, rideInfo.PickupLatitude);
            return id;
        }
        public async Task<bool> AcceptRide(Guid userId, Guid rideId)
        {
            Driver driver = await _driverRepos.GetByPredicate(x => x.UserId == userId);
            Ride ride = await _rideRepos.GetById(rideId);
            User passenger = await _userRepos.GetById(ride.PassengerId);
            if(passenger.Balance < ride.TotalPrice && ride.PaymentMethod == PaymentMethod.Card)
            {
                throw new ConflictException("The user's balance is too low");
            }
            bool accepted = await _matchingRepo
                .TryAcceptRide(userId, rideId);

            if (!accepted)
                return false;

            var db = _redis.GetDatabase();

            await db.GeoRemoveAsync(
                "rides:locations", rideId.ToString());

            var driverGeo = await _driverLoc.GetLocation(userId);
            await db.GeoAddAsync($"ridelocation:{rideId}", driverGeo.Longitude, driverGeo.Latitude, "driver");
            await db.GeoAddAsync($"ridelocation:{rideId}", ride.PickupLongtitude, ride.PickupLatitude, "pickup");
            await db.GeoAddAsync($"ridelocation:{rideId}", ride.DestinationLongtitude, ride.DestinationLatitude, "destination");

            await db.KeyDeleteAsync(
                $"rides:search-radius:{rideId}");

            await _hub.Clients
                .Group("Drivers")
                .SendAsync("RideUnavailable", rideId);

            await _hub.Clients
                .User($"{userId}")
                .SendAsync("RideAccepted", rideId);

            _dbContext.GeneralInfos.First().TotalRides += 1;

            return true;
        }

        public async Task<bool> DriverArrived(Guid userId, Guid rideId)
        {
            Ride ride = await _rideRepos.GetById(rideId);
            Driver driver = await _driverRepos.GetByPredicate(x => x.UserId == userId);


            if (ride == null) throw new NotFoundException("ride");
            if(ride.DriverId != driver.Id) throw new ConflictException("This ride doesn't belong to this driver");

            if(ride.Status != RideStatus.Accepted)
            {
                throw new ConflictException("ride status is wrong");
            }

            //Geo check
            var db = _redis.GetDatabase();
            var driverLocation = await _driverLoc.GetLocation(userId);
            await db.GeoAddAsync($"ridelocation:{rideId}", driverLocation.Longitude, driverLocation.Latitude, "driver");

            double? distance = await db.GeoDistanceAsync($"ridelocation:{rideId}", "driver", "pickup", GeoUnit.Meters);
            if(distance.Value > 500)
            {
                throw new BadRequestException("You're way too far from pick up point");
            }
            //
            ride.Status = RideStatus.DriverArrived;
            ride.ArrivedAtUtc = DateTime.UtcNow;
            _rideRepos.Update(ride);

            //запуск таймеру безкоштовного очікування
            if (await _dbContext.SaveChangesAsync() == 0)
                throw new DataSavingException("database");

            await _hub.Clients
                .User($"{userId}")
                .SendAsync("DriverArrived", rideId);

            return true;
        }

        public async Task<bool> InProgress(Guid userId, Guid rideId)
        {
            Ride ride = await _rideRepos.GetById(rideId);
            Driver driver = await _driverRepos.GetByPredicate(x => x.UserId == userId);

            if (ride == null) throw new NotFoundException("ride");
            if (ride.DriverId != driver.Id) throw new ConflictException("This ride doesn't belong to this driver");

            if (ride.Status != RideStatus.DriverArrived)
            {
                throw new ConflictException("ride status is wrong");
            }

            ride.Status = RideStatus.InProgress;
            _rideRepos.Update(ride);

            if (await _dbContext.SaveChangesAsync() == 0)
                throw new DataSavingException("database");

            await _hub.Clients
                .User($"{userId}")
                .SendAsync("RideStarted", rideId);

            await _hub.Clients
                .User($"{driver.UserId}")
                .SendAsync("RideStarted", ride.Id);
            return true;
        }

        public async Task<bool> FinishRide(Guid userId, Guid rideId)
        {
            Ride ride = await _rideRepos.GetById(rideId);
            Driver driver = await _driverRepos.GetByPredicate(x => x.UserId == userId);

            if (ride == null) throw new NotFoundException("ride");
            if (ride.DriverId != driver.Id) throw new ConflictException("This ride doesn't belong to this driver");

            if (ride.Status != RideStatus.InProgress)
                throw new ConflictException("ride status is wrong");


            //Geo check
            var db = _redis.GetDatabase();
            var driverLocation = await _driverLoc.GetLocation(userId);
            await db.GeoAddAsync($"ridelocation:{rideId}", driverLocation.Longitude, driverLocation.Latitude, "driver");

            double? distance = await db.GeoDistanceAsync($"ridelocation:{rideId}", "driver", "destination", GeoUnit.Meters);
            if (distance.Value > 500)
            {
                throw new BadRequestException("You're way too far from destination");
            }
            
            ride.Status = RideStatus.Completed;
            ride.FinishedAtUtc = DateTime.UtcNow;
            driver.IsAvailable = true;
            _rideRepos.Update(ride);
            _driverRepos.Update(driver);

            if(await _dbContext.SaveChangesAsync() == 0)
            {
                throw new DataSavingException("database");
            }

            await db.KeyDeleteAsync($"ridelocation:{rideId}");

            await _hub.Clients
                .User($"{userId}")
                .SendAsync("RideCompleted", ride.Id);

            await _hub.Clients
                .User($"{driver.UserId}")
                .SendAsync("RideCompleted", ride.Id);

            //process payment
            if (!await _paymentService.ProcessPayment(rideId))
            {
                throw new DataSavingException("Error processing payment");
                //report to admins
            }
            return true;

        }
        public async Task<CancellationDTO> CancelRide(Guid userId, Guid rideId, CancellationReasons reason)
        {
            Ride ride = await _rideRepos.GetById(rideId);
            if (ride == null)
            {
                throw new NotFoundException("ride");
            }
            User user = await _userRepos.GetById(userId);
            Driver? driver = await _driverRepos.GetByPredicate(x => x.Id == ride.DriverId);
            
            var isCancellationOk = await IsCancellationAvailable(userId, user.Role, ride, reason);
            if(!isCancellationOk.IsOk)
            {
                return isCancellationOk;
            }

            bool cancelled = false;
            if (driver == null)
            {
                if (await _rideLocation.RemoveLocation(rideId))
                {
                    cancelled = await _cancelRepos.TryCancelRide(rideId, reason);
                }
                else return new()
                {
                    RideId = rideId,
                    Initiator = user.Role,
                    IsOk = false,
                    DenyReason = "Unable to remove ride location",
                };
            } 

            else cancelled = await _cancelRepos.TryCancelRide(rideId, reason, driver.Id);

            if (!cancelled)
            {
                return new()
                {
                    RideId = rideId,
                    DenyReason = "Failed transaction commit",
                    Initiator = user.Role,
                    IsOk = false,
                    Reason = reason
                };
            }

            var db = _redis.GetDatabase();
            await db.GeoRemoveAsync($"locations:{rideId}", "driver");
            await db.GeoRemoveAsync($"location:{rideId}", "pickup");
            await db.GeoRemoveAsync($"location:{rideId}", "destination");

            if(driver != null)
            {
                await _hub.Clients//........................................
                .User($"{driver.UserId}")
                .SendAsync("RideCancelled", rideId);
            }
            

            await _hub.Clients
                .User($"{userId}")
                .SendAsync("RideCancelled", rideId);

            return isCancellationOk;
        }

        public async Task<List<RideHistoryDTO>> GetHistoryOfRides(Guid userId)
        {
            User user = await _userRepos.GetById(userId);
            if (user.Role == Roles.Driver)
            {
                var history = await _dbContext.Rides
                    .Where(r => r.Driver.UserId == userId)
                    .Select(r => new RideHistoryDTO
                    {
                        RideId = r.Id,
                        Status = r.Status,
                        PassengerUserId = r.PassengerId,
                        DriverUserId = userId,
                        Role = Roles.Driver,
                        PickupLatitude = r.PickupLatitude,
                        PickupLongitude = r.PickupLongtitude,
                        DestinationLatitude = r.DestinationLatitude,
                        DestinationLongitude = r.DestinationLongtitude,
                        TotalPrice = r.TotalPrice,
                        PaymentMethod = r.PaymentMethod,
                        DistanceInKm = r.Distance,
                        CreatedAt = r.CreatedAt,
                        FinishedAt = r.FinishedAtUtc,
                        RideClass = r.RideClass

                    })
                    .ToListAsync();
                return history;
            }
            else if (user.Role == Roles.Passenger)
            {
                var history = await _dbContext.Rides
                    .Where(r => r.PassengerId == userId)
                    .Select(r => new RideHistoryDTO
                    {
                        RideId = r.Id,
                        Status = r.Status,
                        PassengerUserId = userId,
                        DriverUserId = r.Driver.UserId,
                        Role = Roles.Passenger,
                        PickupLatitude = r.PickupLatitude,
                        PickupLongitude = r.PickupLongtitude,
                        DestinationLatitude = r.DestinationLatitude,
                        DestinationLongitude = r.DestinationLongtitude,
                        TotalPrice = r.TotalPrice,
                        PaymentMethod = r.PaymentMethod,
                        DistanceInKm = r.Distance,
                        CreatedAt = r.CreatedAt,
                        FinishedAt = r.FinishedAtUtc,
                        RideClass = r.RideClass

                    })
                    .ToListAsync();
                return history;
            }
            else
            {
                return new List<RideHistoryDTO>() { };
            }
        }
        private async Task<CancellationDTO> IsCancellationAvailable(Guid userId, Roles initiator, Ride ride, CancellationReasons reason)
        {
            CancellationDTO result = new();
            result.RideId = ride.Id;
            result.Reason = reason;
            result.Initiator = initiator;

            if(ride.Status == RideStatus.Completed || ride.Status == RideStatus.Cancelled || ride.Status == RideStatus.NoDriver)
            {
                result.DenyReason = "Inactive status";
                result.IsOk = false;
                return result;
            }
            if (initiator == Roles.Passenger)
            {
                if(ride.PassengerId != userId)
                {
                    result.DenyReason = "This ride doesn't belong to this passenger";
                    result.IsOk = false;
                    return result;
                }
            }
            else if (initiator == Roles.Driver)
            {
                Driver? driver = await _driverRepos.GetByPredicate(x => x.UserId ==  userId);
                if (driver == null || ride.DriverId != driver.Id)
                {
                    result.DenyReason = "This ride doesn't belong to this driver";
                    result.IsOk = false;
                    return result;
                }
            }
            else
            {
                result.DenyReason = "Wrong role";
                result.IsOk = false;
                return result;
            }


            switch (reason)
            {
                case CancellationReasons.PassengerChangedMind or CancellationReasons.WrongPickupLocation or CancellationReasons.WrongDestination:
                    if (initiator != Roles.Passenger)
                    {
                        result.DenyReason = "Wrong role";
                        result.IsOk = false;
                        return result;
                    }
                    if (ride.Status == RideStatus.Searching || ride.Status == RideStatus.Accepted)
                    {
                        result.IsOk = true;
                        return result;
                    }
                    else if(ride.Status == RideStatus.DriverArrived)
                    {
                        if(ride.ArrivedAtUtc.Value.AddMinutes(2) >= DateTime.UtcNow) // 10:45, 10:48
                        {
                            result.IsOk = true;
                            return result;
                        }
                        else
                        {
                            result.DenyReason = "Time is expired";
                            result.IsOk = false;
                            return result;
                        }
                    }
                    else
                    {
                        result.IsOk = false;
                        result.DenyReason = "Not available for cancellation";
                        return result;
                    }
                    break;
                case CancellationReasons.DriverChangedMind:
                    if(initiator != Roles.Driver)
                    {
                        result.DenyReason = "Wrong role";
                        result.IsOk = false;
                        return result;
                    }    
                    if(ride.Status == RideStatus.Accepted)
                    {
                        if(ride.AcceptedAtUtc.Value.AddMinutes(4) >= DateTime.UtcNow)
                        {
                            result.IsOk = true;
                            return result;
                        }
                        else
                        {
                            result.DenyReason = "The 4 minute perios is expired";
                            result.IsOk = false;
                            return result;
                        }
                    }
                    else
                    {
                        result.IsOk = false;
                        result.DenyReason = "Not available for cancellation";
                        return result;
                    }
                        break;
                case CancellationReasons.PassengerNoShow: // later add 5 additional minutes of awaiting
                    if(initiator != Roles.Driver)
                    {
                        result.DenyReason = "Wrong role";
                        result.IsOk = false;
                        return result;
                    }
                    if(ride.Status == RideStatus.DriverArrived)
                    {
                        if (ride.ArrivedAtUtc.Value.AddMinutes(7) <= DateTime.UtcNow) // 10:50, 11:00
                        {
                            result.IsOk = true;
                            return result;
                        }
                        else
                        {
                            result.DenyReason = "The 7 minutes awaiting period isn't expired yet";
                            result.IsOk = false;
                            return result;
                        }
                    }
                    else
                    {
                        result.DenyReason = "Wrong ride status";
                        result.IsOk = false;
                        return result;
                    }
                        break;
                case CancellationReasons.DangerousPassenger:
                    if(initiator != Roles.Driver)
                    {
                        result.DenyReason = "Wrong role";
                        result.IsOk= false;
                        return result;
                    }
                    if(ride.Status == RideStatus.DriverArrived || ride.Status == RideStatus.InProgress)
                    {
                        result.IsOk = true;
                        return result;
                    }
                    else
                    {
                        result.DenyReason = "Wrong ride status";
                        result.IsOk = false;
                        return result;
                    }
                case CancellationReasons.VehicleIssue or CancellationReasons.Emergency:
                    if(initiator != Roles.Driver)
                    {
                        result.DenyReason = "Wrong role";
                        result.IsOk = false;
                        return result;
                    }    
                    if(ride.Status == RideStatus.Accepted || ride.Status == RideStatus.DriverArrived || ride.Status== RideStatus.InProgress)
                    {
                        result.IsOk = true;
                        return result;
                    }
                    else
                    {
                        result.DenyReason = "Wrong ride status";
                        result.IsOk = false;
                        return result;
                    }
                        break;
                case CancellationReasons.Other:
                    result.IsOk = true;
                    return result;
                    break;
            }
            result.DenyReason = "Something wrong";
            result.IsOk = false;
            return result;
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs;
using Taxi.App.DTOs.InputDTO;
using Taxi.App.DTOs.OutputDTO;
using Taxi.App.Services.LocationService.DriverLocationService;
using Taxi.App.Services.RatingService;
using Taxi.App.Services.RideService;
using Taxi.Domain.Entities;
using Taxi.Domain.Enums;
using Taxi.Domain.Exceptions;
using Taxi.Infrastructure;
using Taxi.Infrastructure.Repos;

namespace Taxi.App.Services.AdminService
{
    public class AdminService : IAdminService
    {
        private readonly IGenericRepository<Ride> _rideRepos;
        private readonly IGenericRepository<User> _userRepos;
        private readonly IGenericRepository<Driver> _driverRepos;
        private readonly IGenericRepository<Complaints> _complaintsRepos;
        private readonly IGenericRepository<GeneralInfo> _generalInfoRepos;
        private readonly IGenericRepository<VehicleApplication> _vehicleApplicRepos;
        private readonly IGenericRepository<DriverApplication> _driverApplicRepos;
        private readonly IRatingService _ratingService;
        private readonly AppDbContext _dbContext;
        private readonly IRideService _rideService;
        private readonly IDriverLocationService _driverLocService;
        public AdminService(IGenericRepository<Ride> rideRepos, IGenericRepository<User> userRepos, IGenericRepository<Driver> driverRepos, 
            IGenericRepository<Complaints> complaintsRepos, IGenericRepository<GeneralInfo> generalInfoRepos, IGenericRepository<VehicleApplication> vehicleApplic,
            IGenericRepository<DriverApplication> driverApplic, AppDbContext dbContext, IRatingService ratingService, IRideService rideService,
            IDriverLocationService driverLocSrvc)
        {
            _complaintsRepos = complaintsRepos;
            _rideRepos = rideRepos;
            _userRepos = userRepos;
            _driverRepos = driverRepos;
            _generalInfoRepos = generalInfoRepos;
            _vehicleApplicRepos = vehicleApplic;
            _driverApplicRepos = driverApplic;
            _dbContext = dbContext;
            _ratingService = ratingService;
            _rideService = rideService;
            _driverLocService = driverLocSrvc;
        }
        public async Task<DashboardDTO> GetDashboard(DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            DashboardDTO dshb = new();
            GeneralInfo genInfo = (await _generalInfoRepos.GetAll()).First();
            dshb.TotalUsers = genInfo.TotalUsers;
            dshb.TotalRides = genInfo.TotalRides;
            dshb.TotalEarnedFromComission = genInfo.TotalEarnedFromComission;
            dshb.TotalActiveRides = (await _rideRepos.GetManyByPredicate(x => x.Status == RideStatus.Accepted || x.Status == RideStatus.DriverArrived || x.Status == RideStatus.InProgress)).Count;
            dshb.TotalActiveVehicleApplications = (await _vehicleApplicRepos.GetManyByPredicate(x => x.VerificationStatus == VehicleVerificationStatus.Pending)).Count;
            dshb.TotalActiveDriverApplications = (await _driverApplicRepos.GetManyByPredicate(x => x.Status == DriverApplicationStatus.Pending)).Count;
            dshb.TotalActiveComplaints = genInfo.TotalComplaints;

            return dshb;
        }
        public async Task<UserInfoDTO> GetUserById(Guid searchedUserId)
        {
            UserInfoDTO userInfo = new();
            User user = _dbContext.Users.AsNoTracking().First(x => x.Id == searchedUserId);
            if (user == null)
            {
                throw new NotFoundException("The user wasn't found");
            }
            RatingInfoDTO ratingInfo = new();
            userInfo.Id = user.Id;
            userInfo.FirstName = user.FirstName;
            userInfo.LastName = user.LastName;
            userInfo.Email = user.Email;
            userInfo.AccountStatus = user.AccountStatus;
            userInfo.RatingInfo = await _ratingService.GetRatings(user.Id);
            userInfo.AvgRating = user.ReceivedRatings.Sum(x => x.Score) / user.ReceivedRatings.Count();
            userInfo.Balance = user.Balance;
            userInfo.Phone = user.Phone;
            userInfo.Role = user.Role;
            return userInfo;
        }
        public async Task<List<UserInfoDTO>> GetUsersByFilter(Guid? searchedUserId = null, string? firstname = null, string? lastname = null, string? phoneNumber = null, string? email = null, Roles? role = null, bool? isBlocked = null)
        {
            var query = _dbContext.Users.AsQueryable();

            // 2. Динамічно додаємо фільтри, якщо параметри передані
            if(searchedUserId.HasValue)
            {
                query = query.Where(u => u.Id == searchedUserId.Value);
            }

            if (!string.IsNullOrWhiteSpace(firstname))
            {
                // Використовуємо Contains для часткового збігу (або == для точного)
                query = query.Where(u => u.FirstName != null && u.FirstName.Contains(firstname));
            }

            if (!string.IsNullOrWhiteSpace(lastname))
            {
                query = query.Where(u => u.LastName != null && u.LastName.Contains(lastname));
            }

            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                query = query.Where(u => u.Phone != null && u.Phone.Contains(phoneNumber));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(u => u.Email != null && u.Email.Contains(email));
            }

            if (role.HasValue)
            {
                query = query.Where(u => u.Role == role.Value);
            }
            if(isBlocked.HasValue)
            {
                query = query.Where(u => u.AccountStatus == (isBlocked.Value ? AccountStatus.Blocked : AccountStatus.Unblocked));
            }
            

            // 3. Виконуємо запит та проєктуємо результати в DTO
            var result = await query
                .Select(u => new UserInfoDTO
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Role = u.Role,
                    AccountStatus = u.AccountStatus,
                    Balance = u.Balance
                })
                .ToListAsync();

            foreach(var res in result)
            {
                var rating = await _ratingService.GetUserReview(res.Id);
                res.AvgRating = rating.AverageRating;
                res.RatingInfo = await _ratingService.GetRatings(res.Id);
                if (res.Role == Roles.Passenger)
                    res.TotalRides = (await _rideRepos.GetManyByPredicate(x => x.PassengerId == res.Id)).Count;
                else if (res.Role == Roles.Driver)
                {
                    Driver driver= await _driverRepos.GetByPredicate(x => x.UserId == res.Id);
                    res.Balance = driver.Balance;
                    res.TotalRides = _dbContext.Rides.Count(x => x.Driver.UserId == res.Id);
                    List<VehicleOutput> vehicles = new();
                    foreach (var vehicle in _dbContext.Vehicles.Where(x => x.DriverId == driver.Id).ToList())
                    {
                        vehicles.Add(new()
                        {
                            Id = vehicle.Id,
                            Class = vehicle.Class,
                            Color = vehicle.Color,
                            LicensePlate = vehicle.LicensePlate,
                            Mark = vehicle.LicensePlate,
                            Model = vehicle.Model,
                            Year = vehicle.Year,
                            VerificationStatus = VehicleVerificationStatus.Approved,
                            RejectionReason = null
                        });
                    }
                    res.Vehicles = vehicles;
                }
            }

            return result;
        }

        public async Task<List<RideHistoryDTO>> GetUserRides(Guid searchedUserId)
        {
            List<RideHistoryDTO> rides = new();
            Driver? driver = await _dbContext.Drivers.FirstAsync(x => x.UserId == searchedUserId);   
            foreach(var ride in await _rideRepos.GetManyByPredicate(x => x.PassengerId == searchedUserId || x.DriverId == driver.Id))
            {
                rides.Add(new()
                {
                    RideId = ride.Id,
                    Status = ride.Status,
                    CreatedAt = ride.CreatedAt,
                    DestinationLatitude = ride.DestinationLatitude,
                    DestinationLongitude = ride.DestinationLongtitude,
                    DistanceInKm = ride.Distance,
                    DriverUserId = driver.UserId,
                    PassengerUserId = ride.PassengerId,
                    FinishedAt = ride.FinishedAtUtc,
                    PaymentMethod = ride.PaymentMethod,
                    PickupLatitude = ride.PickupLatitude,
                    PickupLongitude = ride.PickupLongtitude,
                    RideClass = ride.RideClass,
                    Role = ride.PassengerId == searchedUserId ? Roles.Passenger : Roles.Driver,
                    TotalPrice = ride.TotalPrice,
                });
            }

            return rides;
        }


        public async Task<BlockDTO> BlockUser(Guid adminId, Guid blockUserId)
        {
            User user = await _userRepos.GetById(blockUserId);
            if(user == null)
            {
                throw new NotFoundException("User not found");
            }
            if (user.AccountStatus == AccountStatus.Blocked)
                throw new ForbiddenException("The account is already blocked");
            
            user.AccountStatus = AccountStatus.Blocked;
            user.BlockedById = adminId;
            user.BlockDate = DateTime.UtcNow;
            _userRepos.Update(user);
            await _dbContext.SaveChangesAsync();
            return new BlockDTO()
            {
                UserId = blockUserId,
                Status = AccountStatus.Blocked,
                BlockDate = user.BlockDate.Value,
                BlockedById = user.BlockedById.Value
            };
        }
        public async Task<BlockDTO> UnblockUser(Guid unblockUserId)
        {
            User user = await _userRepos.GetById(unblockUserId);
            if (user == null)
                throw new NotFoundException("User not found");

            user.BlockedById = null;
            user.BlockDate = null;
            user.AccountStatus = AccountStatus.Unblocked;
            _userRepos.Update(user);
            await _dbContext.SaveChangesAsync();
            return new BlockDTO()
            {
                UserId = unblockUserId,
                Status = AccountStatus.Unblocked,
            };
        }

        public async Task<CancellationDTO> CancelRide(Guid rideId)
        {
            Ride ride = await _rideRepos.GetById(rideId);
            if (ride == null)
            {
                throw new NotFoundException("Ride not found");
            }
            return await _rideService.CancelRide(ride.PassengerId, rideId, CancellationReasons.CancelledByAdmin);
        }

        public async Task<ComplaintDTO> ChangeComplaintStatus(Guid userId, Guid complaintId, ComplaintStatus status)
        {
            Complaints complaint = await _complaintsRepos.GetById(complaintId);
            if (complaint == null)
            {
                throw new NotFoundException("Complaint not found");
            }
            complaint.Status = status;
            if(status == ComplaintStatus.Resolved)
            {
                complaint.SolvedBy = userId;
                complaint.SolvedAt = DateTime.UtcNow;
            }
            _complaintsRepos.Update(complaint);
            return new()
            {
                ComplaintId = complaintId,
                SolvedAt = complaint.SolvedAt,
                SolvedBy = complaint.SolvedBy,
                Status = complaint.Status,
                Details = complaint.Details,
                InitiatorId = complaint.InitiatorId,
                InitiatorRole = complaint.InitiatorRole,
                Topic = complaint.Topic
            };
        }

        public async Task ExportAllRides()
        {
            var drivers = await _driverRepos.GetAll();
            var csvBuilder = new StringBuilder();

            foreach (var driver in drivers)
            {
                // Увага: якщо у вас є рядки з комами чи лапками, їх треба екранувати!
                csvBuilder.AppendLine($"Driver Id: {driver.Id}\n" +
                    $"Driver UserId: {driver.UserId}\n" +
                    $"Driver IsOnline: {driver.IsOnline}\n" +
                    $"Driver IsAvailable: {driver.IsAvailable}\n" +
                    $"Driver's balance: {driver.Balance}\n" +
                    $"Driver's active vehicle Id: {driver.ActiveVehicleId}\n");
            }

            // 3. Конвертуємо у байти у кодуванні UTF-8 (з BOM, щоб Excel нормально відкривав українську/російську мову)
            var data = Encoding.UTF8.GetPreamble()
                .Concat(Encoding.UTF8.GetBytes(csvBuilder.ToString()))
                .ToArray();

            // 4. Повертаємо файл клієнту
            await File.WriteAllBytesAsync("export-rides.csv", data);
        }

        public async Task ExportAllUsers()
        {
            var users = await _userRepos.GetAll();
            var csvBuilder = new StringBuilder();

            foreach(var user in users)
            {
                csvBuilder.AppendLine($"Id: {user.Id}\n" +
                    $"First name: {user.FirstName} \n" +
                    $"Last name: {user.LastName}\n" +
                    $"Email: {user.Email} \n" +
                    $"Phone: {user.Phone} \n" +
                    $"Role: {user.Role} \n" +
                    $"Balance: {user.Balance} \n" +
                    $"Account status: {user.AccountStatus == AccountStatus.Blocked}\n");
            }
            var data = Encoding.UTF8.GetPreamble()
                .Concat(Encoding.UTF8.GetBytes(csvBuilder.ToString()))
                .ToArray();

            // 4. Повертаємо файл клієнту
            await File.WriteAllBytesAsync("export-users.csv", data);
        }

        public async Task<List<RideOutput>> GetActiveRides()
        {
            List<Ride> rides = (await _rideRepos.GetManyByPredicate(x => x.Status == RideStatus.Accepted || x.Status == RideStatus.DriverArrived || x.Status == RideStatus.InProgress)).ToList();
            List<RideOutput> res = new();
            foreach (var ride in rides)
            {
                res.Add(new()
                {
                    Id = ride.Id,
                    RequestedClass = ride.RideClass,
                    DestinationLatitude = ride.DestinationLatitude,
                    DestinationLongtitude = ride.DestinationLongtitude,
                    PaymentMethod = ride.PaymentMethod,
                    PickupLatitude = ride.PickupLatitude,
                    PickupLongtitude = ride.PickupLongtitude,
                });
            }
            return res;
        }

        public async Task<(Guid driverUserId, LocationDTO)> GetCurrentDriverLocation(Guid driverUserId)
        {
            var loc = await _driverLocService.GetLocation(driverUserId);
            if (loc == null)
                throw new NotFoundException("Location not found");
            return (driverUserId, loc);
        }

        public async Task<DriverDTO> GetDriverInfo(Guid driverUserId)
        {
            Driver driver = await _driverRepos.GetByPredicate(x => x.UserId == driverUserId);
            var location = await _driverLocService.GetLocation(driverUserId);
            if (driver == null)
                throw new NotFoundException("Driver not found");

            List<Vehicle> vehs = _dbContext.Vehicles.AsNoTracking().Where(x => x.DriverId == driver.Id).ToList();
            List<VehicleOutput> vehicles = new();
            foreach (var vehicle in vehs)
            {
                vehicles.Add(new()
                {
                    Id = vehicle.Id,
                    Class = vehicle.Class,
                    Color = vehicle.Color,
                    LicensePlate = vehicle.LicensePlate,
                    Mark = vehicle.Mark,
                    Model = vehicle.Model,
                    Year = vehicle.Year,
                });
            }
            return new()
            {
                UserId = driverUserId,
                Balance = driver.Balance,
                IsAvailable = driver.IsAvailable,
                IsOnline = driver.IsOnline,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                LicenseNumber = driver.LicenseNumber,
                Rating = driver.Rating,
                Vehicles = vehicles
            };
        }

        public async Task<List<(Guid driverUserId, bool isAvailable)>> GetDriversOnline()
        {
            var drivers = await _driverRepos.GetManyByPredicate(x => x.IsOnline == true);
            return drivers
                .Select(r => (driverUserId: r.UserId, isAvailable: r.IsAvailable))
                .ToList();
        }

        public async Task<List<RideOutput>> GetFinishedRides()
        {
            List<Ride> rides = (await _rideRepos.GetManyByPredicate(x => x.Status == RideStatus.Completed)).ToList();
            List<RideOutput> res = new();
            foreach (var ride in rides)
            {
                res.Add(new()
                {
                    Id = ride.Id,
                    RequestedClass = ride.RideClass,
                    DestinationLatitude = ride.DestinationLatitude,
                    DestinationLongtitude = ride.DestinationLongtitude,
                    PaymentMethod = ride.PaymentMethod,
                    PickupLatitude = ride.PickupLatitude,
                    PickupLongtitude = ride.PickupLongtitude,
                });
            }
            return res;
        }

        public async Task<RideOutput> GetRide(Guid rideId)
        {
            Ride ride = await _rideRepos.GetById(rideId);
            if (ride == null)
            {
                throw new NotFoundException("ride");
            }
            return new()
            {
                Id = ride.Id,
                RequestedClass = ride.RideClass,
                DestinationLatitude = ride.DestinationLatitude,
                DestinationLongtitude = ride.DestinationLongtitude,
                PaymentMethod = ride.PaymentMethod,
                PickupLatitude = ride.PickupLatitude,
                PickupLongtitude = ride.PickupLongtitude,
            };
        }

        
    }
}

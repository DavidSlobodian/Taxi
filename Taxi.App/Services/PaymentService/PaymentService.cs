using Microsoft.AspNetCore.SignalR;
using ProjNet.CoordinateSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Domain.Entities;
using Taxi.Domain.Exceptions;
using Taxi.Infrastructure;
using Taxi.Infrastructure.Repos;

namespace Taxi.App.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IGenericRepository<Ride> _rideRepos;
        private readonly IGenericRepository<Driver> _driverRepos;
        private readonly IGenericRepository<User> _userRepos;
        private readonly AppDbContext _dbContext;
        private readonly IGenericRepository<GeneralInfo> _generalInfoRepos;
        private readonly IHubContext<DriverHub> _hub;
        public PaymentService(IGenericRepository<Ride> rideRepos, IGenericRepository<Driver> driverRepos, AppDbContext dbContext, IGenericRepository<GeneralInfo> generalInfoRepos,
            IGenericRepository<User> userRepos, IHubContext<DriverHub> hub)
        {
            _rideRepos = rideRepos;
            _driverRepos = driverRepos;
            _dbContext = dbContext;
            _generalInfoRepos = generalInfoRepos;
            _userRepos = userRepos;
            _hub = hub;
        }

        public async Task<bool> AddUserBalance(Guid userId, double balance)
        {
            User user = await _userRepos.GetById(userId);
            user.Balance += balance;
            _userRepos.Update(user);
            if (await _dbContext.SaveChangesAsync() == 0)
                throw new DataSavingException("Error saving user balance");
            return true;
        }

        public async Task<bool> ProcessPayment(Guid rideId)
        {
            Ride ride = await _rideRepos.GetById(rideId);
            Driver driver = await _driverRepos.GetByPredicate(x => x.Id == ride.DriverId);
            User user = await _userRepos.GetById(ride.PassengerId);
            double commision = ride.TotalPrice * 0.25;

            if (ride.PaymentMethod == Domain.Enums.PaymentMethod.Card)
            {
                user.Balance -= ride.TotalPrice;
                driver.Balance += ride.TotalPrice - commision; // service comission
                await _hub.Clients.User($"{driver.UserId}").SendAsync("Your balance has increased!");
            }
            else
            {
                driver.Balance -= commision;
                await _hub.Clients.User($"{driver.UserId}").SendAsync("You've been charged a comission!");
            }


            GeneralInfo genInfo = (await _generalInfoRepos.GetAll()).First();
            genInfo.TotalEarnedFromComission += commision;

            _generalInfoRepos.Update(genInfo);
            _rideRepos.Update(ride);
            _driverRepos.Update(driver);
            _userRepos.Update(user);

            if (await _dbContext.SaveChangesAsync() == 0)
                throw new DataSavingException("Error processing payment");
            return true;
        }
    }
}

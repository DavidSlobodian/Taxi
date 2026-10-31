using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.App.Services.PaymentService
{
    public interface IPaymentService
    {
        Task<bool> AddUserBalance(Guid userId, double amount);
        Task<bool> ProcessPayment(Guid rideId); //This function doesn't do anything. Since it's a entertainment-purposed project, the payment is just abstraction.
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Domain.Enums;

namespace Taxi.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FirstName {  get; set; }
        public string LastName { get; set; }
        public string Phone {  get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; private set; }
        public double Balance {  get; set; }
        public AccountStatus AccountStatus { get; set; }
        public Guid? BlockedById { get; set; }
        public DateTime? BlockDate { get; set; }
        public Roles Role { get; private set; }
        public Driver? Driver { get; set; }
        public ICollection<Ride> Rides { get; set; }
        public ICollection<DriverApplication> DriverApplications = new List<DriverApplication>();
        public ICollection<Rating> GivenRatings { get; set; } = new List<Rating>();
        public ICollection<Rating> ReceivedRatings { get; set; } = new List<Rating>();

        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
        }

        public void ChangeRole(Roles role)
        {
            Role = role;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Domain.Entities
{
    public class Driver : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string LicenseNumber { get; set; }
        public double Rating { get; set; }
        public bool IsOnline { get; set; } = false;
        public bool IsAvailable { get; set; } = false;
        public ICollection<Vehicle> Vehicles { get; set; }
        public ICollection<VehicleApplication> VehicleApplications { get; set; }
        public Guid? ActiveVehicleId { get; set; }
        public Vehicle? ActiveVehicle { get; set; }
        public ICollection<Ride> Rides { get; set; }
        public double Balance {  get; set; }
    }
}

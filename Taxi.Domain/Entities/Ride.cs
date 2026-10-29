using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Domain.Enums;

namespace Taxi.Domain.Entities
{
    public class Ride : BaseEntity
    {
        public Guid PassengerId { get; set; }
        public User Passenger { get; set; }
        public Guid? DriverId { get; set; }
        public Driver? Driver { get; set; }
        public Guid? VehicleId {  get; set; }
        public Vehicle? Vehicle;
        public double PickupAltitude {  get; set; }
        public double PickupLongtitude {  get; set; }
        public double DestinationAltitude {  get; set; }
        public double DestinationLongtitude { get; set; }
        public double Distance {  get; set; }
        public double PricePerKm {  get; set; } 
        public double TotalPrice {  get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public RideStatus Status { get; set; }
        public VehicleClass RideClass { get; set; }
        public int SearchRadiusMeters { get; set; } = 0;
        public DateTime? NextSearchAtUtc { get; set; }
        public DateTime? AcceptedAtUtc {  get; set; }
        public DateTime? ArrivedAtUtc { get; set; }
        public DateTime? FinishedAtUtc { get; set; }
        public CancellationReasons? CancellationReason {  get; set; }

    }
}

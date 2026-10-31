using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Domain.Entities;
using Taxi.Domain.Enums;

namespace Taxi.App.DTOs.OutputDTO
{
    public class RideHistoryDTO
    {
        public Guid RideId { get; set; }
        public Guid PassengerUserId {  get; set; }
        public Guid? DriverUserId {  get; set; }
        public Roles Role {  get; set; }
        public double PickupLatitude {  get; set; }
        public double PickupLongitude { get; set; }
        public double DestinationLatitude {  get; set; }
        public double DestinationLongitude { get; set; }
        public double DistanceInKm {  get; set; }
        public double TotalPrice {  get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public RideStatus Status {  get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public VehicleClass RideClass { get; set; }

    }
}

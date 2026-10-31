using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Domain.Enums;

namespace Taxi.App.DTOs.OutputDTO
{
    public class NearRidesDTO
    {
        public Guid RideId { get; set; }
        public double PickupLatitude {  get; set; }
        public double PickupLongitude { get; set; }
        public double DestinationLatitude {  get; set; }
        public double DestinationLongitude { get; set; }
        public double? DistanceToPickUpM {  get; set; }
        public double? RideDistanceKm {  get; set; }
        public double TotalPrice {  get; set; }
        public double PricePerKm {  get; set; }
        public VehicleClass VehicleClass { get; set; }
    }
}

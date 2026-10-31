using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Domain.Enums;

namespace Taxi.App.DTOs.OutputDTO
{
    public class RideOutput
    {
        public Guid Id { get; set; }
        public double PickupLatitude { get; set; }
        public double PickupLongtitude { get; set; }
        public double DestinationLatitude { get; set; }
        public double DestinationLongtitude { get; set; }
        public VehicleClass RequestedClass { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}

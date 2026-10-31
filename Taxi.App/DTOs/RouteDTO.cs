using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.App.DTOs
{
    public class RouteDTO
    {
        public double PickupLatitude {  get; set; }
        public double PickupLongtitude { get; set; }
        public double DestinationLatitude {  get; set; }
        public double DestinationLongtitude { get; set; }
    }
}

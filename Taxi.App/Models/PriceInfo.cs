using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.App.Models
{
    public class PriceInfo
    {
        public double DistanceInKm {  get; set; }
        public double PricePerKm {  get; set; }
        public double TotalPrice {  get; set; }
    }
}

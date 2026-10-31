using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.App.Models
{
    public class NearDriversModel
    {
        public Guid DriverId { get; set; }
        public double? DistanceMeter {  get; set; }
    }
}

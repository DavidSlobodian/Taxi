using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Domain.Enums;

namespace Taxi.Domain.Entities
{
    public class Tariff : BaseEntity
    {
        public VehicleClass Class { get; set; }
        public double BaseFare {  get; set; }
        public double WaitingPricePerMin {  get; set; }
        public double BasePricePerKm {  get; set; }
    }
}

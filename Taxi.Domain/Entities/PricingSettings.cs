using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Domain.Entities
{
    public class PricingSettings : BaseEntity
    {
        public double AverageFuelCons {  get; set; }
        public double AverageFuelPrice {  get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Domain.Entities
{

    public class GeneralInfo : BaseEntity
    {
        public int TotalUsers { get; set; }
        public int TotalRides { get; set; }
        public int TotalComplaints { get; set; }
        public double TotalEarnedFromComission { get; set; }
    }
}

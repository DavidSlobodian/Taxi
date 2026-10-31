using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.App.DTOs.OutputDTO
{
    public class DashboardDTO
    {
        public int TotalUsers {  get; set; }
        public int TotalRides {  get; set; }
        public int TotalActiveRides { get; set; }
        public int TotalActiveDriverApplications {  get; set; }
        public int TotalActiveVehicleApplications {  get; set; }
        public int TotalActiveComplaints {  get; set; }
        public double TotalEarnedFromComission {  get; set; }
    }
}

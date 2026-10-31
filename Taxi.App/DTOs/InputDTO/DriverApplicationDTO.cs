using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Domain.Enums;

namespace Taxi.App.DTOs.InputDTO
{
    public class DriverApplicationDTO
    {
       // public Guid? ApplicationId { get; set; } = null;
        public string LicenseNumber { get; set; }
        public DateTime LicenseExpire {  get; set; }
        public bool HasB {  get; set; }
        //public DriverApplicationStatus Status { get; set; }
    }
}

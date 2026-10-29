using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Domain.Enums;

namespace Taxi.Domain.Entities
{
    public class DriverApplication : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string LicenseNumber {  get; set; }
        public bool HasB {  get; set; }
        public DateTime LicenseExpire {  get; set; }
        public DriverApplicationStatus Status { get; set; }
        public string? RejectReason {  get; set; }
    }
}

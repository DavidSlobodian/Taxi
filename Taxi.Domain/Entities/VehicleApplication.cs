using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Domain.Enums;

namespace Taxi.Domain.Entities
{
    public class VehicleApplication : BaseEntity
    {
        public Guid? VehicleId { get; set; }
        public Guid DriverId { get; set; }
        public Driver Driver { get; set; }
        public string Mark {  get; set; }
        public string Model { get; set; }
        public int Year {  get; set; }
        public string LicensePlate {  get; set; }
        public string Color {  get; set; }
        public VehicleVerificationStatus VerificationStatus { get; set; }
        public VehicleClass Class { get; set; }
        public string? RejectReason {  get; set; }
    }
}

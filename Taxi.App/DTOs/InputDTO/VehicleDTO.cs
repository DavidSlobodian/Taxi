using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Domain.Enums;

namespace Taxi.App.DTOs.InputDTO
{
    public class VehicleDTO
    {
        //public Guid? Id { get; set; } = null;
        public string Mark {  get; set; }
        public string Model {  get; set; }
        public string LicensePlate {  get; set; }
        public string Color {  get; set; }
        public int Year {  get; set; }
        public VehicleClass Class { get; set; }
        //public VehicleVerificationStatus VerificationStatus { get; set; }
    }
}

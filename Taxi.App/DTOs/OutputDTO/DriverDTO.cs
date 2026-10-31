using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs.InputDTO;
using Taxi.Domain.Entities;

namespace Taxi.App.DTOs.OutputDTO
{
    public class DriverDTO
    {
        public Guid UserId { get; set; }
        public string LicenseNumber { get; set; }
        public double Latitude {  get; set; }
        public double Longitude { get; set; }
        public double Rating {  get; set; }
        public bool IsOnline { get; set; }
        public bool IsAvailable { get; set; }
        public List<VehicleOutput> Vehicles { get; set; }
        public double Balance { get; set; }
    }
}

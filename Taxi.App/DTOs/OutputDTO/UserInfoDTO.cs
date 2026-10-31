using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs.InputDTO;
using Taxi.Domain.Entities;
using Taxi.Domain.Enums;

namespace Taxi.App.DTOs.OutputDTO
{
    public class UserInfoDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Roles Role { get; set; }
        public AccountStatus AccountStatus { get; set; }
        public List<VehicleOutput>? Vehicles { get; set; }
        public int TotalRides {  get; set; }
        public double AvgRating {  get; set; }
        public List<RatingInfoDTO> RatingInfo { get; set; }
        public double Balance {  get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Domain.Enums;

namespace Taxi.App.DTOs.OutputDTO
{
    public class RatingInfoDTO
    {
        public Guid UserId { get; set; }
        public Guid FromUserId { get; set; }
        public Guid RideId { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; } = string.Empty;
        public List<RatingTags>? Tags { get; set; }
    }
}

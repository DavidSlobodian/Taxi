using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Domain.Enums;

namespace Taxi.Domain.Entities
{
    public class Rating : BaseEntity
    {
        public Guid RideId { get; set; }
        public Guid FromUserId {  get; set; }
        public Guid ToUserId { get; set; }
        public List<RatingTags>? Tags { get; set; }
        public int Score {  get; set; }
        public string? Comment {  get; set; } = string.Empty;

    }
}

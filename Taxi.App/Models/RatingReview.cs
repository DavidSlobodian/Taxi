using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Domain.Enums;

namespace Taxi.App.Models
{
    public class RatingReview
    {
        public Guid UserId { get; set; }
        public Guid FromUserId { get; set; }
        public double AverageRating {  get; set; }
        public int AmountOfRatings {  get; set; }
        public List<RatingTags>? Tags { get; set; }
    }
}

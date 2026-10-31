using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Domain.Entities;
using Taxi.Domain.Enums;

namespace Taxi.App.DTOs
{
    public class ComplaintDTO
    {
        public Guid ComplaintId { get; set; }
        public Guid InitiatorId { get; set; }
        public Roles InitiatorRole { get; set; }
        public ComplaintTopic Topic { get; set; }
        public string Details {  get; set; }
        public ComplaintStatus Status { get; set; }
        public DateTime? SolvedAt { get; set; }
        public Guid? SolvedBy { get; set; }
    }
}

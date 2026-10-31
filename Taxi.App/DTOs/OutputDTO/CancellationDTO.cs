using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Domain.Enums;
using Taxi.Domain.Entities;

namespace Taxi.App.DTOs.OutputDTO
{
    public class CancellationDTO
    {
        public Guid RideId { get; set; }
        public Roles Initiator {  get; set; }
        public CancellationReasons Reason {  get; set; }
        public bool IsOk { get; set; }
        public string? DenyReason {  get; set; }
        public string? OtherReason {  get; set; }
    }
}

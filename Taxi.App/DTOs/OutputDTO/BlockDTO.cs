using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Domain.Enums;

namespace Taxi.App.DTOs.OutputDTO
{
    public class BlockDTO
    {
        public Guid BlockedById { get; set; }
        public Guid UserId { get; set; }
        public DateTime BlockDate { get; set; }
        public AccountStatus Status {  get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Domain.Exceptions
{
    public sealed class BadRequestException : ApiException
    {
        public BadRequestException(string msg) : base(400, msg)
        {
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Domain.Exceptions
{
    public class ForbiddenException : ApiException
    {
        public ForbiddenException(string msg) : base(403, msg)
        {
            
        }
    }
}

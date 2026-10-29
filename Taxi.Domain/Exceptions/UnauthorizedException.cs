using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Domain.Exceptions
{
    public class UnauthorizedException : ApiException
    {
        public UnauthorizedException(string msg): base(401, msg)
        {
            
        }
    }
}

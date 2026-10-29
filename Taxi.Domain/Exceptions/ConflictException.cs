using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Domain.Exceptions
{
    public class ConflictException : ApiException
    {
        public ConflictException(string msg): base(409, msg)
        {
            
        }
    }
}

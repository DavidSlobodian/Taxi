using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Domain.Exceptions
{
    public class ValidationException : ApiException
    {
        public ValidationException(string msg): base(422, msg)
        {
            
        }
    }
}

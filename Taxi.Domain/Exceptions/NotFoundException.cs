using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Domain.Exceptions
{
    public class NotFoundException : ApiException
    {
        public NotFoundException(string msg) : base(404, msg)
        {
            
        }
    }
}

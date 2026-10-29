using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Domain.Exceptions
{
    public class DataSavingException : ApiException
    {
        public DataSavingException(string msg): base(500, msg)
        {
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Infrastructure.Services.TokenGenerator
{
    public interface ITokenGenerator
    {
        string GenerateToken(Guid id, string email, string role);
    }
}

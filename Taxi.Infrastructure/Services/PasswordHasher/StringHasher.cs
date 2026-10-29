using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Infrastructure.Services.PasswordHasher
{
    public class StringHasher : IStringHasher
    {
        private const string Salt = "gJ9ssco7ChoMHfWDWzDNWA==";

        public string HashString(string source)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: source,
                salt: Convert.FromBase64String(Salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32));
        }
    }
}

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Taxi.Infrastructure.Options;

namespace Taxi.Infrastructure.Services.TokenGenerator
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly AuthOptions _authOptions;

        public TokenGenerator(IOptions<AuthOptions> options)
        {
            _authOptions = options.Value;
        }

        public string GenerateToken(Guid id, string username, string role)
        {
            var identity = GetIdentity(id, username, role);

            var jwt = new JwtSecurityToken(
                        issuer: _authOptions.Issuer,
                        audience: _authOptions.Audience,
                        notBefore: DateTime.UtcNow,
                        claims: identity.Claims,
                        expires: DateTime.UtcNow.Add(
                            TimeSpan.FromSeconds(_authOptions.LifeTimeInSeconds)),
                        signingCredentials:
                        new SigningCredentials(
                            new SymmetricSecurityKey(
                                Encoding.ASCII.GetBytes(_authOptions.Key)),
                            SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private static ClaimsIdentity GetIdentity(Guid id, string username, string role)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                    new Claim(ClaimsIdentity.DefaultNameClaimType, username),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
                };

            ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            return claimsIdentity;
        }
    }
}

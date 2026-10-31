using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs.InputDTO;
using Taxi.App.Validators.UserValidators;
using Taxi.Domain.Entities;
using Taxi.Domain.Exceptions;
using Taxi.Infrastructure;
using Taxi.Infrastructure.Repos;
using Taxi.Infrastructure.Services.PasswordHasher;
using Taxi.Infrastructure.Services.TokenGenerator;

namespace Taxi.App.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<User> _userRepos;
        private readonly IStringHasher _stringHasher;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly AppDbContext _dbContext;
        private readonly IGenericRepository<GeneralInfo> _generalInfoRepos;
        public AuthService(IGenericRepository<User> userRepos, ITokenGenerator tokenGenerator, IStringHasher stringHasher, 
            AppDbContext dbContext, IGenericRepository<GeneralInfo> generalInfoRepos)
        {
            _userRepos = userRepos;
            _stringHasher = stringHasher;
            _tokenGenerator = tokenGenerator;
            _dbContext = dbContext;
            _generalInfoRepos = generalInfoRepos;
        }
        public async Task<Guid> SignUp(RegistrationDto regDto)
        {
            if (await _userRepos.GetByPredicate(x => x.Phone == regDto.PhoneNumber || x.Email == regDto.Email) != null)
            {
                Console.WriteLine($"{regDto.Email}, {regDto.PhoneNumber}");
                throw new ConflictException("user already exists");
            }

            string hashed = _stringHasher.HashString(regDto.Password);
            
            User newUser = new User
            {
                FirstName = regDto.FirstName,
                LastName = regDto.LastName,
                Email = regDto.Email,
                Phone = regDto.PhoneNumber
            };
            newUser.ChangePassword(hashed);
            newUser.ChangeRole(Roles.Passenger);

            var response = _userRepos.Add(newUser);

            GeneralInfo info = (await _generalInfoRepos.GetAll()).First();
            info.TotalUsers += 1;
            _generalInfoRepos.Update(info);

            await _dbContext.SaveChangesAsync();
            return response;
        }
        public async Task<string> SignIn(LoginDto login)
        {
            var user = await _userRepos.GetByPredicate(x => x.Email == login.Email);
            if (user == null)
            {
                throw new NotFoundException("User's login is invalid");
            }

            var hashed = _stringHasher.HashString(login.Password);

            if (user.PasswordHash != hashed)
            {
                throw new UnauthorizedException("Wrong password");
            }

            var role = user.Role;
            return _tokenGenerator.GenerateToken(user.Id, user.Email, role.ToString());
        }

        public Task<bool> ChangePassword(string email, string oldPass, string newPass)
        {
            throw new NotImplementedException();
        }
    }
}

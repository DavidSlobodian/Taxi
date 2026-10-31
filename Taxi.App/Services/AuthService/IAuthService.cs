using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs.InputDTO;

namespace Taxi.App.Services.AuthService
{
    public interface IAuthService
    {
        Task<string> SignIn(LoginDto login);
        Task<Guid> SignUp(RegistrationDto userDto);
        Task<bool> ChangePassword(string email, string oldPass, string newPass);
    }
}

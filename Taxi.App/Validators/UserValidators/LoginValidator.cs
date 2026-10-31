using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs.InputDTO;

namespace Taxi.App.Validators.UserValidators
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(login => login.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Invalid email format");

            RuleFor(login => login.Password)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(30)
                .WithMessage("Invalid password format");
        }
    }
}

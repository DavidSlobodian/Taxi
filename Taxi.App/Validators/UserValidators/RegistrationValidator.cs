using FluentValidation;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs.InputDTO;

namespace Taxi.App.Validators.UserValidators
{
    public class RegistrationValidator : AbstractValidator<RegistrationDto>
    {
        public RegistrationValidator() 
        {
            RuleFor(user => user.FirstName)
                .NotEmpty()
                .WithMessage("First name is empty");

            RuleFor(user => user.LastName)
                .NotEmpty()
                .WithMessage("Last name is empty");

            RuleFor(user => user.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Invalid email format");

            RuleFor(user => user.Password)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(30)
                .WithMessage("Invalid password format");

            RuleFor(user => user.PhoneNumber)
                .NotEmpty()
                .Must(IsValidPhoneNumber)
                .WithMessage("Invalid phone number format");

        }
        private static bool IsValidPhoneNumber(string phone)
        {
            try
            {
                var phoneUtil = PhoneNumberUtil.GetInstance();
                var number = phoneUtil.Parse(phone, "UA");

                return phoneUtil.IsValidNumber(number);
            }
            catch (NumberParseException)
            {
                return false;
            }
        }
    }
}

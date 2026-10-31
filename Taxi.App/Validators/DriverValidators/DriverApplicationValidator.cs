using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs.InputDTO;

namespace Taxi.App.Validators.DriverValidators
{
    public class DriverApplicationValidator : AbstractValidator<DriverApplicationDTO>
    {
        public DriverApplicationValidator()
        {
            RuleFor(driverApp => driverApp.HasB)
                .Equal(true)
                .WithMessage("No B category");

            RuleFor(driverapp => driverapp.LicenseNumber)
                .MinimumLength(9)
                .MaximumLength(9)
                .WithMessage("Length is over format");

            RuleFor(driverapp => driverapp.LicenseExpire)
                .NotEmpty()
                .GreaterThan(DateTime.Now)
                .WithMessage("The license is expired");
        }
    }
}

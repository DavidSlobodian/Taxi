using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.App.DTOs.InputDTO;
using Taxi.Domain.Entities;

namespace Taxi.App.Validators.VehicleValidators
{
    public class VehicleValidator : AbstractValidator<VehicleDTO>
    {
        public VehicleValidator()
        {
            RuleFor(x => x.Model)
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(30)
                .WithMessage("The model is invalid");

            RuleFor(x => x.Mark)
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(30)
                .WithMessage("The mark is invalid");

            RuleFor(x => x.Year)
                .GreaterThan(2010)
                .LessThan(DateTime.Now.Year)
                .WithMessage("The car is too old");

            RuleFor(x => x.LicensePlate)
                .NotEmpty()
                .Length(8)
                .Matches(@"^[ABCDEHIKMOPTXabcdehikmoptx]{2}\d{4}[ABCDEGHIKMOPTXYZabcdeghikmoptxyz]{2}$")
                .WithMessage("Wrong license plate format");

            RuleFor(x => x.Color)
                .NotEmpty()
                .MaximumLength(30)
                .WithMessage("Wrong color format");
        }
    }
}

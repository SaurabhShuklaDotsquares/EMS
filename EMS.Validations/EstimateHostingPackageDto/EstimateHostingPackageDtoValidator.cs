using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Validations
{
    public class EstimateHostingPackageDtoValidator : AbstractValidator<EstimateHostingPackageDto>
    {
        public EstimateHostingPackageDtoValidator()
        {
            RuleFor(l => l.CountryId).NotNull().WithMessage("The Country field is required");
            RuleFor(l => l.Name).NotNull().WithMessage("The Name field is required");
            RuleFor(l => l.PackageDetail).NotNull().WithMessage("The Package Detail field is required");
        }
    }
}

using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Validations
{
   public class EstimateFormDtoValidator : AbstractValidator<EstimateFormDto>
    {
        public EstimateFormDtoValidator()
        {
              RuleFor(l => l.RoleId).NotNull().WithMessage("Role is required.");
              RuleFor(l => l.ExperienceId).Null().WithMessage("Experience is required.").LessThan(1).WithMessage("Experience is required.");
              RuleFor(l => l.NoOfResources).NotNull().WithMessage("No Of Resources is required.");

        }
    }
}

using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EMS.Validations
{
    public class ForecastingDtoValidator : AbstractValidator<ManageForecastingDto>
    {
        public ForecastingDtoValidator()
        {
            RuleFor(l => l.ForecastingType).NotNull().WithMessage("*required");
            RuleFor(l => l.ProjectDescription).NotNull().WithMessage("*required");
            RuleFor(l => l.Country).NotNull().WithMessage("*required");
            RuleFor(l => l.TentiveDate).NotNull().WithMessage("*required");
            RuleFor(l => l.SelectedDepartment).NotNull().WithMessage("*required");
        }
    }
}

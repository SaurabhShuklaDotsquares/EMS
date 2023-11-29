using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Validations
{
    public class BugReportDtoValidator : AbstractValidator<BugReportDto>
    {
        public BugReportDtoValidator()
        {
            RuleFor(l => l.SectionName).NotNull().WithMessage("*required");
            RuleFor(l => l.SectionDescription).NotNull().WithMessage("*required");
        }
    }
}

using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace EMS.Validations
{
    public class WFHActivityDtoValidator : AbstractValidator<WFHActivityDto>
    {
        public WFHActivityDtoValidator()
        {
            RuleFor(l => l.Uid).NotNull().WithMessage("*required");
            RuleFor(l => l.PMid).NotNull().WithMessage("*required");
            RuleFor(l => l.StartDate).NotNull().WithMessage("*required");
            RuleFor(l => l.EndDate).NotNull().WithMessage("*required");          
            RuleFor(l => l.WFHCategory).NotEmpty().WithMessage("*required");
            RuleFor(l => l.HalfValue).NotEmpty().WithMessage("*required");
            RuleFor(l => l.IsHalf).NotEmpty().WithMessage("*required");
            RuleFor(l => l.Comment).NotEmpty().WithMessage("*required");
        }
    }
}

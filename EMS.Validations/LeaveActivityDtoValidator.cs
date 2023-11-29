using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Validations
{
    public class LeaveActivityDtoValidator : AbstractValidator<LeaveActivityDto>
    {

        public LeaveActivityDtoValidator()
        {
            RuleFor(l => l.Uid).NotNull().WithMessage("*required");
            RuleFor(l => l.PMid).NotNull().WithMessage("*required");
            RuleFor(l => l.StartDate).NotNull().WithMessage("*required");
            RuleFor(l => l.EndDate).NotNull().WithMessage("*required");
            RuleFor(l => l.Reason).NotEmpty().WithMessage("*required");
            RuleFor(l => l.WorkAlternator).NotEmpty().WithMessage("*required");
            RuleFor(l => l.WorkAlterID).NotEmpty().WithMessage("*required");
            RuleFor(l => l.LeaveType).NotEmpty().WithMessage("*required");
            RuleFor(l => l.LeaveCategory).NotEmpty().WithMessage("*required");
            RuleFor(l => l.HalfValue).NotEmpty().WithMessage("*required");
        }
    }
}

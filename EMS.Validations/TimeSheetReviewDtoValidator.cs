using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Validations
{
    public class TimeSheetReviewDtoValidator : AbstractValidator<TimeSheetReviewDto>
    {
        public TimeSheetReviewDtoValidator()
        {
            RuleFor(l => l.AddedDate).NotEmpty().WithMessage("*required");
            RuleFor(l => l.ProjectId).NotEmpty().WithMessage("*required");
            RuleFor(l => l.DeveloperId).NotEmpty().WithMessage("*required");
        }
    }
}

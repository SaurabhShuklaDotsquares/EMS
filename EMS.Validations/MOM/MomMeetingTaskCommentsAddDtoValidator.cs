using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Validations
{
    public class MomMeetingTaskCommentsAddDtoValidator : AbstractValidator<MomMeetingTaskCommentsAddDto>
    {
        public MomMeetingTaskCommentsAddDtoValidator()
        {

            RuleFor(m => m.Comment).NotEmpty().WithMessage("*Comment required");
            RuleFor(m => m.Paticipants).NotEmpty().WithMessage("Atleast one participant need to be there in Assign To");
            RuleFor(m => m.Status).NotNull().WithMessage("*Status required");
            //RuleFor(m => m.TargetDates).NotEmpty().WithMessage("*required");
            RuleFor(m => m.Priority).NotEmpty().WithMessage("*Priority required");
            // RuleFor(m => m.MomMeetingTaskTimeLineDto.Status).NotEmpty().WithMessage("*Status required");

        }
    }
}

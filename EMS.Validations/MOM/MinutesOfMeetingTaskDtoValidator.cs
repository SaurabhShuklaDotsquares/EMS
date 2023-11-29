using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Validations
{
    public class MomMeetingTaskDtoValidator : AbstractValidator<MomMeetingTaskDto>
    {
        public MomMeetingTaskDtoValidator()
        {
            RuleFor(m => m.Task).NotEmpty().WithMessage("*Action required");         
            RuleFor(m => m.Paticipants).NotNull().WithMessage("*Participant required");
            RuleFor(m => m.Priority).NotNull().NotEmpty().WithMessage("*Priority required");
            RuleFor(m => m.TargetDates).NotEmpty().WithMessage("*Target Date required");
        }
    }
}

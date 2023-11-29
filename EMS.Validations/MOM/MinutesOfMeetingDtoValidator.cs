using FluentValidation;
using EMS.Dto;

namespace EMS.Validations
{
    public class MomMeetingDtoValidator : AbstractValidator<MomMeetingDto>
    {
        public MomMeetingDtoValidator()
        {
            RuleFor(m => m.MeetingTitle).NotEmpty().WithMessage("*required");
            RuleFor(m => m.VenueName).NotEmpty().WithMessage("*required");
            RuleFor(m => m.DateOfMeetings).NotEmpty().WithMessage("*required");
            RuleFor(m => m.MeetingTime).NotEmpty().WithMessage("*required").GreaterThan(0).WithMessage("* Time must be greater than '0' ");
            RuleFor(m => m.Agenda).NotEmpty().WithMessage("*Meeting Agenda is required");
            RuleFor(m => m.ChairedByUID).NotEmpty().WithMessage("*required");
            RuleFor(m => m.MeetingStartTime).NotEmpty().WithMessage("*required");
            // RuleFor(m => m.Paticipants).NotEmpty().WithMessage("*required");
            // RuleFor(m => m.Groups).NotEmpty().WithMessage("*required");
        }
    }
}

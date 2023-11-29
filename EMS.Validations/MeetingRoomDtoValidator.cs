using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Validations
{
    public class MeetingRoomDtoValidator : AbstractValidator<AddMeetingRoomDto>
    {
        public MeetingRoomDtoValidator()
        {
            RuleFor(l => l.CompanyOfficeId).NotNull().WithMessage("*required");
            RuleFor(l => l.Name).NotNull().WithMessage("*required");
            RuleFor(l => l.ThemeColor).NotNull().WithMessage("*required");
        }
    }
}

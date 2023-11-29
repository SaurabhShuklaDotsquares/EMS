using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Validations
{
    
    public class ReminderDtoValidator : AbstractValidator<ReminderDto>
    {
        public ReminderDtoValidator()
        {
            RuleFor(A => A.Title).NotNull().WithMessage("*required");
            RuleFor(A => A.ReminderDate).NotNull().WithMessage("*required");
        }
    }
}

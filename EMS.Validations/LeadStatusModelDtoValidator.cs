using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using EMS.Dto;

namespace EMS.Validations
{
   public class LeadStatusModelDtoValidator: AbstractValidator<LeadStatusModelDto>
    {
        public LeadStatusModelDtoValidator()
        {
            RuleFor(l => l.StatusName).NotNull().WithMessage("*required");
            RuleFor(l => l.FromEmail).EmailAddress().WithMessage("*A valid email is required");
            RuleFor(l => l.To).EmailAddress().WithMessage("*A valid email is required");
            RuleFor(l => l.CC).EmailAddress().WithMessage("*A valid email is required");
            RuleFor(l => l.BCC).EmailAddress().WithMessage("*A valid email is required");
        }
    }
}

using EMS.Data;
using EMS.Dto;
using EMS.Validations;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace EMS.Validations
{
    public class EscalationDtoValidator: AbstractValidator<EscalationDto>
    {
        public EscalationDtoValidator()
        {
            //RuleFor(l => l.RootCause).NotNull().WithMessage("*Root cause is required.");
            //RuleFor(l => l.EscalationDesctiption).NotNull().WithMessage("Description is required.").
            //    MaximumLength(5000).WithMessage("Description can be max 5000 characters long");
           
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
   public class VirtualDeveloperDtoValidator : AbstractValidator<VirtualDeveloperDto>
    {
        public VirtualDeveloperDtoValidator()
        {
            RuleFor(l => l.Email).NotNull().WithMessage("*Required");
            RuleFor(l => l.Email).EmailAddress().WithMessage("Invalid email");
            RuleFor(l => l.VirtualDeveloper_Name).NotNull().WithMessage("*Required");
            RuleFor(l => l.PMUid).NotNull().WithMessage("&nbsp;");
        }
    }
}

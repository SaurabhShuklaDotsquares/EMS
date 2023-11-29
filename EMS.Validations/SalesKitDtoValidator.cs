using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Dto.LibraryManagement;
using FluentValidation;

namespace EMS.Validations
{
   public class SalesKitDtoValidator : AbstractValidator<SalesKitTypeDto>
    {
        public SalesKitDtoValidator()
        {
            RuleFor(l => l.Name).NotNull().WithMessage("*Required");            
            RuleFor(l => l.DisplayName).NotNull().WithMessage("*Required");            
            RuleFor(l => l.ParentId).NotEmpty().WithMessage("*Required");            
        }
    }
}

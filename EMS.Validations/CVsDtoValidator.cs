using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Dto.LibraryManagement;
using FluentValidation;

namespace EMS.Validations
{
   public class CVsDtoValidator : AbstractValidator<CvsTypeDto>
    {
        public CVsDtoValidator()
        {
            RuleFor(l => l.Name).NotNull().WithMessage("*Required");
        }
    }
}

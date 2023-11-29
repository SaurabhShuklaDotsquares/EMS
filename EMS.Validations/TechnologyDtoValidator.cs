using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
   public class TechnologyDtoValidator : AbstractValidator<TechnologyDto>
    {
        public TechnologyDtoValidator()
        {
            RuleFor(l => l.Title).NotNull().WithMessage("*required");
        }
    }
}

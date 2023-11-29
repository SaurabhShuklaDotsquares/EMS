using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Validations
{
   public class ComponentDtoValidator : AbstractValidator<ComponentDto>
    {
        public ComponentDtoValidator()
        {
            RuleFor(l => l.Title).NotNull().WithMessage("*Required");
            RuleFor(l => l.Type).NotNull().WithMessage("*Required");
            RuleFor(l => l.ComponentCategoryId).NotNull().WithMessage("*Required");       
        }
    }
}

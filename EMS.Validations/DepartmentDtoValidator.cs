using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Dto;
using FluentValidation;
namespace EMS.Validations
{
    public class DepartmentDtoValidator : AbstractValidator<DepartmentDto>
    {
        public DepartmentDtoValidator()
        {
            RuleFor(l => l.Name).NotNull().WithMessage("*Required");
            RuleFor(l => l.Deptcode).NotNull().WithMessage("*Required");
        }

    }
}

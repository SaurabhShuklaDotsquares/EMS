using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
    public class AppraiseDtoValidator : AbstractValidator<AppraiseDto>
    {
        public AppraiseDtoValidator()
        {
            RuleFor(A => A.EmployeeId).NotNull().WithMessage("*required");
            RuleFor(A => A.ClientComment).NotNull().When(x => x.AppraiseId == 2).WithMessage("*required");
            RuleFor(A => A.TlComment).NotNull().When(x => x.AppraiseId == 1).WithMessage("*required");
        }
    }
}

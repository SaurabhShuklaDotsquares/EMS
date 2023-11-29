using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
    public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordDtoValidator()
        {
            RuleFor(l => l.Password).NotNull().WithMessage("*required");
            RuleFor(l => l.NewPassword).NotNull().WithMessage("*required");
            RuleFor(l => l.ConfirmPassword).Equal(l => l.NewPassword).WithMessage("Password doesn't match");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Dto;
using FluentValidation;
namespace EMS.Validations
{
    public class ChangeUserPasswordDtoValidator : AbstractValidator<ChangeUserPasswordDto>
    {
        public ChangeUserPasswordDtoValidator()
        {
            
            RuleFor(l => l.NewPassword).NotNull().WithMessage("*required");
            RuleFor(l => l.ConfirmPassword).Equal(l => l.NewPassword).WithMessage("Password doesn't match");
        }
    }
}

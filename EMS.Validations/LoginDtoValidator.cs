using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(l => l.Email).NotNull().WithMessage("*required");
            RuleFor(l => l.Password).NotNull().WithMessage("*required");
            //RuleFor(l => l.Email).EmailAddress().WithMessage("Enter valid email.");
        }
    }
}
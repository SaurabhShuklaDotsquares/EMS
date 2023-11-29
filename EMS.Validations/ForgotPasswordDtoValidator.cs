using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
    public class ForgotPasswordDtoValidator : AbstractValidator<ForgotPasswordDto>
    {
        public ForgotPasswordDtoValidator()
        {
            RuleFor(l => l.Email).NotNull().WithMessage("*required");            
            RuleFor(l => l.Email).EmailAddress().WithMessage("Enter valid email.");
        }
    }
}
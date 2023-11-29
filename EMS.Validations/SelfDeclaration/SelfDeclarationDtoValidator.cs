
using EMS.Dto;
using FluentValidation;
namespace EMS.Validations
{
    public class SelfDeclarationDtoValidator : AbstractValidator<SelfDeclarationDto>
    {
        public SelfDeclarationDtoValidator()
        {
            RuleFor(l => l.Dob).NotNull().WithMessage("*DOB is required");
            RuleFor(l => l.MobileNumber).NotNull().WithMessage("*Mobile No. is required");
            RuleFor(l => l.Address).NotNull().WithMessage("*Address is required");
            RuleFor(l => l.Location).NotNull().WithMessage("*Location is required");
            RuleFor(l => l.DeclarationName).NotNull().WithMessage("*Declaration Name is required");
            RuleFor(l => l.EmailPersonal).EmailAddress().NotNull().WithMessage("*Personal email is required");
        }
    }
}

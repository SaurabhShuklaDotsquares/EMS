 
using EMS.Dto;
using FluentValidation;
namespace EMS.Validations
{
  public  class UserProfileDtoValidator : AbstractValidator<UserProfileDto>
    {
         public UserProfileDtoValidator()
        {
            RuleFor(l => l.EmailOffice).NotNull().WithMessage("*required").EmailAddress().WithMessage("*Office email format not valid.");
            RuleFor(l => l.Name).NotNull().WithMessage("*required");
            RuleFor(l => l.DOB).NotNull().WithMessage("*required");
            RuleFor(l => l.JoinedDate).NotNull().WithMessage("*required");
            RuleFor(l => l.AadharNumber).Length(0,20).WithMessage("Aadhar Number length must be less than 20 digits.");
            RuleFor(l => l.PanNumber).Length(0, 20).WithMessage("PAN Number length must be less than 20 characters.");
            RuleFor(l => l.PassportNumber).Length(0, 20).WithMessage("Passport Number length must be less than 20 characters.");
        }
    }
}


using EMS.Dto;
using FluentValidation;
namespace EMS.Validations
{
    public class JobReferFriendDtoValidator : AbstractValidator<JobReferFriendDto>
    {
        public JobReferFriendDtoValidator()
        {
            RuleFor(l => l.Name).NotNull().WithMessage("*required");
            RuleFor(l => l.Email).NotNull().WithMessage("*required");
            RuleFor(l => l.MobileNo).NotNull().WithMessage("*required").NotEmpty()
            .Matches(@"^(\+91[\-\s]?)?[0]?(91)?[789]\d{9}$").WithMessage("Not a valid number");
        }
    }
}

using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
    public class BugStatusDtoValidator : AbstractValidator<BugStatusDto>
    {
        public BugStatusDtoValidator()
        {
            RuleFor(S => S.StatusId).NotNull().WithMessage("*required");
            RuleFor(S => S.Comment).NotNull().WithMessage("*required");
        }
    }
}

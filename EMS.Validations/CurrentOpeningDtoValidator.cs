
using EMS.Dto;
using FluentValidation;
namespace EMS.Validations
{
    public class CurrentOpeningDtoValidator : AbstractValidator<CurrentOpeningDto>
    {
        public CurrentOpeningDtoValidator()
        {
            RuleFor(l => l.Post).NotNull().WithMessage("*required");
            RuleFor(l => l.Technology).NotNull().WithMessage("*required");
            RuleFor(l => l.Min_Experience).NotNull().WithMessage("*required");
            RuleFor(l => l.Small_Description).NotNull().WithMessage("*required");
            RuleFor(l => l.DepartmentId).NotNull().WithMessage("*required");
        }
    }
}

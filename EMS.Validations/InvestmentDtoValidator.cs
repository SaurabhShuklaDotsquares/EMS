using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
    public class InvestmentDtoValidator : AbstractValidator<InvestmentDto>
    {
        public InvestmentDtoValidator()
        {
            RuleFor(m => m.Name).NotEmpty().WithMessage("*required");
            RuleFor(m => m.FatherName).NotEmpty().WithMessage("*required");
            RuleFor(m => m.AttendanceCode).MaximumLength(20).WithMessage("max length is 20");
            RuleFor(m => m.DOB).NotEmpty().WithMessage("*required");
            RuleFor(m => m.HomeAddress).NotEmpty().WithMessage("*required");
            RuleFor(m => m.PAN).NotEmpty().WithMessage("*required")
                .MaximumLength(10).WithMessage("max length is 10")
                .MinimumLength(10).WithMessage("min length is 10");
        }
    }
}

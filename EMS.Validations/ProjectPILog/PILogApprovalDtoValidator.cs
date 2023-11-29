using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
    public class PILogApprovalDtoValidator : AbstractValidator<PILogApprovalDto>
    {
        public PILogApprovalDtoValidator()
        {
            RuleFor(m => m.Id).NotEmpty().WithMessage("PI Log Id is required");
            RuleFor(m => m.Status).NotEmpty().WithMessage("Status is required");
        }
    }
}

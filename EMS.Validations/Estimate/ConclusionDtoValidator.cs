using FluentValidation;
using EMS.Dto;


namespace EMS.Validations
{
    public class ConclusionDtoValidator : AbstractValidator<ConclusionDto>
    {
        public ConclusionDtoValidator()
        {
            RuleFor(m => m.LeadId).NotEmpty().WithMessage("Lead Id must be greater than 0");
            RuleFor(m => m.Status).NotEmpty().WithMessage("Status is required");
            RuleFor(m => m.StatusUpdateDate).NotEmpty().WithMessage("Update Date is required");
        }
    }
}


using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
    public class ProjectNCLogAuditeeDtoValidator : AbstractValidator<ProjectNCLogAuditeeDto>
    {
        public ProjectNCLogAuditeeDtoValidator()
        {
            RuleFor(m => m.Id).NotEmpty().WithMessage("NC Log Id is required");
            RuleFor(m => m.Status).NotEmpty().WithMessage("Audit Status is required");
            RuleFor(m => m.AuditAction).NotEmpty().WithMessage("Audit Action is required");
            RuleFor(m => m.RootCause).NotEmpty().WithMessage("Audit Root Cause is required");
        }
    }
}

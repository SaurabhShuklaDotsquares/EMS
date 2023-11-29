using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
    public class ProjectNCLogDtoValidator : AbstractValidator<ProjectNCLogDto>
    {
        public ProjectNCLogDtoValidator()
        {
            RuleFor(m => m.AuditCycle).NotEmpty().WithMessage("Audit Cycle is required");
            RuleFor(m => m.AuditType).NotEmpty().WithMessage("NC Type is required");
            RuleFor(m => m.ProjectId).NotEmpty().WithMessage("Project is required");
            RuleFor(m => m.ProjectAuditPAId).NotEmpty().WithMessage("Audit Process Area is required");
            RuleFor(m => m.AuditDesc).NotEmpty().WithMessage("Description of NC is required");
            RuleFor(m => m.AuditeeUid).NotEmpty().WithMessage("Auditee is required");
        }
    }
}

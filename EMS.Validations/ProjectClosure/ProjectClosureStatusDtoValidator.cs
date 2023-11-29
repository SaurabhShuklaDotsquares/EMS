using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
    public class ProjectClosureStatusDtoValidator : AbstractValidator<ProjectClosureStatusDto>
    {
        public ProjectClosureStatusDtoValidator()
        {
            RuleFor(x => x.ProjectClosureId).NotEmpty().WithMessage("Project Closure Id is required");
            RuleFor(x => x.ChangeStatusId).NotEmpty().WithMessage("Change Status is required");
            RuleFor(x => x.DeadResponseDate).NotEmpty().WithMessage("Dead Response Date is required");
            RuleFor(x => x.Reason).NotEmpty().WithMessage("Reason is required");
        }
    }
}

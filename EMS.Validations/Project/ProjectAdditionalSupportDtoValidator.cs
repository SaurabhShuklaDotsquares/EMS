using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
    public class ProjectAdditionalSupportDtoValidator : AbstractValidator<ProjectAdditionalSupportDto>
    {
        public ProjectAdditionalSupportDtoValidator()
        {
            RuleFor(l => l.ProjectId).NotEmpty().WithMessage("Project id is required");
           // RuleFor(l => l.AddDescription).NotEmpty().When(x => x.Id == 0 && x.FromProjectStatus == false).WithMessage("Description is required");
            RuleFor(l => l.AddDescription).NotEmpty().NotNull().When(x => x.FromProjectStatus == false).WithMessage("Description is required");
            RuleFor(l => l.AssignedUserIds).NotEmpty().WithMessage("At least one developer is required");
            RuleFor(l => l.StartDate).NotEmpty().WithMessage("Start date is required");
            RuleFor(l => l.EndDate).NotEmpty().WithMessage("End date is required");
            RuleFor(l => l.Status).NotNull().WithMessage("Request status is required");
            RuleFor(l => l.TLid).NotEmpty().When(x => x.FromProjectStatus).WithMessage("TL is required");
        }
    }
}

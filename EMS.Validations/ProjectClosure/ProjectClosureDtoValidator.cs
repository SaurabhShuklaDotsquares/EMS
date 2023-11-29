using EMS.Dto;
using FluentValidation;
namespace EMS.Validations
{
    public class ProjectClosureDtoValidator : AbstractValidator<ProjectClosureDto>
    {
        public ProjectClosureDtoValidator()
        {
            RuleFor(l => l.DateOfClosing).NotEmpty().WithMessage("Date of Closing required");
            //RuleFor(l => l.NextStartDate).NotEmpty().WithMessage("Next engagement date required");
            RuleFor(l => l.ProjectID).NotEmpty().WithMessage("Project required");
            RuleFor(l => l.Uid_Dev).NotEmpty().WithMessage("Actual Lead Developer required");
            RuleFor(l => l.Uid_TL).NotEmpty().WithMessage("TL required");
            RuleFor(l => l.Uid_BA).NotEmpty().WithMessage("BA required");
            RuleFor(l => l.Suggestion).NotEmpty().WithMessage("Suggestion required");
            RuleFor(l => l.Reason).NotEmpty().WithMessage("Reason required");
            RuleFor(x => x.DeadResponseDate).NotEmpty().WithMessage("Dead Response Date is required");
        }

    }
}

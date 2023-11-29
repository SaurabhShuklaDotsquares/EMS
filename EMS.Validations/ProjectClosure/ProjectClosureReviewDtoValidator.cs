using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
    public class ProjectClosureReviewDtoValidator : AbstractValidator<ProjectClosureReviewDto>
    {
        public ProjectClosureReviewDtoValidator()
        {
            RuleFor(x => x.ProjectClosureId).NotEmpty();
            RuleFor(x => x.Comments).NotEmpty();
            RuleFor(x => x.PromisingPercentageId).NotEmpty().When(x => !string.IsNullOrWhiteSpace(x.NextStartDate));
            RuleFor(x => x.DeveloperCount).NotNull().GreaterThan(-1).When(x=> !string.IsNullOrWhiteSpace(x.NextStartDate)).WithMessage("Developers : Enter 0 or greater value");
        }
    }
}

using EMS.Dto;
using FluentValidation;
namespace EMS.Validations
{
    public class ProjectClosureDetailDtoValidator : AbstractValidator<ProjectClousreDetailDto>
    {
        public ProjectClosureDetailDtoValidator()
        {
            RuleFor(l => l.ProjectClousreId).NotEmpty().WithMessage("Project Closure Id is required");
            RuleFor(l => l.NextStartDate).NotEmpty().WithMessage("*Next Date is required");
            RuleFor(l => l.Reason).NotEmpty().WithMessage("*Reason is required");
            RuleFor(l => l.ConversionDate).NotEmpty().WithMessage("*Conversion Date is required");
        }
    }
}

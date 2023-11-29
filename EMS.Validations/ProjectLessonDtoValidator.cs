using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
    public class ProjectLessonDtoValidator : AbstractValidator<ProjectLessonDto>
    {
        public ProjectLessonDtoValidator()
        {
            RuleFor(m => m.ProjectId).NotEmpty().WithMessage("*required");
            RuleFor(m => m.LearnedLessons).NotEmpty().WithMessage("Learned lessons required")
                .SetCollectionValidator(new ProjectLessonLearnedDtoValidator());
        }
    }
}

using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
    public class ProjectLessonLearnedDtoValidator : AbstractValidator<ProjectLessonLearnedDto>
    {
        public ProjectLessonLearnedDtoValidator()
        {
            RuleFor(m => m.ProjectLessonTopicId).NotEmpty().WithMessage("*required");
        }
    }
}

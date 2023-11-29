using FluentValidation;
using EMS.Dto;

namespace EMS.Validations
{
    public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
    {
        public CreateTaskDtoValidator()
        {
            RuleFor(m => m.TaskName).NotEmpty().WithMessage("*required");
            RuleFor(m => m.Remark).NotEmpty().WithMessage("*Remark is required");                     
            RuleFor(x => x.TaskEndDate).NotEmpty().WithMessage("*required");
            RuleFor(x => x.Assign).NotEmpty().WithMessage("*required");
        }
    }
}

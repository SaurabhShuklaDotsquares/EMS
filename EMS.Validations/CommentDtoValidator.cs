using FluentValidation;
using EMS.Dto;


namespace EMS.Validations
{
    public class CommentDtoValidator : AbstractValidator<Commentdto>
    {
        public CommentDtoValidator()
        {
            RuleFor(m => m.Comment).NotEmpty().WithMessage("*required");
            RuleFor(m => m.TaskStatusID).NotEmpty().WithMessage("*required");
            RuleFor(m => m.TaskID).NotEmpty().WithMessage("*required");
        }
    }
}


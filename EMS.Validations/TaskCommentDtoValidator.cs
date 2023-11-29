using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
    public class TaskCommentDtoValidator : AbstractValidator<TaskCommentDto>
    {
        public TaskCommentDtoValidator()
        {
            RuleFor(l => l.Comment).NotNull().WithMessage("*required");           
        }
    }
}

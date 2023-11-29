using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Validations
{
   public class ProjectClientFeedbackDetailDtoValidator: AbstractValidator<ProjectClientFeedbackDetailDto>
    {
        public ProjectClientFeedbackDetailDtoValidator()
        {
            RuleFor(f => f.ProjectId).NotNull().WithMessage("*Project is required");
            RuleFor(f => f.Status).NotNull().WithMessage("*Status is required");
            RuleFor(f => f.MeetRequirements).NotNull().WithMessage("*Meet requirement is required");
            RuleFor(f => f.ValueAboutDotsquares).NotNull().WithMessage("*Value about dotsquares is required");
        }
        
    }
}

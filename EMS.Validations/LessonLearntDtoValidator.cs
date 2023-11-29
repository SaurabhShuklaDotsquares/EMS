using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Validations
{
    public class LessonLearntDtoValidator : AbstractValidator<LessonLearntDto>
    {
        public LessonLearntDtoValidator()
        {
            //RuleFor(l => l.ProjectId).NotNull();
            RuleFor(l => l.WhatLearnt).NotNull();
        }
    }
}

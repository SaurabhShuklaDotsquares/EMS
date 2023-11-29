using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Validations.Sme
{
    public class SmeDtoValidator : AbstractValidator<SmeDto>
    {
        public SmeDtoValidator()
        {
            RuleFor(A => A.SubjectMatterExpert).NotNull().WithMessage("*required");
            RuleFor(A => A.Level1).NotNull().WithMessage("*required");
            //RuleFor(A => A.Level2).NotNull().WithMessage("*required");
            //RuleFor(A => A.Level3).NotNull().WithMessage("*required");
            //RuleFor(A => A.Level4).NotNull().WithMessage("*required");
            //RuleFor(A => A.Level5).NotNull().WithMessage("*required");
        }
    }
}

using EMS.Data;
using EMS.Dto;
using EMS.Validations.LibraryManagement;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMS.Validations
{
    public class EstimateHourDtoValidator:AbstractValidator<EstimateHourDto>
    {

        public EstimateHourDtoValidator()
        {
            RuleFor(l => l.RequirementName).NotNull().WithMessage("*Title is required.").
                MaximumLength(250).WithMessage("Title can be max 250 characters long");

            RuleFor(l => l.RequirementDesc).MaximumLength(2000).WithMessage("Description can be max 2000 characters long"); //.NotNull().WithMessage("Description is required.")

            RuleFor(l => l.EstimatedHour).NotNull().WithMessage("Estimate hour is required.");

            //RuleFor(l => l.FileName).MaximumLength(250).WithMessage("File name can be max 250 characters long");

            RuleFor(m => m.Bauid).NotEmpty().WithMessage("BA is required");
            RuleFor(m => m.Tluid).NotEmpty().WithMessage("TL is required");
            RuleFor(m => m.DomainId).NotEmpty().WithMessage("*required");

            //RuleFor(l => l.LiveURL).MaximumLength(300).WithMessage("Live URL can be max 300 characters long");
            //RuleForEach(x => x.LibraryFiles).SetValidator(new FileValidator());
            //RuleFor(y => y.Title).Must(IsNameUnique)
            //  .WithMessage("Title must be unique");
        }
    }
}

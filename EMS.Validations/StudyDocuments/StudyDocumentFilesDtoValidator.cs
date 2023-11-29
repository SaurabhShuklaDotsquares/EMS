using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Validations.StudyDocuments
{
    public class StudyDocumentFilesDtoValidator : AbstractValidator<StudyDocumentFilesDto>
    {
        public StudyDocumentFilesDtoValidator()
        {
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage("*required").MaximumLength(100).WithMessage("maximum 100 characters");
        }
    }
}

using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Validations.StudyDocuments
{
    public class StudyDocumentsDtoValidator : AbstractValidator<StudyDocumentsDto>
    {
        public StudyDocumentsDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("required*").MaximumLength(100).WithMessage("maximum 100 characters"); 
            RuleFor(x => x.TechnologyId).NotEmpty().WithMessage("required*");
            
            RuleForEach(x => x.studyDocumentFiles).SetValidator(new StudyDocumentFilesDtoValidator());   
        }
    }
}

using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Validations.StudyDocuments
{
    public class UnapprovedReasonDtoValidator : AbstractValidator<StudyDocumentsUnapprovedReasonDto>
    {
        public UnapprovedReasonDtoValidator()
        {
            RuleFor(x => x.StudyDocumentIds).NotEmpty().WithMessage("required*");
            RuleFor(x => x.UnapprovedReason).NotEmpty().WithMessage("required*").MaximumLength(300).WithMessage("maximum 300 characters");
        }
    }

}

using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Validations.StudyDocuments
{
    public class StudyDocumentAddDelUsersPermissionValidator : AbstractValidator<StudyDocumentAddDelUsersPermission>
    {
        public StudyDocumentAddDelUsersPermissionValidator()
        {
            RuleFor(x => x.StudyDocumentIds).NotEmpty().WithMessage("required*");
            RuleFor(x => x.UserId).NotNull().WithMessage("required*").NotEmpty().WithMessage("required*");
            RuleFor(x => x.StartDate).NotEmpty().WithMessage("required*");
            RuleFor(x => x.EndDate).NotEmpty().WithMessage("required*");
        }
    }
}

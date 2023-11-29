using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Validations.StudyDocuments
{
    public class StudyDocumentsPermissionsDtoValidator : AbstractValidator<StudyDocumentsPermissionDto>
    {
        public StudyDocumentsPermissionsDtoValidator()
        {
            // child
            RuleFor(x => x.userPermission).SetValidator(new UserPermissionsDtoValidator());
        }
    }

    public class UserPermissionsDtoValidator : AbstractValidator<UserPermissionsDto>
    {
        public UserPermissionsDtoValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("required*").GreaterThan(0).WithMessage("required*");
            RuleFor(x => x.StartDate).NotEmpty().WithMessage("required*");
            RuleFor(x => x.EndDate).NotEmpty().WithMessage("required*");
 
        }
    }
}

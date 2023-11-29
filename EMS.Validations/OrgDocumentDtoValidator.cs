using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
    public class OrgDocumentDtoValidator : AbstractValidator<OrgDocumentDto>
    {
        public OrgDocumentDtoValidator()
        {
            RuleFor(m => m.DocType).NotEmpty().WithMessage("*required");
            RuleFor(m => m.OrgDocumentMasterId).NotEmpty().WithMessage("*required");
            RuleFor(m => m.Document).NotEmpty().When(x => x.Id == 0).WithMessage("*required");
            RuleFor(m => m.DepartmentIds).NotEmpty().When(x => x.RoleIds == null || x.RoleIds.Length == 0).WithMessage("Choose at least one department or role");
            RuleFor(m => m.RoleIds).NotEmpty().When(x => x.DepartmentIds == null || x.DepartmentIds.Length == 0).WithMessage("Choose at least one department or role");
        }
    }
}

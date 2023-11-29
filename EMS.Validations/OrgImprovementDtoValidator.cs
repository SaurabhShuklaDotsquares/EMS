using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
    public class OrgImprovementDtoValidator: AbstractValidator<OrgImprovementDto>
    {
        public OrgImprovementDtoValidator()
        {
            RuleFor(i => i.Title).NotNull().WithMessage("*Title is required");
            RuleFor(i => i.TypeId).NotNull().WithMessage("*Type is required");
            RuleFor(i => i.ImprovementDate).NotNull().WithMessage("*Improvement Date is required");
            //RuleFor(i => i.EmployeeUid).NotNull().WithMessage("*User is required");
        }
        
    }
}

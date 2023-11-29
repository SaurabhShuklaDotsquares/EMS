using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
    public class ComplaintDtoValidator : AbstractValidator<ComplaintDto>
    {
        public ComplaintDtoValidator()
        {
            RuleFor(l => l.EmployeeId).NotNull();
            RuleFor(l => l.ClientComplain).NotNull().When(l=>l.ComplaintTypeId == 2);
            RuleFor(l => l.TlExplanation).NotNull().When(l => l.ComplaintTypeId == 1);
        }
    }
}

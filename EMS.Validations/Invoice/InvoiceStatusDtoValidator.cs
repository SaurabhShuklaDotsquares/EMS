using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
    public class InvoiceStatusDtoValidator : AbstractValidator<InvoiceStatusDto>
    {
        public InvoiceStatusDtoValidator()
        {
            RuleFor(l => l.InvoiceId).NotEmpty().WithMessage("Invoice Id is required");
            RuleFor(l => l.InvoiceStatusId).NotEmpty().WithMessage("Status is required");
        }
    }
}

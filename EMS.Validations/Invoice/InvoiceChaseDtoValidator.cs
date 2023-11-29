using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
    public class InvoiceChaseDtoValidator : AbstractValidator<InvoiceChaseDto>
    {
        public InvoiceChaseDtoValidator()
        {
            RuleFor(l => l.InvoiceId).NotEmpty().WithMessage("Invoice Id is required");
            RuleFor(l => l.ChaseDate).NotEmpty().WithMessage("*required");
            RuleFor(l => l.Comment).NotEmpty().WithMessage("*required");
        }
    }
}

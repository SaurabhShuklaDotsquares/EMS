using FluentValidation;
using EMS.Dto;


namespace EMS.Validations
{
    public class ExpenseDtoValidator : AbstractValidator<ExpenseDto>
    {
        public ExpenseDtoValidator()
        {
            RuleFor(m => m.Descp).NotEmpty().WithMessage("Description is required");
            RuleFor(m => m.Amount).NotEmpty().WithMessage("Amount is required");
            RuleFor(m => m.ExpenseDate).NotEmpty().WithMessage("Expense Date is required");
            RuleFor(m => m.PaidThrough).NotEmpty().WithMessage("Payment made through is required");
        }       
    }
}

using FluentValidation;
using EMS.Dto;


namespace EMS.Validations
{
    public class LeadStatusDtoValidator : AbstractValidator<LeadStatusDto>
    {
        public LeadStatusDtoValidator()
        {
            RuleFor(m => m.LeadId).NotEmpty().WithMessage("Lead Id must be greater than 0");
            RuleFor(m => m.Status).NotEmpty().WithMessage("Status is required");
            RuleFor(m => m.NextChaseDate).NotEmpty().When(x => x.Status != 21).WithMessage("Next Chase Date is required");
            RuleFor(m => m.Notes).NotEmpty().WithMessage("Notes field is required");
            RuleFor(m => m.ConversionDate).NotEmpty().When(x => x.IsAlmostConverted == true && x.Status != 21).WithMessage("Conversion Date is required");
        }
    }
}


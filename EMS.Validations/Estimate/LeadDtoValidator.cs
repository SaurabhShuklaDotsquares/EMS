using FluentValidation;
using EMS.Dto;


namespace EMS.Validations
{
    public class LeadDtoValidator : AbstractValidator<LeadDto>
    {
        public LeadDtoValidator()
        {
            RuleFor(m => m.AbroadPMId).NotEmpty().WithMessage("Lead from is required");
            RuleFor(m => m.LeadCRMId).NotEmpty().WithMessage("Lead CRMId is required");
            RuleFor(m => m.Title).NotEmpty().WithMessage("Title is required");

            RuleFor(m => m.AssignedDate).NotEmpty().WithMessage("Generated Date is required");
            RuleFor(m => m.ConversionDate).NotEmpty().When(x => x.Status != 21).WithMessage("Conversion Date is required");
            RuleFor(m => m.LeadType).NotEmpty().WithMessage("Lead Type is required");
            //RuleFor(m => m.EstimateTimeInDays).NotEmpty().WithMessage("Estimate Time is required");

            RuleFor(m => m.EstimateOwnerId).NotEmpty().WithMessage("Estimate Owner is required");
            RuleFor(m => m.CommunicatorOwnerId).NotEmpty().WithMessage("Communicator Owner is required");
            RuleFor(m => m.ProjectTag).NotEmpty().WithMessage("Project Tag is required");

            RuleFor(m => m.Technician).NotEmpty().WithMessage("Choose atleast one Technical helper");
            RuleFor(m => m.Technology).NotEmpty().WithMessage("Choose atleast one Technology");
        }
    }
}


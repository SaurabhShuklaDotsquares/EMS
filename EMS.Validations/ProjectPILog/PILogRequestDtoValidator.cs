using EMS.Dto;
using FluentValidation;


namespace EMS.Validations
{
    public class PILogRequestDtoValidator : AbstractValidator<PILogRequestDto>
    {
        public PILogRequestDtoValidator()
        {
            RuleFor(m => m.ProcessId).NotEmpty().WithMessage("Process Name is required");
            RuleFor(m => m.PotentialArea).NotEmpty().WithMessage("Potential Improvement Area is required");
            RuleFor(m => m.ProcessName).NotEmpty().WithMessage("Other Process Name is required");
        }        
    }
}

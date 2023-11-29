using FluentValidation;
using EMS.Dto;


namespace EMS.Validations
{
    public class MeetingMasterDtoValidator : AbstractValidator<MeetingMasterDto>
    {
        public MeetingMasterDtoValidator()
        {
            RuleFor(m => m.Title).NotEmpty().WithMessage("*required");
        }       
    }
}

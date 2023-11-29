using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
    public class ReturnDeviceDtoValidator : AbstractValidator<ReturnDeviceDto>
    {
        public ReturnDeviceDtoValidator()
        {
            RuleFor(m => m.ReturnToUid).NotNull().WithMessage("*required");            
            RuleFor(m => m.ReturnDate).NotEmpty().WithMessage("*required");
        }
    }
}

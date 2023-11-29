
using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
    public class AssignDeviceDtoValidator : AbstractValidator<AssignDeviceDto>
    {
        public AssignDeviceDtoValidator()
        {
            RuleFor(m => m.DeviceType).NotEmpty().NotNull().WithMessage("*required");
            RuleFor(m => m.DeviceId).NotEmpty().NotNull().WithMessage("*required");
            RuleFor(m => m.AssignedToUid).NotEmpty().NotNull().WithMessage("*required");
            RuleFor(m => m.AssignedDateTime).NotEmpty().WithMessage("*required");
            RuleFor(m => m.SerialNumber).NotEmpty().WithMessage("*required");
        }
    }
}

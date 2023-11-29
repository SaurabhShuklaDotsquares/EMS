
using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
    public class DeviceDataDtoValidator : AbstractValidator<DeviceDataDto>
    {
        public DeviceDataDtoValidator()
        {
            RuleFor(m => m.Name).NotEmpty().WithMessage("*required");
            RuleFor(m => m.Quantity).NotEmpty().WithMessage("*required");
        }
    }
}

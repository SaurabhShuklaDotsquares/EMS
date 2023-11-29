using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Validations
{
    public class DeviceDtoValidator : AbstractValidator<DeviceDto>
    {
        public DeviceDtoValidator()
        {           
            RuleFor(m => m.AssignUid).NotEmpty();
            RuleFor(m => m.StartTime).NotEmpty();
            RuleFor(m => m.Description).NotEmpty();
            RuleFor(m => m.DeviceId).NotEmpty().When(x=> !x.SimId.HasValue && (x.Accessory==null || !x.Accessory.Any()));
            RuleFor(m => m.SimId).NotEmpty().When(x => !x.DeviceId.HasValue && (x.Accessory == null || !x.Accessory.Any()));
            RuleFor(m => m.Accessory).NotEmpty().When(x => !x.DeviceId.HasValue && !x.SimId.HasValue);
        }
    }   
}

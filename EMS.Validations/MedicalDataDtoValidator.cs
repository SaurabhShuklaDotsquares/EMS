using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Validations
{
    public class MedicalDataDtoValidator : AbstractValidator<MedicalDataDto>
    {
        public MedicalDataDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("*required");
            //RuleFor(x => x.EmployeeCode).NotEmpty().WithMessage("*required");
            RuleFor(x => x.Designation).NotEmpty().WithMessage("*required");
            RuleFor(x => x.DOB).NotEmpty().WithMessage("*required");
        }

    }
}

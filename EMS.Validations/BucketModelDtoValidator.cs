using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Dto;
using FluentValidation;

namespace EMS.Validations
{
    public class BucketModelDtoValidator:AbstractValidator<BucketModelDto>
    {
        public BucketModelDtoValidator()
        {
            RuleFor(l => l.ModelName).NotNull().WithMessage("*required");
            RuleFor(l => l.ModelCode).NotNull().WithMessage("*required");
        }
    }
}

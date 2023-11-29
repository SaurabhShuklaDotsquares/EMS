using EMS.Dto;
using EMS.Dto.CVEstimatePrice;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Validations
{
    public class CVEstimatePriceDtoValidator : AbstractValidator<CVEstimatePriceDto>
    {
        public CVEstimatePriceDtoValidator()
        {
            RuleFor(A => A.Price).NotNull().WithMessage("*required");
        }
    }
}

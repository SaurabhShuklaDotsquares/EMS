using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Validations
{
    public class EstimateCalculationDtoValidator : AbstractValidator<EstimateCalculationDto>
    {
        public EstimateCalculationDtoValidator()
        {
            RuleFor(l => l.CRMLeadId).NotNull().WithMessage("CRM Lead Id is required.");
            //When(model => model.Command == "Save Calculator", () =>
            //{
            //    RuleFor(model => model.CRMLeadId).NotNull().WithMessage("CRM Lead Id is required.");
            //});

        }
    }
}

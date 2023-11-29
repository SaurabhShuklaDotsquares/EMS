using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Validations
{
    public class InvoiceDtoValidator : AbstractValidator<InvoiceDto>
    {
        public InvoiceDtoValidator()
        {
            RuleFor(l => l.InvoiceAmount).NotEmpty().When(x => x.Id == 0).WithMessage("*required");
            RuleFor(l => l.InvoiceNumber).NotEmpty().WithMessage("*required");
            RuleFor(l => l.Comment).NotEmpty().WithMessage("*required");
            RuleFor(l => l.StartDate).NotEmpty().WithMessage("*required");
            RuleFor(l => l.EndDate).NotEmpty().WithMessage("*required");
            RuleFor(l => l.Uid_TL).NotEmpty().WithMessage("*required");
            RuleFor(l => l.Uid_BA).NotEmpty().WithMessage("*required");
            RuleFor(l => l.InvoiceStatusId).NotEmpty().WithMessage("*required");
            RuleFor(l => l.ProjectId).NotEmpty().WithMessage("*required");
        }

    }
}


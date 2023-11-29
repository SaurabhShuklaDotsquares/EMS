using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Validations
{
    public class UploadDocumentDtoValidator : AbstractValidator<UploadDocumentDto>
    {
        public UploadDocumentDtoValidator()
        {
            RuleFor(l => l.Title).NotNull().WithMessage("*Required");
            RuleFor(l => l.Tags).NotNull().WithMessage("*Required");
            //RuleFor(l => l.EstimateDocPath).NotNull().WithMessage("&nbsp");
            //RuleFor(l => l.Industry).NotNull().WithMessage("*Required");
            RuleFor(l => l.Technology).NotNull().WithMessage("*Required");
            
        }
    }
}

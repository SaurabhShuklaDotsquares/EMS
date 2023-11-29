using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Dto;
using FluentValidation;
using System.Text.RegularExpressions;

namespace EMS.Validations
{
   public class PreferenceDtoValidator : AbstractValidator<PreferenceDto>
    {
        public PreferenceDtoValidator()
        {

            RuleFor(l => l.EmailFrom).NotNull().WithMessage("*Required");
            RuleFor(l => l.EmailFrom).Matches(@"(([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)(\s*;\s*|\s*$))*").WithMessage("*Invalid Email");
            RuleFor(l => l.EmailHR).Matches(@"(([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)(\s*;\s*|\s*$))*").WithMessage("*Invalid Email");
            RuleFor(l => l.EmailPM).Matches(@"(([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)(\s*;\s*|\s*$))*").WithMessage("*Invalid Email");
            RuleFor(l => l.TimeSheetEmail).Matches(@"(([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)(\s*;\s*|\s*$))*").WithMessage("*Invalid Email");
            RuleFor(l => l.ProjectClosureEmail).Matches(@"(([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)(\s*;\s*|\s*$))*").WithMessage("*Invalid Email");
            RuleFor(l => l.EmailDeveloper).Matches(@"(([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)(\s*;\s*|\s*$))*").WithMessage("*Invalid Email");
            //RuleFor(l => l.DeveloperFile.FileName).Matches(@"^.*\.(pdf|PDF|doc|DOC|docx|DOCX)$").WithMessage("*Invalid File");
         //   RuleFor(l => l.EmailDeveloper).Matches(@"^.*\.(pdf|PDF|doc|DOC|docx|DOCX)$").WithMessage("*Invalid File");

        }
    }
}

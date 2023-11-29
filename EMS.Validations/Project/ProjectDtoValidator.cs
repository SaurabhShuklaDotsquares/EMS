using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Dto;
using FluentValidation;
namespace EMS.Validations
{
   public class ProjectDtoValidator : AbstractValidator<ProjectDto>
    {
        public ProjectDtoValidator()
        {

            RuleFor(l => l.Name).NotNull().WithMessage("*Required");
            RuleFor(l => l.CRMId).NotNull().WithMessage("*Required");
            RuleFor(l => l.Model).NotNull().WithMessage("*Required");
            RuleFor(l => l.BillingTeam).NotNull().WithMessage("*Required");
            RuleFor(l => l.Status).NotNull().WithMessage("*Required");
            RuleFor(l => l.DepartmentList).NotNull().WithMessage("*Required");
            RuleFor(l => l.TechnologyList).NotNull().WithMessage("*Required");
            RuleFor(l => l.StartDate).NotNull().WithMessage("*Required");
            RuleFor(l => l.Department).NotNull().WithMessage("*Required");
            RuleFor(l => l.PMUid).NotNull().WithMessage("*Required");
        }
    }
}

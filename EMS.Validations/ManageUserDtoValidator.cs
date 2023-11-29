using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Validations
{
    public class ManageUserDtoValidator : AbstractValidator<ManageUserDto>
    {
        public ManageUserDtoValidator()
        {
            RuleFor(l => l.EmailOffice).NotNull().WithMessage("*required").EmailAddress().WithMessage("*Office email format not valid.");
            RuleFor(l => l.Name).NotNull().WithMessage("*required");
            RuleFor(l => l.DOB).NotNull().WithMessage("*required");
            RuleFor(l => l.JoinedDate).NotNull().WithMessage("*required");
            RuleFor(l => l.AadharNumber).Length(0, 20).WithMessage("Aadhar Number length must be less than 20 digits.");
            RuleFor(l => l.PanNumber).Length(0, 20).WithMessage("PAN Number length must be less than 20 characters.");
            RuleFor(l => l.PassportNumber).Length(0, 20).WithMessage("Passport Number length must be less than 20 characters.");
            RuleFor(l => l.PMUid).NotEmpty().WithMessage("*required");
            RuleFor(l => l.TLId).NotEmpty().WithMessage("*required");
            RuleFor(l => l.DeptId).NotEmpty().WithMessage("*required");
            RuleFor(l => l.MobileNumber).NotNull().WithMessage("*required");
            RuleFor(l => l.JobTitle).NotNull().WithMessage("*required");
            RuleFor(l => l.RoleCateGoryId).NotEmpty().WithMessage("*required");
            RuleFor(l => l.RoleId).NotEmpty().WithMessage("*required");
            RuleFor(l => l.DesignationId).NotEmpty().WithMessage("*required");
            
            //RuleFor(l => l.AlternativeNumber).NotEmpty().WithMessage("*required");
        }
    }
}

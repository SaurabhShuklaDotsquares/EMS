using EMS.Data;
using EMS.Dto;
using EMS.Validations.LibraryManagement;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMS.Validations
{
    public class LibraryDtoValidator : AbstractValidator<LibraryDto>
    {
        public LibraryDtoValidator()
        {
            RuleFor(l => l.Title).NotNull().WithMessage("*Title is required.").
                MaximumLength(250).WithMessage("Title can be max 250 characters long");
            RuleFor(l => l.Description).NotNull().WithMessage("Description is required.").
                MaximumLength(2000).WithMessage("Description can be max 2000 characters long");
            RuleFor(l => l.keywords).MaximumLength(250).WithMessage("Keyword can be max 250 characters long");
            RuleFor(l => l.LiveURL).MaximumLength(300).WithMessage("Live URL can be max 300 characters long");
            RuleForEach(x => x.LibraryFiles).SetValidator(new FileValidator());
            RuleFor(x => x.LibraryTypeId).NotNull().WithMessage("Type is required.").NotEmpty().WithMessage("Type is required.").NotEqual((byte)EMS.Core.Enums.LibraryType.Select).WithMessage("Type is required.");
            RuleFor(x => x.SalesKitId).NotNull().WithMessage("Required.").NotEmpty().WithMessage("Required.");
            RuleFor(x => x.CvsId).NotNull().WithMessage("Required.").NotEmpty().WithMessage("Required.");
            //RuleFor(y => y.Title).Must(IsNameUnique)
            //  .WithMessage("Title must be unique");
        }

        private bool IsNameUnique(string arg)
        {
            using (db_dsmanagementnewContext context = new db_dsmanagementnewContext())
            {
                var data = context.Library.Where(x => x.Title.ToLower() == arg.ToLower()).FirstOrDefault();
                if (data == null)
                    return false;
            }
            return true;
        }

        //public bool IsNameUnique(Library library, string newValue)
        //{
        //   // return _library.All(x =>
        //    //  x.Equals(library) || x.Title != newValue);
        //}
    }
}

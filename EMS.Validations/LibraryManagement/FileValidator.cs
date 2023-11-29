using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Validations.LibraryManagement
{
    public class FileValidator : AbstractValidator<IFormFile>
    {
        public FileValidator()
        {
            // 1kb = 1024byte
            // 1024kbx200mb = 204800kbx1024byte = 209715200
            RuleFor(x => x.Length).NotNull().LessThanOrEqualTo(209715200)
                .WithMessage("Max 200 MB file size allowed");
            RuleFor(y => y.Length).Must(CheckLength)
              .WithMessage("Title must be unique");
        }
        private bool CheckLength(long arg)
        {
            return true;
        }
    }
}

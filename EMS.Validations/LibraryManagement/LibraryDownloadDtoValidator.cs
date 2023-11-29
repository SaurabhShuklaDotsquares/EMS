using EMS.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Validations
{
    public class LibraryDownloadDtoValidator:AbstractValidator<LibraryDownloadDto>
    {
        public LibraryDownloadDtoValidator()
        {
            //RuleFor(ld => ld.UserLoginId).NotEmpty().WithMessage("*Required");
        }
    }
}

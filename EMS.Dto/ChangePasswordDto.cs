using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EMS.Dto
{
    public class ChangePasswordDto
    {
        [DataType(DataType.Password)]
        [DisplayName("Password*")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("New Password*")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Confirm New Password*")]
        public string ConfirmPassword { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EMS.Dto
{
    public class ChangeUserPasswordDto
    {
        
        
        public int UserId { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("New Password*")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Confirm New Password*")]
        public string ConfirmPassword { get; set; }
    }
}

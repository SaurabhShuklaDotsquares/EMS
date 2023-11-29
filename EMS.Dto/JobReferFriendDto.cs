using EMS.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace EMS.Dto
{
    public class JobReferFriendDto
    {
        public int Id { get; set; }
        public int? CurrentOpeningId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [DataType(DataType.PhoneNumber, ErrorMessage = "Provided phone number not valid")]
        [DisplayName("Mobile Number")]
        public string MobileNo { get; set; }
        [DisplayName("Description")]
        public string Small_Desc { get; set; }

        public int Status { get; set; }
        [DisplayName("Upload Resume")]
        public string AttacchmentFileName { get; set; }
        public IFormFile Attacchment { get; set; }

        public CurrentOpening CurrentOpening { get; set; }

        public UserLogin UserLogin { get; set; }
    }
}

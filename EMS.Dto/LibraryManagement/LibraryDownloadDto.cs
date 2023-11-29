using EMS.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EMS.Dto
{
    public class LibraryDownloadDto
    {
        public LibraryDownloadDto()
        {
            Roles = new List<SelectListItem>();
            Users = new List<SelectListItem>();
            TypeList = new List<SelectListItem>();
        }
        [DisplayName("Roles")]
        public List<SelectListItem> Roles { get; set; }
        public List<SelectListItem> Users { get; set; }
        [Required]
        public List<SelectListItem> TypeList { get; set; }
        [DisplayName("Maximum Download In Day")]
        public int? MaximumDownloadInDay { get; set; }
        [DisplayName("Maximum Download In Month")]
        public int? MaximumDownloadInMonth { get; set; }

        public int LibraryFileTypeId { get; set; }
        public int? RoleId { get; set; }
        public int? UserLoginId { get; set; }
        public long Id { get; set; }
        public int Type { get; set; }

    }

    public class LibraryDownloadHistoryDto
    {
        public LibraryDownloadHistoryDto()
        {
            Users = new List<SelectListItem>();
        }
        public List<SelectListItem> Users { get; set; }
        public string LibraryTitle { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
}

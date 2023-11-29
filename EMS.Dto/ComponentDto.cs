using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{
   public class ComponentDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public byte Type { get; set; }

        [Display(Name="Category")]
        public int ComponentCategoryId { get; set; }
        public string Tags { get; set; }
        public string UploadedBy { get; set; }
        [Display(Name = "Upload HTML")]
        public string ImageName { get; set; }
        public string DataUrl { get; set; }

        [Display(Name = "Upload Image")]
        public string DesignImages { get; set; }

        [Display(Name = "Tips")]
        public string Description { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime Modified { get; set; }
        public bool IsActive { get; set; }
        public int CreatedByUid { get; set; }
        public List<SelectListItem> CategoryList { get; set; }
        public List<SelectListItem> TypeList { get; set; }

        private bool _IsSuperAdmin = false;
        public bool IsSuperAdmin
        {
            get { return _IsSuperAdmin; }
            set { _IsSuperAdmin = value; }
        }

        public string FileExtension { get; set; }
        [Display(Name = "Upload PSD")]
        public string PsdImages { get; set; }

        public int UId { get; set; }

    }
}

using EMS.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class OrgImprovementDto
    {
        public OrgImprovementDto()
        {
            Users = new List<SelectListItem>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        
        public Enums.ImprovementType TypeId { get; set; }
        public string ImprovementDate { get; set; }
        public string Description { get; set; }
        public int EmployeeUid { get; set; }
        public int CurrentUserId { get; set; }
        public List<SelectListItem> Users { get; set; }
        public List<SelectListItem> ImprovementTypes { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace EMS.Dto
{
    public class ManageForecastingDto
    {
        public int ForecastingType { get; set; }
        public int Id { get; set; }
        public string Lead { get; set; }
        public string ProjectDescription { get; set; }
        public string DeveloperCount { get; set; }
        public string Country { get; set; }
        public string TentiveDate { get; set; }
        public string SelectedDepartment { get; set; }
        public List<SelectListItem> DepartmentList { get; set; }
        public string ProjectId { get; set; }
        public int OwnerUId { get; set; }
        public int? ReviewedUId { get; set; }
        public bool IsHold { get; set; }
        public string HoldReason { get; set; }
    }

    public class ClientDto
    {
        public string ClientOrProjectId { get; set; }
        public int CrmId { get; set; }
        public int? PMUid { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
    }
}

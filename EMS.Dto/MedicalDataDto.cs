using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Dto
{
    
    public class MedicalDataDto
    {
        public MedicalDataDto()
        {
            Relatives = new List<RelativeMedicalDataDto>();
            Employee = new List<SelectListItem>();
        }
        /*Start add propery for medicaldata relative 23 Nov */
        public bool ShowRelative { get; set; }
        /*End add propery for medicaldata relative 23 Nov */
        public int Id { get; set; }
        public string EmployeeCode { get; set; }
        public byte NameTitle { get; set; }
        public string Name { get; set; }
        public byte Gender { get; set; }
        public string Designation { get; set; }

        public string JoiningDate { get; set; }
        public string DOB { get; set; }
        public System.DateTime AddedDate { get; set; }
        public bool IsActive { get; set; }
        public int UserId { get; set; }
        public string EmpGender { get; set; }
        public decimal PremiumTotal { get; set; }
        public decimal PremiumPerMonth { get; set; }
        public int TotalCoverage { get; set; }
        public int Validity { get; set; }
        public List<RelativeMedicalDataDto> Relatives { get; set; }

        public int EmployeeId { get; set; }
        public List<SelectListItem> Employee { get; set; }
    }

    public class RelativeMedicalDataDto
    {
        public int Id { get; set; }
        public int EmployeeMedicalId { get; set; }
        public byte Relation { get; set; }
        public byte NameTitle { get; set; }
        public string Name { get; set; }
        public byte Gender { get; set; }
        public string DOB { get; set; }
        public System.DateTime AddedDate { get; set; }
        public bool IsActive { get; set; }
        public string RelationName { get; set; }
        public string RelativeGender { get; set; }
    }
}

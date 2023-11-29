using EMS.Data;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using static EMS.Core.Enums;
using System;
using EMS.Data.Model;

namespace EMS.Dto
{
    public class CreateTDSDto
    {
        public CreateTDSDto()
        {
            TDSType = new List<Tdstype>();
            TdsdeductionType = new List<TdsdeductionType>();

        }
        public int EmpDeductionId { get; set; }

        [DisplayName("Deduction Type")]
        public int DeductionTypeId { get; set; }

        [DisplayName("Type Type")]

        public int? TypeId { get; set; }
        public string Uid { get; set; }
        public int StatusId { get; set; }

        [DisplayName("Assesment Year")]

        public int AssesmentYearId { get; set; }

        [DisplayName("Other Deduction")]

        public string OtherDeduction { get; set; }

        [DisplayName("Claimed By Employee")]

        public decimal? ClaimedByEmployee { get; set; }

        [DisplayName("Given By Employer")]

        public decimal? GivenByEmployer { get; set; }

        [DisplayName("Employee Remark")]

        public string EmployeeRemark { get; set; }

        [DisplayName("Employer Remark")]
        public string EmployerRemark { get; set; }

        [DisplayName("File Name")]
        public string FileName { get; set; }
        public string File { get; set; }


        public DateTime CreatedDate { get; set; }
        public DateTime? ModifyDate { get; set; }

        public List<SelectListItem> DeductionType { get; set; }

        public virtual List<TdsassesmentYear> AssesmentYear { get; set; }
        public virtual List<TdsdeductionType>  TdsdeductionType { get; set; }
        public virtual List<Tdstype> TDSType { get; set; }


    }
}

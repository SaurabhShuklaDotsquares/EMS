using System;
using System.Collections.Generic;

namespace EMS.Data.Model
{
    public partial class TdsempDeduction
    {
        public int EmpDeductionId { get; set; }
        public int DeductionTypeId { get; set; }
        public int? TypeId { get; set; }
        public int Uid { get; set; }
        public int StatusId { get; set; }
        public int AssesmentYearId { get; set; }
        public string OtherDeduction { get; set; }
        public decimal? ClaimedByEmployee { get; set; }
        public decimal? GivenByEmployer { get; set; }
        public string EmployeeRemark { get; set; }
        public string EmployerRemark { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifyDateEmp { get; set; }
        public DateTime? ModifyDateAc { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifyByEmp { get; set; }
        public int? ModifyByAc { get; set; }
        public bool? IsActive { get; set; }
        public bool IsLockUnlock { get; set; }
        public string PanNumberByEmployee { get; set; }
        public bool Is13Ahradedution { get; set; }
        public bool Is24Bhouseloan { get; set; }

        public virtual TdsassesmentYear AssesmentYear { get; set; }
        public virtual TdsdeductionType DeductionType { get; set; }
        public virtual Tdstype Type { get; set; }
        public virtual UserLogin U { get; set; }
        public virtual UserLogin CreatedByNavigation { get; set; }
        //public virtual UserLogin ModifyByNavigation { get; set; }
        public virtual UserLogin ModifyByAcNavigation { get; set; }
        public virtual UserLogin ModifyByEmpNavigation { get; set; }
        public virtual ICollection<TdsdeductionDoc> TdsdeductionDoc { get; set; }

    }



    public class TDSDeductionModel
    {
        public TdsempDeduction TdsempDeduction { get; set; }

        public IList<TdsdeductionDoc> TdsDoc { get; set; }

    }
}

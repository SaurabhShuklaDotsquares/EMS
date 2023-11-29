using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Investment
    {
        public Investment()
        {
            InvestmentDocuments = new HashSet<InvestmentDocument>();
            InvestmentMonths = new HashSet<InvestmentMonth>();
            InvestmentTypeAmountMaps = new HashSet<InvestmentTypeAmountMap>();
        }

        public int Id { get; set; }
        public int UserloginId { get; set; }
        public int FinancialYearId { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public string FatherName { get; set; }
        public string HomeAddress { get; set; }
        public string PAN { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string AttendanceCode { get; set; }
        public bool IsDraft { get; set; }

        public virtual FinancialYear FinancialYear { get; set; }
        public virtual UserLogin UserLogin { get; set; }
        public virtual ICollection<InvestmentDocument> InvestmentDocuments { get; set; }
        public virtual ICollection<InvestmentMonth> InvestmentMonths { get; set; }
        public virtual ICollection<InvestmentTypeAmountMap> InvestmentTypeAmountMaps { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class InvestmentType
    {
        public InvestmentType()
        {
            InvestmentDocument = new HashSet<InvestmentDocument>();
            InvestmentTypeAmountMap = new HashSet<InvestmentTypeAmountMap>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortNote { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public bool IsActive { get; set; }
        public int FinancialYearId { get; set; }
        public byte ClaimType { get; set; }

        public virtual FinancialYear FinancialYear { get; set; }
        public virtual ICollection<InvestmentDocument> InvestmentDocument { get; set; }
        public virtual ICollection<InvestmentTypeAmountMap> InvestmentTypeAmountMap { get; set; }
    }
}

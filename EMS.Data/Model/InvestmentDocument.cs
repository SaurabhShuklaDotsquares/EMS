using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class InvestmentDocument
    {
        public int Id { get; set; }
        public int? Investment { get; set; }
        public int? InvestmentTypeId { get; set; }
        public string DocumentPath { get; set; }
        public string DocumentName { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }

        public virtual Investment InvestmentNavigation { get; set; }
        public virtual InvestmentType InvestmentType { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace EMS.Data.Model
{
    public partial class TdsdeductionDoc
    {
        public int DeductionDocId { get; set; }
        public int EmpDeductionId { get; set; }
        public string FileName { get; set; }
        public string Files { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public bool? IsActive { get; set; }
        public int? Uid { get; set; }
        public virtual TdsempDeduction EmpDeduction { get; set; }
    }
}

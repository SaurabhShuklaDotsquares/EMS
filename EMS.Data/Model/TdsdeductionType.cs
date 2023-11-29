using System;
using System.Collections.Generic;

namespace EMS.Data.Model
{
    public partial class TdsdeductionType
    {
        public TdsdeductionType()
        {
            TdsempDeduction = new HashSet<TdsempDeduction>();
        }

        public int DeductionTypeId { get; set; }
        public int TypeId { get; set; }
        public string DeductionTypeName { get; set; }
        public bool IsActive { get; set; }

        public virtual Tdstype DeductionType { get; set; }
        public virtual ICollection<TdsempDeduction> TdsempDeduction { get; set; }
    }
}

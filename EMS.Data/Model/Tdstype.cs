using System;
using System.Collections.Generic;

namespace EMS.Data.Model
{
    public partial class Tdstype
    {
        public Tdstype()
        {
            TdsempDeduction = new HashSet<TdsempDeduction>();
        }

        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public bool IsActive { get; set; }

        public virtual TdsdeductionType TdsdeductionType { get; set; }
        public virtual ICollection<TdsempDeduction> TdsempDeduction { get; set; }
    }
}

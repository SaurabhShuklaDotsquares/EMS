using System;
using System.Collections.Generic;

namespace EMS.Data.Model
{
    public partial class TdsassesmentYear
    {
        public TdsassesmentYear()
        {
            TdsempDeduction = new HashSet<TdsempDeduction>();
        }

        public int AssesmentYearId { get; set; }
        public string YearRange { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<TdsempDeduction> TdsempDeduction { get; set; }
    }
}

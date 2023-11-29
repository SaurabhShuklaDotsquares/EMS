using System;
using System.Collections.Generic;

namespace EMS.Data.saral
{
    public partial class AttEncash
    {
        public int Empid { get; set; }
        public int Encashslno { get; set; }
        public int Allotid { get; set; }
        public DateTime Uptodate { get; set; }
        public int Monthyear { get; set; }
        public decimal Noofdays { get; set; }
        public decimal? Unitrate { get; set; }
        public decimal Calcamt { get; set; }
        public string Formula { get; set; }
        public int? Addentryid { get; set; }
        public int? Formulaid { get; set; }
        public decimal? Tdsamount { get; set; }
        public DateTime? Tdspaydate { get; set; }
        public byte? Onrtnyn { get; set; }
        public byte Mop { get; set; }
        public string Mopno { get; set; }
        public string Mopbank { get; set; }
        public int? Instid { get; set; }

        public virtual LevAllotment Allot { get; set; }
        public virtual MasEmployee Emp { get; set; }
    }
}

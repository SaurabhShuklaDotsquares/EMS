using System;
using System.Collections.Generic;

namespace EMS.Data.saral
{
    public partial class LevAdjustment
    {
        public int Empid { get; set; }
        public int Empdetid { get; set; }
        public int Attheadid { get; set; }
        public DateTime Asondate { get; set; }
        public int Levid { get; set; }
        public int Monthyear { get; set; }
        public decimal? Allot { get; set; }
        public decimal? Taken { get; set; }
        public string Remarks { get; set; }
    }
}

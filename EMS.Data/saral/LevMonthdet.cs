using System;
using System.Collections.Generic;

namespace EMS.Data.saral
{
    public partial class LevMonthdet
    {
        public int Empid { get; set; }
        public int Empdetid { get; set; }
        public int Monthyear { get; set; }
        public decimal? Nodcalc { get; set; }
        public decimal? Nod { get; set; }
        public decimal? Ndp { get; set; }
        public decimal? Ot1 { get; set; }
        public decimal? Ot2 { get; set; }
        public decimal? Adjhrs { get; set; }
        public byte? Editmodeyn { get; set; }
        public string Remarks { get; set; }
    }
}

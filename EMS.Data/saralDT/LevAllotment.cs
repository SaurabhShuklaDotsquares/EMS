using System;
using System.Collections.Generic;

namespace EMS.Data.saralDT
{
    public partial class LevAllotment
    {
        public int Allotid { get; set; }
        public int Empid { get; set; }
        public int Empdetid { get; set; }
        public int Attheadid { get; set; }
        public int Levid { get; set; }
        public DateTime Allotfrom { get; set; }
        public DateTime Allotto { get; set; }
        public decimal Allotlev { get; set; }
        public decimal Actallotlev { get; set; }
        public decimal Colev { get; set; }
        public decimal Lapse { get; set; }
        public byte Editmodeyn { get; set; }
    }
}

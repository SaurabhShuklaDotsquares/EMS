using System;
using System.Collections.Generic;
using System.Text;
using EMS.Data.saral;

namespace EMS.Dto.SARAL
{
    public class LevAllotmentDto
    {
        public LevAllotmentDto()
        {
            AttEncash = new HashSet<AttEncash>();
        }

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

        public virtual ICollection<AttEncash> AttEncash { get; set; }
    }
}

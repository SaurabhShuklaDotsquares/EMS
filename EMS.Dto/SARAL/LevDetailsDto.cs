using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto.SARAL
{
    public class LevDetailsDto
    {
        public int Empid { get; set; }
        public int Empdetid { get; set; }
        public int Levid { get; set; }
        public DateTime Leavedate { get; set; }
        public byte Firsthalfyn { get; set; }
        public byte Secondhalfyn { get; set; }
        public decimal? Levmonthly { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public byte? Eipyn { get; set; }
    }
}

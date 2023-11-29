using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto.SARAL
{
    public class AttDefinitionDto
    {
        public int Levid { get; set; }
        public string Levname { get; set; }
        public string Levshort { get; set; }
        public byte? Affectsalaryyn { get; set; }
        public decimal? Colimit { get; set; }
        public byte? Leaveyn { get; set; }
        public byte? Encashyn { get; set; }
        public decimal? Encashoffset { get; set; }
        public bool? Allotmentyn { get; set; }
    }
}

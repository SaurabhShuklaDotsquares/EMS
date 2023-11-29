using System;

namespace EMS.Data.Models
{
    public partial class Sme
    {
        public int Id { get; set; }
        public string SubjectMatterExpert { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsActive { get; set; }
        public int? Level1 { get; set; }
        public int? Level2 { get; set; }
        public int? Level3 { get; set; }
        public int? Level4 { get; set; }
        public int? Level5 { get; set; }
    }
}

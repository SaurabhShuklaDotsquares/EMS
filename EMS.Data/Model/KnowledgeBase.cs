using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class KnowledgeBase
    {
        public KnowledgeBase()
        {
            KnowledgeDepartment = new HashSet<KnowledgeDepartment>();
            KnowledgeTech = new HashSet<KnowledgeTech>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Instructions { get; set; }
        public string FilePath { get; set; }
        public int UserId { get; set; }
        public DateTime AddDate { get; set; }
        public int KnowledgeType { get; set; }

        public virtual UserLogin User { get; set; }
        public virtual ICollection<KnowledgeDepartment> KnowledgeDepartment { get; set; }
        public virtual ICollection<KnowledgeTech> KnowledgeTech { get; set; }
    }
}

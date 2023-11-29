using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class KnowledgeTech
    {
        public int Id { get; set; }
        public int CodeId { get; set; }
        public int TechId { get; set; }

        public virtual KnowledgeBase Code { get; set; }
        public virtual Technology Tech { get; set; }
    }
}

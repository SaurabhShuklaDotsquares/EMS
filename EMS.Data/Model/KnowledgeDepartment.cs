using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class KnowledgeDepartment
    {
        public int Id { get; set; }
        public int CodeId { get; set; }
        public int DeptId { get; set; }

        public virtual KnowledgeBase Code { get; set; }
        public virtual Department Dept { get; set; }
    }
}

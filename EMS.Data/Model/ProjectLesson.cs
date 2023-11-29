using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProjectLesson
    {
        public ProjectLesson()
        {
            ProjectLessonLearneds = new HashSet<ProjectLessonLearned>();
        }

        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int CreateByUid { get; set; }
        public DateTime CreateDate { get; set; }
        public int? ModifyByUid { get; set; }
        public DateTime ModifyDate { get; set; }

        public virtual UserLogin UserLogin { get; set; }
        public virtual UserLogin ModifyByU { get; set; }
        public virtual Project Project { get; set; }
        public virtual ICollection<ProjectLessonLearned> ProjectLessonLearneds { get; set; }
    }
}

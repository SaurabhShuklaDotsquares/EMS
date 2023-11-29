using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProjectLessonTopic
    {
        public ProjectLessonTopic()
        {
            ProjectLessonLearned = new HashSet<ProjectLessonLearned>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public byte TopicGroup { get; set; }
        public bool Active { get; set; }

        public virtual ICollection<ProjectLessonLearned> ProjectLessonLearned { get; set; }
    }
}

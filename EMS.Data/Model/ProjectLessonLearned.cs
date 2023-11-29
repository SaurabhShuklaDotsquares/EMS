using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProjectLessonLearned
    {
        public int ProjectLessonId { get; set; }
        public int ProjectLessonTopicId { get; set; }
        public string WhatWentGood { get; set; }
        public string WhatWentBad { get; set; }
        public string WhatLearned { get; set; }
        public string WhatImpacted { get; set; }

        public virtual ProjectLesson ProjectLesson { get; set; }
        public virtual ProjectLessonTopic ProjectLessonTopic { get; set; }
    }
}

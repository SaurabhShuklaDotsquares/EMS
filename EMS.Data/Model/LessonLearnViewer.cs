using System;
using System.Collections.Generic;

namespace EMS.Data.model
{
    public partial class LessonLearnViewer
    {
        public int ProjectLessonId { get; set; }
        public int Uid { get; set; }
        public DateTime AddDate { get; set; }

        public virtual ProjectLesson ProjectLesson { get; set; }
        public virtual UserLogin U { get; set; }
    }
}

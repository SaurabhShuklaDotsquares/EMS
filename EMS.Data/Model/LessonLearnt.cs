using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LessonLearnt
    {
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        public string WhatLearnt { get; set; }
        public int CreatedById { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual UserLogin CreatedBy { get; set; }
        public virtual Project Project { get; set; }
    }
}

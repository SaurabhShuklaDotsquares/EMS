﻿using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class AppraisalExtras
    {
        public int Id { get; set; }
        public int? AppraisalId { get; set; }
        public int? UserId { get; set; }
        public string Comments { get; set; }
        public int? TluserId { get; set; }
        public string Tlcomments { get; set; }
        public DateTime? AddDate { get; set; }
        public string Ip { get; set; }

        public virtual Appraisal Appraisal { get; set; }
    }
}

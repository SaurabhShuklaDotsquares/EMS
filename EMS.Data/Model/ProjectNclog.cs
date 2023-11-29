using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProjectNCLog
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public byte AuditCycle { get; set; }
        public byte AuditType { get; set; }
        public int? ProjectAuditPAId { get; set; }
        public string AuditDesc { get; set; }
        public int AuditorUid { get; set; }
        public DateTime AuditDate { get; set; }
        public int AuditeeUid { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public string RootCause { get; set; }
        public string AuditAction { get; set; }
        public byte Status { get; set; }
        public DateTime? ClosedDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        public virtual UserLogin UserLogin  { get; set; }
        public virtual UserLogin UserLogin1  { get; set; }
        public virtual Project Project { get; set; }
        public virtual ProjectAuditPA ProjectAuditPa { get; set; }
      
    }
}

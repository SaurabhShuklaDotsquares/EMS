using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class TypeMaster
    {
        public TypeMaster()
        {
            ExamQuestionQuestionLevelNavigation = new HashSet<ExamQuestion>();
            ExamQuestionQuestionTypeNavigation = new HashSet<ExamQuestion>();
            LeaveActivityLeaveTypeNavigation = new HashSet<LeaveActivity>();
            LeaveActivityStatusNavigation = new HashSet<LeaveActivity>();
            ProjectDeveloper = new HashSet<ProjectDeveloper>();
            ProjectDeveloperAddonWorkRoleNavigation = new HashSet<ProjectDeveloperAddon>();
            ProjectDeveloperAddonWorkStatusNavigation = new HashSet<ProjectDeveloperAddon>();
            ProjectLead = new HashSet<ProjectLead>();
            ProjectLeadArchive = new HashSet<ProjectLeadArchive>();
        }

        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public string TypeGroup { get; set; }

        public virtual ICollection<ExamQuestion> ExamQuestionQuestionLevelNavigation { get; set; }
        public virtual ICollection<ExamQuestion> ExamQuestionQuestionTypeNavigation { get; set; }
        public virtual ICollection<LeaveActivity> LeaveActivityLeaveTypeNavigation { get; set; }
        public virtual ICollection<LeaveActivity> LeaveActivityStatusNavigation { get; set; }
        public virtual ICollection<ProjectDeveloper> ProjectDeveloper { get; set; }
        public virtual ICollection<ProjectDeveloperAddon> ProjectDeveloperAddonWorkRoleNavigation { get; set; }
        public virtual ICollection<ProjectDeveloperAddon> ProjectDeveloperAddonWorkStatusNavigation { get; set; }
        public virtual ICollection<ProjectLead> ProjectLead { get; set; }
        public virtual ICollection<ProjectLeadArchive> ProjectLeadArchive { get; set; }
    }
}

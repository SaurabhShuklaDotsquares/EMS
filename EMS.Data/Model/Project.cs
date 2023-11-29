using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Project
    {
        public Project()
        {
            DesignerManagement = new HashSet<DesignerManagement>();
            EmployeeProject = new HashSet<EmployeeProject>();
            Escalation = new HashSet<Escalation>();
            Forecasting = new HashSet<Forecasting>();
            ProjectAdditionalSupports = new HashSet<ProjectAdditionalSupport>();
            ProjectClosures = new HashSet<ProjectClosure>();
            Project_Department = new HashSet<Project_Department>();
            ProjectDevelopers = new HashSet<ProjectDeveloper>();
            ProjectDeveloperAddon = new HashSet<ProjectDeveloperAddon>();
            ProjectInvoice = new HashSet<ProjectInvoice>();
            ProjectLesson = new HashSet<ProjectLesson>();
            ProjectNclog = new HashSet<ProjectNCLog>();
            ProjectPm = new HashSet<ProjectPm>();
            Project_Tech = new HashSet<Project_Tech>();
            SubProjects = new HashSet<SubProject>();
            UserActivity = new HashSet<UserActivity>();
            UserActivityLog = new HashSet<UserActivityLog>();
            UserTimeSheet = new HashSet<UserTimeSheet>();
            ProjectOtherPm = new HashSet<ProjectOtherPm>();
            ProjectClientFeedback = new HashSet<ProjectClientFeedback>();
            LessonLearnt = new HashSet<LessonLearnt>();
            UserActivityManageProject = new HashSet<UserActivityManageProject>();
        }

        public int ProjectId { get; set; }
        public string Name { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public bool? IsClosed { get; set; }
        public int CRMProjectId { get; set; }
        public int? Model { get; set; }
        public int? EstimateTime { get; set; }
        public string BillingTeam { get; set; }
        public DateTime? StartDate { get; set; }
        public int? Uid { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public int? ClientId { get; set; }
        public string IP { get; set; }
        public int ActualDevelopers { get; set; }
        public string ProjectDetailsDoc { get; set; }
        public int? PMUid { get; set; }
        public int? AbroadPMUid { get; set; }
        public bool IsInHouse { get; set; }

        public bool? IsCmmi { get; set; }

        public bool? IsManualTimeSheetAllowed { get; set; }

        public virtual UserLogin AbroadPmu { get; set; }
        public virtual Client Client { get; set; }
        public virtual BucketModel BucketModel { get; set; }
        public virtual ICollection<DesignerManagement> DesignerManagement { get; set; }
        public virtual ICollection<EmployeeProject> EmployeeProject { get; set; }
        public virtual ICollection<Escalation> Escalation { get; set; }
        public virtual ICollection<Forecasting> Forecasting { get; set; }
        public virtual ICollection<ProjectAdditionalSupport> ProjectAdditionalSupports { get; set; }
        public virtual ICollection<ProjectClosure> ProjectClosures { get; set; }
        public virtual ICollection<Project_Department> Project_Department { get; set; }
        public virtual ICollection<ProjectDeveloper> ProjectDevelopers { get; set; }
        public virtual ICollection<ProjectDeveloperAddon> ProjectDeveloperAddon { get; set; }
        public virtual ICollection<ProjectInvoice> ProjectInvoice { get; set; }
        public virtual ICollection<ProjectLesson> ProjectLesson { get; set; }
        public virtual ICollection<ProjectNCLog> ProjectNclog { get; set; }
        public virtual ICollection<ProjectPm> ProjectPm { get; set; }
        public virtual ICollection<Project_Tech> Project_Tech { get; set; }
        public virtual ICollection<SubProject> SubProjects { get; set; }
        public virtual ICollection<UserActivity> UserActivity { get; set; }
        public virtual ICollection<UserActivityLog> UserActivityLog { get; set; }
        public virtual ICollection<UserTimeSheet> UserTimeSheet { get; set; }
        public virtual ICollection<ProjectOtherPm> ProjectOtherPm { get; set; }
        public virtual ICollection<ProjectClientFeedback> ProjectClientFeedback { get; set; }
        public virtual ICollection<LessonLearnt> LessonLearnt { get; set; }

        public virtual ICollection<UserActivityManageProject> UserActivityManageProject { get; set; }
    }
}

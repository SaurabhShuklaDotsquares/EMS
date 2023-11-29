using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Department
    {
        public Department()
        {
            CompanyDocument = new HashSet<CompanyDocument>();
            CurrentOpening = new HashSet<CurrentOpening>();
            ForecastingDepartment = new HashSet<ForecastingDepartment>();
            KnowledgeDepartment = new HashSet<KnowledgeDepartment>();
            MomMeetingDepartment = new HashSet<MomMeetingDepartment>();
            OrgDocumentDepartment = new HashSet<OrgDocumentDepartment>();
            ProjectDepartment = new HashSet<Project_Department>();
            UserLogin = new HashSet<UserLogin>();
        }

        public int DeptId { get; set; }
        public string Name { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string IP { get; set; }
        public bool? IsActive { get; set; }
        public string Deptcode { get; set; }

        public virtual ICollection<CompanyDocument> CompanyDocument { get; set; }
        public virtual ICollection<CurrentOpening> CurrentOpening { get; set; }
        public virtual ICollection<ForecastingDepartment> ForecastingDepartment { get; set; }
        public virtual ICollection<KnowledgeDepartment> KnowledgeDepartment { get; set; }
        public virtual ICollection<MomMeetingDepartment> MomMeetingDepartment { get; set; }
        public virtual ICollection<OrgDocumentDepartment> OrgDocumentDepartment { get; set; }
        public virtual ICollection<Project_Department> ProjectDepartment { get; set; }
        public virtual ICollection<UserLogin> UserLogin { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Cvbuilder
    {
        public Cvbuilder()
        {
            CvBuilderEducation = new HashSet<CvBuilderEducation>();
            CvbuilderCertifications = new HashSet<CvbuilderCertifications>();
            CvbuilderCoreCompetencies = new HashSet<CvbuilderCoreCompetencies>();
            CvbuilderPreviousExperience = new HashSet<CvbuilderPreviousExperience>();
            CvbuilderIndustry = new HashSet<CvbuilderIndustry>();
            CvbuilderTechnology = new HashSet<CvbuilderTechnology>();
        }

        public long Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string ProfileSummary { get; set; }
        public string TechnicalSkills { get; set; }
        public string WorkExperience { get; set; }
        public string RolesAcross { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LinkedinId { get; set; }
        public string Languages { get; set; }
        public int ExperienceId { get; set; }
        public string OtherIndustry { get; set; }
        public string OtherTechnology { get; set; }
        public string OtherTechnologyParent { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsTraining { get; set; }
        public bool? IsAgree { get; set; }

        public virtual ICollection<CvbuilderIndustry> CvbuilderIndustry { get; set; }
        public virtual ICollection<CvbuilderTechnology> CvbuilderTechnology { get; set; }
        public virtual UserLogin User { get; set; }
        public virtual ICollection<CvBuilderEducation> CvBuilderEducation { get; set; }
        public virtual ICollection<CvbuilderCertifications> CvbuilderCertifications { get; set; }
        public virtual ICollection<CvbuilderCoreCompetencies> CvbuilderCoreCompetencies { get; set; }
        public virtual ICollection<CvbuilderPreviousExperience> CvbuilderPreviousExperience { get; set; }
    }
}

using EMS.Data.Model;
using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Technology
    {
        public Technology()
        {
            ExamQuestion = new HashSet<ExamQuestion>();
            KnowledgeTech = new HashSet<KnowledgeTech>();
            PortfolioTech = new HashSet<PortfolioTech>();
            ProjectLeadTech = new HashSet<ProjectLeadTech>();
            ProjectLeadTechArchive = new HashSet<ProjectLeadTechArchive>();
            ProjectTech = new HashSet<Project_Tech>();
            UserTech = new HashSet<User_Tech>();
            LibraryTechnology = new HashSet<LibraryTechnology>();
            LibrarySearchTechnology = new HashSet<LibrarySearchTechnology>();
            TechnologyParentMapping = new HashSet<TechnologyParentMapping>();
            CvbuilderTechnology = new HashSet<CvbuilderTechnology>();
            StudyDocuments = new HashSet<StudyDocuments>();
        }
        public int TechId { get; set; }
        public string Title { get; set; }
        public DateTime? AddDate { get; set; }
        public string Alias { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? ModifyDate { get; set; }
        public long? EstimateTechnologyId { get; set; }
        public virtual ICollection<ExamQuestion> ExamQuestion { get; set; }
        public virtual ICollection<KnowledgeTech> KnowledgeTech { get; set; }
        public virtual ICollection<PortfolioTech> PortfolioTech { get; set; }
        public virtual ICollection<ProjectLeadTech> ProjectLeadTech { get; set; }
        public virtual ICollection<ProjectLeadTechArchive> ProjectLeadTechArchive { get; set; }
        public virtual ICollection<Project_Tech> ProjectTech { get; set; }
        public virtual ICollection<User_Tech> UserTech { get; set; }
        public virtual ICollection<LibraryTechnology> LibraryTechnology { get; set; }
        public virtual ICollection<LibrarySearchTechnology> LibrarySearchTechnology { get; set; }
        public virtual ICollection<TechnologyParentMapping> TechnologyParentMapping { get; set; }
        public virtual ICollection<StudyDocuments> StudyDocuments { get; set; }
        public virtual ICollection<CvbuilderTechnology> CvbuilderTechnology { get; set; }
    }
}

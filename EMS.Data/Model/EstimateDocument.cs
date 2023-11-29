using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EstimateDocument
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Tags { get; set; }
        public string DocumentPath { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string Industry { get; set; }
        public string Technology { get; set; }
        public int? Uid_UploadedBy { get; set; }
        public string MockupDocument { get; set; }
        public string OtherDocument { get; set; }
        public string Flowcharts { get; set; }
        public string Wireframe_MockupsDoc { get; set; }
        public int? LeadId { get; set; }
        public int? EstimateTimeInDays { get; set; }
        public bool? IsSpam { get; set; }
        public bool? IsDSPhoto { get; set; }

        public virtual ProjectLead Lead { get; set; }
        public virtual UserLogin UserLogin { get; set; }
        public virtual ICollection<EstimateDocumentIndustry> EstimateDocumentIndustry { get; set; }
        public bool IsDelete { get; set; }
    }
}


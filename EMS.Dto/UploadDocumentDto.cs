using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EMS.Dto
{
    public class UploadDocumentDto
    {
        public int EstimateDocumentId { get; set; }
        public string Title { get; set; }
        public string Tags { get; set; }



        public string[] Domains { get; set; }
        public string Industry { get; set; }

        public string Technology { get; set; }
        [DisplayName("Proposal Document")]
        public string EstimateDocPath { get; set; }

        [DisplayName("Wireframe/ Mockup (Images)/ DS Photos")]
        public string WireformMockupDoc { get; set; }

        [DisplayName("Other Document")]
        public string OtherDoc { get; set; }
        [DisplayName("Flowcharts")]
        public string Flowcharts { get; set; }

        [DisplayName("Zip file (Wireframe/ Mockup)")]
        public string MockupDoc { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }

        [DisplayName("Estimated Time (in Days)")]
        public int? EstimateTimeinDays { get; set; }

        [DisplayName("Exclude From Library")]
        public bool IsSpam { get; set; }

        [DisplayName("Dotsquares Photos")]
        public bool IsDSPhoto { get; set; }
    }
    public class WireframeMockupsImage
    {

        public string Title { get; set; }
        public string Tags { get; set; }
        public int? id { get; set; }

        public int? Uid { get; set; }
        public string Industry { get; set; }
        public string UploadedBy { get; set; }
        public string Technology { get; set; }
        [DisplayName("Wireframe/ Mockup (Images)")]
        public string WireformMockupDoc { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }

        [DisplayName("Estimated Time (in Days)")]
        public string EstimateTimeinDays { get; set; }


        [DisplayName("Lead Id")]
        public int LeadId { get; set; }
    }
}

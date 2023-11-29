using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{
    public class LeadDto
    {
        [DisplayName("Lead Id")]
        public int LeadId { get; set; }

        [DisplayName("Client Id")]
        public int? LeadClientId { get; set; }

        [DisplayName("Estimate Owner")]
        public int EstimateOwnerId { get; set; }

        [DisplayName("Communicator Owner")]
        public int CommunicatorOwnerId { get; set; }

        //public string Technologies { get; set; }

        //[DisplayName("Generated Date")]
        [DisplayName("Assigned Date")]
        public string AssignedDate { get; set; }

        public int Status { get; set; }

        [DisplayName("Requirement Document")]
        public string InitialReqDoc { get; set; }
        public string OtherDoc { get; set; }
        public string WireframeMockupsImages { get; set; }
        public string WireframeMockupsZip { get; set; }
        public string Notes { get; set; }

        [DisplayName("Lead Type")]
        public int LeadType { get; set; }
        public string Title { get; set; }

        [DisplayName("Chase Request Type:")]
        public int ChaseRequest { get; set; }

        [DisplayName("Client Type")]
        public bool IsNewClient { get; set; }

        [DisplayName("Estimate Time (in Days)")]
        public int? EstimateTimeInDays { get; set; }

        [DisplayName("Status Updated On:")]
        public string StatusUpdateDate { get; set; }

        public string Tag { get; set; }
        public int PMUid { get; set; }

        [DisplayName("Lead CRMID")]
        public string LeadCRMId { get; set; }

        [DisplayName("Project Tag")]
        public string ProjectTag { get; set; }

        [DisplayName("Technology Other")]
        public string TechnologyOther { get; set; }

        [DisplayName("Proposal Docuemnt")]
        public IFormFile FileRequirmentDoc { get; set; }
        public IFormFile FileProposalDoc { get; set; }
        public IFormFile FileWireframeMockupsImg { get; set; }
        public IFormFile FileOtherDoc { get; set; }
        public IFormFile FileWireframeMockupsZip { get; set; }
        public string[] Technology { get; set; }
        public string[] Technician { get; set; }

        public string ProposalDoc { get; set; }

        [DisplayName("Lead From")]
        public int AbroadPMId { get; set; }
        public int TotalClients { get; set; }
        public int NewClients { get; set; }
        public int ExistingClient { get; set; }
        public int TotalConvertedClients { get; set; }
        public int ConvertedNewClients { get; set; }
        public int ConvertedExistingClients { get; set; }
        public int EscalatedLeads { get; set; }
        public int AwaitingResponse { get; set; }
        public string TotalConversion { get; set; }
        public string Remark { get; set; }
        public string ClientName { get; set; }
        public DateTime AddDate { get; set; }
        public bool IsCovid19 { get; set; }

        public bool IsAlmostConverted { get; set; } = false;
        [DisplayName("Conversion Date")]
        public string ConversionDate { get; set; }

        public string[] Domains { get; set; }
        public string Industry { get; set; }
        public List<SelectListItem> TechnicianList { get; set; }
        public List<SelectListItem> TakenTechnician { get; set; }
        public List<SelectListItem> TechnologyList { get; set; }
        public List<SelectListItem> TakenTechnologies { get; set; }
        public List<SelectListItem> EstimateOwnerList;
        public List<SelectListItem> OwnerList;
        public List<SelectListItem> CommunicatorOwnerList;
        public List<SelectListItem> StatusList;
        public List<SelectListItem> TypeList;
        public List<LeadDetails> lsLeadDetail;
        public List<SelectListItem> LeadFromList;
        public LeadDto()
        {
            List<LeadDetails> lsLeadDetail = new List<LeadDetails>();
            EstimateOwnerList = new List<SelectListItem>();
            StatusList = new List<SelectListItem>();
            TypeList = new List<SelectListItem>();
            LeadFromList = new List<SelectListItem>();
            CommunicatorOwnerList = new List<SelectListItem>();
            OwnerList = new List<SelectListItem>();
            TechnicianList = new List<SelectListItem>();
            TakenTechnician = new List<SelectListItem>();
            TechnologyList = new List<SelectListItem>();
            TakenTechnologies = new List<SelectListItem>();

        }

    }

    public class LeadDetails
    {
        public int index { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        public Int32 TotalEstimate { get; set; }
        public int TotalNew { get; set; }
        public int TotalExisting { get; set; }
        public Int32 TotalConversion { get; set; }
        public Int32 NewConversion { get; set; }
        public Int32 ExistingConversion { get; set; }
    }
    public class LeadClientDto
    {

        public string Name { get; set; }
        public string Email { get; set; }
        public int LeadId { get; set; }
        [DisplayName("Select Client")]
        public int ClientId { get; set; }
        public List<SelectListItem> LeadClientList;

        public LeadClientDto()
        {
            LeadClientList = new List<SelectListItem>();
        }
    }
    public class ConclusionDto
    {
        [DisplayName("Lead ID")]
        public int LeadId { get; set; }
        [DisplayName("Owner")]
        public string Owner { get; set; }
        [DisplayName("Communicator")]
        public string Communicator { get; set; }
        [DisplayName("Conclusion")]
        public string Conclusion { get; set; }


        [DisplayName("Reason")]
        public string ChildStatus { get; set; }
        [DisplayName("Reason")]
        public List<string> arrChildStatus { get; set; }
        [DisplayName("Reason")]
        public List<string> arrSelectChildStatus { get; set; }



        [DisplayName("Flag")]
        public int Status { get; set; }
        [DisplayName("Date")]
        public string StatusUpdateDate { get; set; }
        public List<SelectListItem> StatusList;
        public ConclusionDto()
        {
            StatusList = new List<SelectListItem>();
        }
    }

    public class LeadStatusDto
    {
        [DisplayName("Lead ID")]
        public int LeadId { get; set; }
        public int LeadType { get; set; }
        public bool IsAlmostConverted { get; set; } = false;
        [DisplayName("Document")]
        public string Document { get; set; }
        [DisplayName("Status")]
        public int Status { get; set; }
        [DisplayName("Next Chase Date")]
        public string NextChaseDate { get; set; }
        [DisplayName("Conversion Date")]
        public string ConversionDate { get; set; }

        [DisplayName("Notes")]
        public string Notes { get; set; }

        public List<SelectListItem> StatusList;
        public LeadStatusDto()
        {
            StatusList = new List<SelectListItem>();
        }

        public bool IsConfirmSubmit { get; set; } = false;
    }
}

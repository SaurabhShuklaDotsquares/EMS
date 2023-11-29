using EMS.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{
    public class InvoiceDto : InvoiceFilterDto
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string BAName { get; set; }
        public string TLName { get; set; }
        public string ClientName { get; set; }
        public string Status { get; set; }
        public int ProjectId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceStartDate { get; set; }
        public string StartDate { get; set; }
        public DateTime InvoiceEndDate { get; set; }
        public string EndDate { get; set; }
        public string InvoiceAmount { get; set; }
        public string Amount { get; set; }
        public int InvoiceStatusId { get; set; }
        public int? Uid_BA { get; set; }
        public int? Uid_TL { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }
        public int CurrencyID { get; set; }
        public string CurrencySign { get; set; }
        public int CountryId { get; set; }
        public int PMID { get; set; }
        public string Comment { get; set; }

        public string Technologies { get; set; }
        public List<InvoiceComment> ProjectInvoiceComments { get; set; }
        public List<SelectListItem> ProjectList { get; set; }
        public List<SelectListItem> InvoiceStatus { get; set; }
        public List<SelectListItem> CurrencyList { get; set; }
        public List<SelectListItem> ClientCountry { get; set; }
        public InvoiceDto()
        {
            ProjectInvoiceComments = new List<InvoiceComment>();
            ProjectList = new List<SelectListItem>();
            InvoiceStatus = new List<SelectListItem>();
            CurrencyList = new List<SelectListItem>();
            ClientCountry = new List<SelectListItem>();
        }
    }

    public class InvoiceStatusDto
    {
        public int InvoiceId { get; set; }

        [DisplayName("Project Name")]
        public string ProjectName { get; set; }

        [DisplayName("Current Status")]
        public string CurrentStatus { get; set; }

        [DisplayName("Partial Amount")]
        public string RemainingAmount { get; set; }

        [DisplayName("Change Status")]
        public int InvoiceStatusId { get; set; }
        public List<SelectListItem> InvoiceStatus { get; set; }

        public InvoiceStatusDto()
        {
            InvoiceStatus = new List<SelectListItem>();
        }
    }
    public class InvoiceComment
    {
        public string Comment { get; set; }
        public string CommentDate { get; set; }
    }

    public class InvoiceChaseDto
    {
        public int InvoiceId { get; set; }
        public string ChaseDate { get; set; }
        public string Comment { get; set; }

        public int CurrentUserId { get; set; }
    }

    public class InvoiceFilterDto
    {
        public List<SelectListItem> BAList { get; set; }
        public List<SelectListItem> TLList { get; set; }
        public InvoiceFilterDto()
        {
            BAList = new List<SelectListItem>();
            TLList = new List<SelectListItem>();
        }
    }
}

using EMS.Core;
using EMS.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;


namespace EMS.Dto
{
    public class ProjectActualHoursDto
    {
        public List<SelectListItem> Projects { get; set; }
        public List<SelectListItem> ProjectStatusList { get; set; }
        public string ProjectId { get; set; }
        public string ProjectStatus { get; set; }
        //public List<SelectListItem> Employees { get; set; }
        public string Uid { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }


    }

    public class ProjectActualHoursDetailsDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public Project ProjectInfo { get; set; }
        public List<InvoiceDetails> invoiceDetails { get; set; }
        //public List<ProjectInvoice> Invoices { get; set; }
        //public List<UserTimeSheet> TimeSheets { get; set; }
    }

    public class InvoiceDetails
    {
        public string InvoicePlanHours { get; set; }
        public double dInvoicePlanHours { get; set; }
        public string InvoiceActualHours { get; set; }
        public double dInvoiceActualHours { get; set; }
        public ProjectInvoice Invoice { get; set; }
        public List<UserTimeSheet> TimeSheets { get; set; }
    }
}

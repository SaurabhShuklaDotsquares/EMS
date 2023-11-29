using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using static EMS.Core.Enums;

namespace EMS.Dto
{
    public class ExpenseDto
    {
        public ExpenseDto()
        {
            ExpensePaymentThroughList = new List<SelectListItem>();
            CurrencyList = new List<SelectListItem>();
        }

        public int Id { get; set; }

        [DisplayName("Description")]
        public string Descp { get; set; }

        [DisplayName("Expense Date")]
        public string ExpenseDate { get; set; }
        public string Amount { get; set; }

        [DisplayName("Receipt")]
        public string ReceiptPath { get; set; }

        [DisplayName("Paid Through")]
        public byte PaidThrough { get; set; }
        public byte Status { get; set; }

        public int CurrencyId { get; set; }
        public int CurrentUserId { get; set; }

        [DisplayName("Requested By")]
        public string CreatedByUser { get; set; }

        [DisplayName("Payment Date")]
        public string ReimburseDate { get; set; }

        [DisplayName("Receipt")]
        public IFormFile Receipt { get; set; }
        public List<SelectListItem> ExpensePaymentThroughList { get; set; }
        public List<SelectListItem> CurrencyList { get; set; }
        public bool HasReceipt { get; set; }
    }

    public class ExpenseApproveDto
    {
        public int[] ExpenseIds { get; set; }
        public byte Status { get; set; }
        public int CurrentUserId { get; set; }

        public bool IsApproved { get; set; }
    }

    public class ExpenseIndexDto
    {
        public ExpenseIndexDto()
        {
            UserList = new List<SelectListItem>();
            StatusList = new List<SelectListItem>();
        }

        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public bool IsApprover { get; set; }
        public List<SelectListItem> UserList { get; set; }
        public List<SelectListItem> StatusList { get; set; }

    }

    public class ExpenseSummary
    {
        public ExpenseSummary()
        {
            CompanyCardSummary = new List<string>();
            CashOrPersonalCardSummary = new List<string>();
            TotalSummary = new List<string>();
        }

        public string Status { get; set; }

        public List<string> CompanyCardSummary { get; set; }
        public List<string> CashOrPersonalCardSummary { get; set; }
        public List<string> TotalSummary { get; set; }


    }
}

using EMS.Data;
using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace EMS.Dto
{
    public class InvestmentDto
    {
        public InvestmentDto()
        {
            InvestmentTypeAmountMaps = new List<InvestmentTypeAmountMapDto>();
            IncomeTypeAmountMaps = new List<InvestmentTypeAmountMapDto>();
            InvestmentMonths = new List<InvestmentMonthDto>();
            DocumentTypeList = new List<SelectListItem>();
            InvestmentDocuments = new List<InvestmentDocmentDto>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string DOB { get; set; }
        public string HomeAddress { get; set; }
        public string AttendanceCode { get; set; }
        public string PAN { get; set; }
        public string FatherName { get; set; }
        public int FinancialYearId { get; set; }
        public string FinancialYear { get; set; }
        public bool IsDraft { get; set; }
        public string SubmitionDate { get; set; }

        public int CurrentUserId { get; set; }

        public List<InvestmentTypeAmountMapDto> InvestmentTypeAmountMaps { get; set; }
        public List<InvestmentMonthDto> InvestmentMonths { get; set; }
        public List<InvestmentDocmentDto> InvestmentDocuments { get; set; }

        public List<InvestmentTypeAmountMapDto> IncomeTypeAmountMaps { get; set; }

        public List<SelectListItem> DocumentTypeList { get; set; } 
    }

    public class InvestmentTypeAmountMapDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortNote { get; set; }
        public decimal? Amount { get; set; }
        public int InvestmentTypeId { get; set; }
        public byte ClaimType { get; set; }
    }

    public class InvestmentMonthDto
    {
        public string MonthName { get; set; }
        public decimal? MonthlyRent { get; set; }
        public int InvYear { get; set; }
        public int InvMonth { get; set; }    
    }

    public class InvestmentDocmentDto
    {
        public int Id { get; set; }
        public int DocumentTypeId { get; set; }
        public IFormFile Document { get; set; }
        public string DocumentUrl { get; set; }
        public string DocumentName { get; set; }
    }

    public class InvestmentIndexDto
    {
        public bool ShowAddNewOption { get; set; }
        public List<SelectListItem> FinancialYearList { get; set; }
        public List<SelectListItem> PMUserList { get; set; }

        public InvestmentIndexDto()
        {
            FinancialYearList = new List<SelectListItem>();
            PMUserList = new List<SelectListItem>();
        }
    }
}

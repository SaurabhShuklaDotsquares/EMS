using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Web.Models
{
    public class SpecialSearchFilterViewModel
    {
        public int? user { get; set; }
        public int? status { get; set; }
        public int? leavetype { get; set; }
        public int? pm { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public int? leavecatagory { get; set; }
       
    }

    public class WFHSpecialSearchFilterViewModel
    {
        public int? user { get; set; }
        public int? status { get; set; }       
        public int? pm { get; set; }
        public string  pmname { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }        
        public int? WFHcatagory { get; set; }
        //public string Comment { get; set; }
        //public string AnyComment { get; set; }
    }

    public class SpacialSearch
    {
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
        public int? search_text { get; set; }
        public int? EmployeeId { get; set; }
    }
}
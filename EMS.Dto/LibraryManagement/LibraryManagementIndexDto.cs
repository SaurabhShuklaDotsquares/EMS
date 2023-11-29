using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class LibraryManagementIndexDto
    {
        public LibraryManagementIndexDto()
        {
            //Libraries = new List<SelectListItem>();
            Industries = new List<SelectListItem>();
            Layouts = new List<SelectListItem>();
            Technologies = new List<SelectListItem>();
            //Featured = new List<SelectListItem>();
            //IsNda = new List<SelectListItem>();
            LibraryTypes = new List<SelectListItem>();
            Layouts = new List<SelectListItem>();
            //FileTypes = new List<SelectListItem>();
            Templates = new List<SelectListItem>();

            SalesKitTypes = new List<SelectListItem>();
            CvsTypes = new List<SelectListItem>();

        }

        
        public List<SelectListItem> SalesKitTypes { get; set; }
      
        public List<SelectListItem> CvsTypes { get; set; }

        public byte? SalesKitId { get; set; }
        public byte? CvsId { get; set; }

        //public List<SelectListItem> Libraries { get; set; }
        public List<SelectListItem> Industries { get; set; }
        public List<SelectListItem> Technologies { get; set; }
        public string Featured { get; set; }
        public string IsReadyToUse { get; set; }
        public string IsNda { get; set; }
        public List<SelectListItem> LibraryTypes { get; set; }
        public List<SelectListItem> DesignTypes { get; set; }
        public bool AdvanceSearch { get; set; }
        public string Keyword { get; set; }
        public byte LibraryTypeId { get; set; }
        public byte? DesignTypeId { get; set; }

        public Guid? KeyId { get; set; }
        public List<SelectListItem> Layouts { get; set; }
        public List<SelectListItem> Components { get; set; }
        //public List<SelectListItem> FileTypes { get; set; }
        public LibraryManagementDto libraryManagementDto { get; set; }
        public List<SelectListItem> Templates { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Web
{
    public class EstimateViewModel
    {
        public EstimateViewModel()
        {
            //bind default client types
            ClientTypes = new List<SelectListItem>() {
                new SelectListItem() {Text="New",Value="1" },
                new SelectListItem() {Text="Existing",Value="0" }
            };
        }

        public string AssignedFrom { get; set; }
        public string AssignedTo { get; set; }

        public List<SelectListItem> LeadOwners { get; set; }
        public List<SelectListItem> LeadStatus { get; set; }
        public List<SelectListItem> LeadTypes { get; set; }
        public List<SelectListItem> ClientTypes { get; private set; }
    }
}

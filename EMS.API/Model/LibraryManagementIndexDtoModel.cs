using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.API.Model
{
    public class LibraryManagementIndexDtoModel
    {
        public Guid? KeyId { get; set; }
        public List<SelectListItem> Industries { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Technologies { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> LibraryTypes { get; set; } = new List<SelectListItem>();
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EMS.Dto
{
    public class DomainTypeDto
    {
        public int DomainId { get; set; }
        [DisplayName("Domain Name")]
        public string DomainName { get; set; }
        public string AddDate { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }
        public string Alias { get; set; }
        public string Ip { get; set; }
    }
}

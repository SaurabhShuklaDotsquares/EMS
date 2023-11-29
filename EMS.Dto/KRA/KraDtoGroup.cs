using EMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMS.Dto.KRA
{
    public class KraDtoGroup
    {
        public List<Designation> DesignationList { get; set; }
        public List<Kra> KRAList { get; set; }
        public string Name { get; set; }
        public string DesignationName { get; set; }
        public string GroupId { get; set; }
        public string ParentGroupId { get; set; }
    }
}

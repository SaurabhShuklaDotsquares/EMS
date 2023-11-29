using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Project_Department
    {
        public int Id { get; set; }
        public int ProjectID { get; set; }
        public int DeptID { get; set; }

        public virtual Department Department { get; set; }
        public virtual Project Project { get; set; }
    }
}

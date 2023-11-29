using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.API.Model
{
    public class DeactivateUserModel
    {
        public int HrmId { get; set; }
        public string empCode { get; set; }
        public bool Active { get; set; }
        public string EmpStatus { get; set; }
        public string ResignationDate { get; set; }
        public string RelievingDate { get; set; }
        public bool isAbscond { get; set; }
        public bool isTerminate { get; set; }
    }
}

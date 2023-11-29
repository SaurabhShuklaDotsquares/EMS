using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Dto
{
    public class BucketModelDto
    {
        public int BucketId { get; set; }
        [DisplayName("Bucket Model Name")]
        public string ModelName { get; set; }
        public string AddDate { get; set; }
        public string ModifyDate { get; set; }
        [DisplayName("Is Active")]
        public bool IsActive { get; set; }
        public string IP { get; set; }
        [DisplayName("Bucket Model Code")]
        public string ModelCode { get; set; }
    }
}

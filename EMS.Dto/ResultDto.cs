using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
namespace EMS.Dto
{
    public class ResultDto
    {
        public int project_id { get; set; }
        public string project_name { get; set; }
        public string modelname { get; set; }
        public string project_status { get; set; }
        public string start_date { get; set; }
        public string estimate_time { get; set; }
        public string bill_team { get; set; }
        public object dev_detail { get; set; }
        public int developer { get; set; }
        public string primary_technology { get; set; }
        public int totalDeveloper { get; set; }
    }
}

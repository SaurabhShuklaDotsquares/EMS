using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.API.Model
{

    public class ProjectInfoReqModel
    {
        public string project_id { get; set; }
        public string project_name { get; set; }
        public string tm_email { get; set; }
        public string pm_email { get; set; }
        public string modelname { get; set; }
        public string project_status { get; set; }
        public string start_date { get; set; }
        public string estimate_time { get; set; }
        public string bill_team { get; set; }
        public string client_name { get; set; }
        public string developer { get; set; }
        public string primary_technology { get; set; }
        public List<other_contact> other_contact { get; set; }
        public List<dev_detail> dev_detail { get; set; }
    }

    public class other_contact
    {
        public string email { get; set; }
    }

    public class ProjectCMMIActive
    {
        public int ProjectCRMId{ get; set; }
        public string IsCMMI { get; set; }
    }

    public class ProjectInfoModel 
    {

        public string project_id { get; set; }
        public string EmsCRMPId { get; set; }
        public string project_name { get; set; }
        public string modelname { get; set; }
        public string project_status { get; set; }
        public string developer { get; set; }
        public bool IsNew { get; set; }
        public bool IsRunDev { get; set; }
        public int WorkingDevelopers { get; set; }
        public DateTime Modifiydate { get; set; }
        public string bill_team { get; set; }
        public string estimate_time { get; set; }
        public DateTime start_date { get; set; }
        public string primary_technology { get; set; }

        public string other_contact { get; set; }


        public List<dev_detail> dev_detail { get; set; }
        public string pdev_detail { get; set; }
        public string virtual_devId { get; set; }
        public string actual_devId { get; set; }
        public bool C_Status { get; set; }
        public string client_name { get; set; }
        public List<proj_devdetail> projectDevelopers { get; set; }
        public string pm_email { get; set; }



        public int? PMUId { get; set; }
        public bool IsRemovedOtherPm { get; set; }
        public string tm_email { get; set; } 



    public List<ProjectOtherContactPM> ProjectOtherContactPMList { get; set; }


    }

    public class proj_devdetail
    {
        public int ProjectDeveloperID { get; set; }
        public int WorkStatus { get; set; }
        public int? VD_id { get; set; }
        public int? AD_id { get; set; }
    }

    public class dev_detail
    {
        [JsonProperty("virtual")]
        public vdevlopersname virtual_dev { get; set; }
        [JsonProperty("actual")]
        public adevlopersname actual { get; set; }
    }
    public class vdevlopersname
    {
        [JsonProperty("username")]
        public string username { get; set; }
        [JsonProperty("email")]
        public string email { get; set; }
    }
    public class adevlopersname
    {
        [JsonProperty("username")]
        public string username { get; set; }
        [JsonProperty("email")]
        public string email { get; set; }

    }

    public class ProjectOtherContactPM
    {
        public int ProjectId { get; set; }
        public int PMUid { get; set; }

    }
    public class ProjectData
    {
        public int EMSProjectID { get; set; }
    }

}

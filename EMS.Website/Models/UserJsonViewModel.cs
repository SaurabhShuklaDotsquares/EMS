using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Web.Models
{
    public class UserJsonViewModel
    {
        public string name { get; set; }

        public int maxLimit { get; set; }
        public List<UserJsonViewModel> children { get; set; }
    }

    public class UserJsonModelForDeveloperInfo
    {
        public UserJsonModelForDeveloperInfo()
        {
            data = new List<UserTechnologyInfo>();
        }

        public string email_address { get; set; }
        public string hrm_id { get; set; }
        public List<UserTechnologyInfo> data { get; set; }
    }

    public class UserTechnologyInfo
    {
        public string technology_ems_id { get; set; }
        public string technology_ems_name { get; set; }
        public string specialization { get; set; }
    }

    public class UserTechInfoInCRMModel
    {
        public UserTechInfoInCRMModel()
        {
            data = new List<UserTechInfoInCRM>();
        }

        public List<UserTechInfoInCRM> data { get; set; }
        public int Uid { get; set; }
    }

    public class UserTechInfoInCRM
    {
        public int Uid { get; set; }
        public string UserName { get; set; }
        public string EmailOffice { get; set; }
        public string UserPMName { get; set; }
        public bool HasTechInfo { get; set; }
        public string ResultFromCRM { get; set; }
    }
}
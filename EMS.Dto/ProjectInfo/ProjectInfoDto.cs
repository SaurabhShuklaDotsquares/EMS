using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Dto
{

    public class ProjectsUpdateDto
    {
        public int PmUid { get; set; }

        public string PmUidEncrypted { get; set; }

        public List<ProjectInfoDto> ProjectsListInfo { get; set; }

        public ProjectsUpdateDto()
        {
            ProjectsListInfo = new List<ProjectInfoDto>();
        }
    }

    public class ProjectInfoDto
    {

        public string Project_Id { get; set; }
        public string EmsCRMPId { get; set; }
        public string Project_Name { get; set; }
        public string ModelName { get; set; }
        public string Project_Status { get; set; }
        public string Developer { get; set; }
        public bool IsNew { get; set; }
        public bool IsRunDev { get; set; }
        public int WorkingDevelopers { get; set; }
        public DateTime Modifiydate { get; set; }
        public string Bill_Team { get; set; }
        public string Estimate_Time { get; set; }
        public DateTime Start_Date { get; set; }
        public string Primary_Technology { get; set; }
        public List<Dev_Detail> Dev_Detail { get; set; }
        public string Pdev_Detail { get; set; }
        public string Virtual_DevId { get; set; }
        public string Actual_DevId { get; set; }
        public bool C_Status { get; set; }
        public string Client_Name { get; set; }
        public List<Proj_DevDetail> ProjectDevelopers { get; set; }
        public string Pm_Email { get; set; }
        public string CommandName { get; set; }
        public bool RecordCheckbox { get; set; }
        public string other_contact { get; set; }  

        public int? PMUId { get; set; }
        public bool IsRemovedOtherPm { get; set; }

        public List<ProjectOtherContactPM> ProjectOtherContactPMList { get; set; }
    }

    public class Proj_DevDetail
    {
        public int ProjectDeveloperID { get; set; }
        public int WorkStatus { get; set; }
        public int? VD_Id { get; set; }
        public int? AD_Id { get; set; }
    }

    public class Dev_Detail
    {
        public VDevlopersName Virtual_Dev { get; set; }
        public AdevlopersName Actual { get; set; }

    }

    public class VDevlopersName
    {
        public string UserName { get; set; }
        public string Email { get; set; }
    }

    public class AdevlopersName
    {
        public string UserName { get; set; }
        public string Email { get; set; }

    }

    public class DeveloperDetailIDs
    {
        public int VirtualDeveloperID { get; set; }
        public int? ActualDeveloperID { get; set; }
    }

    public class ProjectOtherContactPM
    {
        public int ProjectId { get; set; }
        public int PMUid { get; set; }

    }
}

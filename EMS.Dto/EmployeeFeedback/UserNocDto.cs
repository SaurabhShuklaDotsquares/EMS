using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto.EmployeeFeedback
{
    public class UserNocDto
    {
        public int Id { get; set; }
        public int Uid { get; set; }
        public string Name { get; set; }
        public string EmpCode { get; set; }
        public string EmailOffice { get; set; }
        public string PMName { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public List<NocMasterDto> UserNocList { get; set; }
        public bool? IsFeedbackEmailSent { get; set; }
        public bool? IsFeedbackReceived { get; set; }
        public bool? IsVoluntaryExit { get; set; }
        public string VoluntaryComment { get; set; }
        public bool? IsEligibleForRehire { get; set; }
        public string RehireComment { get; set; }
        public bool EmailSkypePassReset { get; set; }
        public bool LFProfile { get; set; }
        public bool? IsIdCardSubmitted { get; set; }
        public bool? ReleaseDocPrepared { get; set; }

        public DateTime? ResignationDate { get; set; }
        public bool? IsLfprofileResetted { get; set; }
        public bool? IsExitFormalitiesCompleted { get; set; }
        public DateTime? RelieveDate { get; set; }
    }

    public class NocMasterDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool? Value { get; set; }
        public  bool? IsClear { get; set; }
    }
}

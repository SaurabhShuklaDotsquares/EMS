using System;
using System.Collections.Generic;

namespace EMS.Data.Model
{
    public partial class UserExitProcess
    {
        public int Id { get; set; }
        public int EmpUid { get; set; }
        public bool? IsFeedbackEmailSent { get; set; }
        public bool? IsFeedbackReceived { get; set; }
        public bool? IsVoluntaryExit { get; set; }
        public string VoluntaryComment { get; set; }
        public bool? IsEligibleForRehire { get; set; }
        public string RehireComment { get; set; }
        public bool? IsIdCardSubmitted { get; set; }
        public bool? ReleaseDocPrepared { get; set; }
        public bool? IsExitFormalitiesCompleted { get; set; }
        public bool? IsLfprofileResetted { get; set; }
        public bool? IsEmailSkypePassResetted { get; set; }
        public int? EmpPmuid { get; set; }
        public virtual UserLogin EmpU { get; set; }
    }
}

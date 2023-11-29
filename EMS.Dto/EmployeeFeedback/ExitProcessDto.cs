using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto.EmployeeFeedback
{
    public class ExitProcessDto
    {
        public int Id { get; set; }
        public int Uid { get; set; }
        public bool IsFeedbackEmailSent { get; set; }
        public bool IsFeedbackReceived { get; set; }
        public bool IsVoluntaryExit { get; set; }
        public string VoluntaryComment { get; set; }
        public bool IsEligibleForRehire { get; set; }
        public string RehireComment { get; set; }
        public bool? IsIdCardSubmitted { get; set; }
        public bool? ReleaseDocPrepared { get; set; }
        public bool? IsExitFormalitiesCompleted { get; set; }
        public DateTime? ResignationDate { get; set; }
        public DateTime? RelieveDate { get; set; }
    }
}

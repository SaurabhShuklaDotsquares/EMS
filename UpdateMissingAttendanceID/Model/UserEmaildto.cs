using System.Collections.Generic;

namespace UpdateMissingAttendanceID.Model
{
    public class HRMRequestDataModel
    {
        public string email { get; set; }
    }

    public class HRMResponseDataModel
    {
        public HRMResponseDataModel()
        {
            data = new List<HRMAttendanceReferenceId>();
        }

        public string Status { get; set; }

        public string Message { get; set; }

        public List<HRMAttendanceReferenceId> data { get; set; }
    }

    public class HRMAttendanceReferenceId
    {
        public string email { get; set; }

        public string attendance_ref_no { get; set; }
    }

    public class ErroredEmails
    {
        public string email { get; set; }

        public string ErrorMessage { get; set; }
    }
}

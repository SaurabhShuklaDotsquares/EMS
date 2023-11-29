using System;
using System.Collections.Generic;
using System.Linq;

using EMS.Data;
using UpdateMissingAttendanceID.Model;

namespace UpdateMissingAttendanceID.DAL
{
    public static class MissingAttendaceDAL
    {
        static db_dsmanagementnewContext db = new db_dsmanagementnewContext();

        public static List<HRMRequestDataModel> GetUsersForMissingAttendanceScheduler()
        {
            var logins = db.UserLogin.Where(UL =>
                                            (UL.AttendenceId == null || UL.AttendenceId == 0)
                                            && UL.IsActive.Value == true
                                             && UL.PMUid != 631
                                            //&& (UL.PMUid != 631 || !string.IsNullOrEmpty(Convert.ToString(UL.PMUid)))

                                            && UL.DeptId != 13
                                            && !string.IsNullOrEmpty(UL.EmailOffice)
                                            );

            if (logins != null)
            {
                return logins.Select(p => new HRMRequestDataModel
                {
                    email = p.EmailOffice
                }).OrderBy(UL => UL.email).ToList();

            }

            List<HRMRequestDataModel> clankList = new List<HRMRequestDataModel>();
            return clankList;
        }

        public static List<ErroredEmails> UpdateAttendancereferenceId(List<HRMAttendanceReferenceId> data)
        {
            var erroredEmail = new List<ErroredEmails>();

            foreach (var email in data.Where(a => a.attendance_ref_no != null))
            {
                try
                {
                    var login = db.UserLogin.Where(UL => UL.EmailOffice.Equals(email.email, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                    if (login != null)
                    {
                        login.AttendenceId = Convert.ToInt32(email.attendance_ref_no);
                    }

                    db.UserLogin.Update(login);

                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    erroredEmail.Add(new ErroredEmails()
                    {
                        email = email.email,
                        ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message
                    });
                }
            }

            return erroredEmail;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EMS.Data;
using Microsoft.EntityFrameworkCore;

namespace TimesheetReminderSchedular.DAL
{
    public static class TimeSheetDAL
    {
        static db_dsmanagementnewContext db = new db_dsmanagementnewContext();
        public static List<UserSchedulerDto> GetUsersForTimeSheetSchedular()
        {
            var logins = db.UserLogin.AsEnumerable();
            return logins.Where(UL => UL.IsActive == true && UL.RoleId != 5 && UL.RoleId != 4 && UL.RoleId != 7 && UL.RoleId != 14 && UL.UserTimeSheets1.Any()).Select(p => new UserSchedulerDto
            {
                Uid = p.Uid,
                TimeSheetDate = p.UserTimeSheets1.Any() ? p.UserTimeSheets1.OrderByDescending(u => u.AddDate).FirstOrDefault().AddDate : (DateTime?)null,
                UserEmail = p.EmailOffice,
                UserName = p.Name,
                PMUid = p.PMUid,
                Isactive = p.IsActive == true,
                Role = p.RoleId,
                JoinedDate = p.JoinedDate,
                UserLeaves = p.LeaveActivities1 != null && p.LeaveActivities1.Count > 0 ? p.LeaveActivities1.Where(la => la.Status.Value == 7).ToList() : new List<LeaveActivity>()
            }).OrderBy(X => X.UserName).ToList();
        }
        public static List<Preference> GetAllPrefrences()
        {
            return db.Preferences.Where(p => p.IsActive.Value).ToList();
        }

        public static List<OfficialLeave> GetOfficalLeave()
        {
            return db.OfficialLeave.Where(ol => ol.IsActive && ol.LeaveDate.Year == DateTime.Now.Year && ol.LeaveType.ToLower()=="holiday").ToList();
        }
    }
}

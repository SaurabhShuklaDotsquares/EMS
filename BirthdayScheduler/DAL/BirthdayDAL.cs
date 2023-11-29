using System;
using System.Collections.Generic;
using System.Text;
using EMS.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BirthdayScheduler.Model;

namespace BirthdayScheduler.DAL
{
    public static class BirthdayDAL
    {
        static db_dsmanagementnewContext db = new db_dsmanagementnewContext();
        public static List<BirthdayUserDto> GetUsersForBirthdaySchedular()
        {
            var logins = db.UserLogin.AsEnumerable();
            return logins.Where(UL => UL.IsActive == true && UL.IsResigned == false && Convert.ToDateTime(UL.DOB).Date == DateTime.Today.Date).Select(p => new BirthdayUserDto
            {
                Uid = p.Uid,
                EmailOffice = p.EmailOffice,
                UserName = p.UserName,
                Title = p.Title,
                Name = p.Name,
                PMUid = p.PMUid,
                Role = p.RoleId,
                DOB = p.DOB,
                EmailPersonal = p.EmailPersonal,
                Gender = p.Gender,
                MarriageDate = p.MarraigeDate,
                Mobile = Convert.ToInt64(p.MobileNumber),
                ProfilePicture = p.ProfilePicture,
                IsResigned = p.IsResigned,
                Isactive = p.IsActive == true
            }).OrderBy(X => X.Name).ToList();
        }
    }
}

using EMS.Core;
using EMS.Data;
using EMS.Data.Model;
using EMS.Dto;
using EMS.Repo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace EMS.Service
{
    public class ReminderService : IReminderService
    {
        #region Constructor and Fields

        private readonly IRepository<Reminder> repoReminder;
        private readonly IRepository<ReminderUser> repoReminderUser;
        public ReminderService(IRepository<Reminder> repoReminder, IRepository<ReminderUser> repoReminderUser)
        {
            this.repoReminder = repoReminder;
            this.repoReminderUser = repoReminderUser;
        }

        public void Dispose()
        {
            if (repoReminder != null)
            {
                repoReminder.Dispose();
                //repoReminder = null;
            }
        }

        #endregion
        public List<Reminder> ReminderList(int UserId)
        {
            return repoReminder.Query()
                    .Filter(x => x.CreatedBy == UserId && x.IsActive == true && (x.Status == 0 || x.Status == null) && x.IsExcludeMe == false && x.ReminderDate.Value.Date <= DateTime.Now.Date)
                    .Include(x => x.ReminderUser)
                    .Get()
                    .ToList();
        }

        public List<Reminder> GetRecordsByUserId(int userId)
        {
            return repoReminder.Query().Include(x => x.ReminderUser).Filter(x => x.CreatedBy == userId).Get().ToList();
        }

        public List<ReminderUser>FindRecordsByParameters(List<long> reminderIDs)
        {
            return repoReminderUser.Query().Filter(x=> reminderIDs.Contains(x.ReminderId.Value)).Get().ToList();
        }

        public List<ReminderUser> ReminderUserList(int UserId)
        {
            return repoReminderUser.Query()
                    .Filter(x => x.Uid == UserId)
                    .Get().ToList();
        }

        public Reminder GetReminderById(int reminderId)
        {
            return repoReminder.Query().Include(x=> x.ReminderUser).Get().FirstOrDefault(x => x.Id == reminderId);
        }

        public DateTime? GetLastEmailTimeByUId(int UId)
        {
            var emailTime =  repoReminder.Query()
                       .Filter(x => x.CreatedBy == UId && x.LastEmail != null)
                       .Get().FirstOrDefault();
            if(emailTime != null)
            {
                return emailTime.LastEmail;
            }
            else
            {
                return null;
            }
                      
        }

        public IEnumerable<Reminder> GetReminderByUId(int UId)
        {
            
            return repoReminder.Query().Filter(x => x.CreatedBy == UId && x.IsActive == true && x.ReminderDate.Value.Date <= DateTime.Now.Date && x.IsExcludeMe == false).Get().ToList();
            
        }

        public bool Save(Reminder reminder)
        {
            if (reminder.Id != 0)
            {
                repoReminder.Update(reminder);
            }
            else
            {
                repoReminder.Insert(reminder);
            }
            return true;

        }

        public void Delete(int id)
        {
            var entity = GetReminderById(id);
            if (entity != null)
            {
                repoReminder.Delete(entity);
            }
        }

        public void DeleteCollection(List<ReminderUser> entityCollection)
        {
            repoReminderUser.DeleteCollection(entityCollection);
        }

        public List<Reminder> GetReminderByPaging(out int total, PagingService<Reminder> pagingSerices)
        {
            return repoReminder.Query().Filter(pagingSerices.Filter).
                 OrderBy(pagingSerices.Sort).Include(x => x.ReminderUser).
                 GetPage(pagingSerices.Start, pagingSerices.Length, out total).
                 ToList();
        }

    }
}

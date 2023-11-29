using EMS.Data;
using EMS.Data.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service
{
    public interface IReminderService : IDisposable
    {
        List<Reminder> ReminderList(int UserId);
        Reminder GetReminderById(int reminderId);
        bool Save(Reminder reminder);
        void DeleteCollection(List<ReminderUser> entityCollection);
        List<Reminder> GetReminderByPaging(out int total, PagingService<Reminder> pagingSerices);
        IEnumerable<Reminder> GetReminderByUId(int UId);
        DateTime? GetLastEmailTimeByUId(int UId);
        void Delete(int id);
        List<Reminder> GetRecordsByUserId(int userId);
        List<ReminderUser> FindRecordsByParameters(List<long> reminderIDs);

    }
}

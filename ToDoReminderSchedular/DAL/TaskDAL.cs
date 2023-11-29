using EMS.Core;
using EMS.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ToDoReminderSchedular.DAL
{
    public class TaskDAL
    {
        public static List<Task> TaskList()
        {
            DateTime lastDate = DateTime.Today.AddDays(1);
            using (var db = new db_dsmanagementnewContext())
            {
                return db.Task
                    .Include(x => x.TaskAssignedToes)
                        .Include(x => x.TaskAssignedToes.Select(u => u.UserLogin))
                        .Include(x => x.UserLogin)
                        .Where(t => t.TaskEndDate == lastDate && !t.ReminderEmailSent &&
                                    t.TaskStatusID != (int)Enums.TaskStatusType.Closed &&
                                    t.TaskStatusID != (int)Enums.TaskStatusType.Completed)
                        .ToList();
            }
        }

        public static void UpdateReminderEmail(int id)
        {
            if (id > 0)
            {
                using (var db = new db_dsmanagementnewContext())
                {
                    var entity = db.Task.Find(id);
                    if (entity != null)
                    {
                        entity.ReminderEmailSent = true;
                        db.SaveChanges();
                    }
                }
            }
        }
    }
}

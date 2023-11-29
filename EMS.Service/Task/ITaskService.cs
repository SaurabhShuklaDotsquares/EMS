using EMS.Data;
using EMS.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Core;

namespace EMS.Service
{
    public interface ITaskService
    {
        bool Save(CreateTaskDto entity);
        List<TaskStatu> GetTask();
        List<Data.Task> GettaskByPaging(out int total, PagingService<Data.Task> pagingSerices);
        List<Data.Task> GetTaskList();
        Data.Task GetTaskById(int id);
        void Delete(int id);
        Data.Task SaveComment(TaskCommentDto entity);
        Data.Task GetTaskByMomMeetingTaskId(int momMeetingTaskId);
        string GetStatus(decimal taskId, int CurrentUserId);

        Enums.TaskStatusType GetTaskStatusId(decimal taskId, int CurrentUserId);

        List<Data.Task> GetTasksInDuration(int uid, DateTime? startDate, DateTime? endDate);
    }
}

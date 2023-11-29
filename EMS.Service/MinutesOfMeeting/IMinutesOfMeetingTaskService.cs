using EMS.Data;
using EMS.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service
{
    public interface IMinutesOfMeetingTaskService:IDisposable
    {
        List<MomMeetingTask> GetMinutesOfMeetingTaskByPaging(out int total, PagingService<MomMeetingTask> pagingService);

        List<MomMeetingTask> GetMinutesOfMeetingTaskForEmail(int MeetingMasterID, DateTime MOMCreatedDate);
        List<MomMeetingTask> GetMinutesOfMeetingTaskByMeetingMasterId(int MeetingMasterID, DateTime MOMCreatedDate);
        
        MomMeetingTask GetMinutesOfMeetingTaskFindById(int id);
        List<MomMeetingTask> GetMinutesOfMeetingTaskList(int id);
        
        MomMeetingTask Save(MomMeetingTaskDto model);
        MomMeetingTask SaveTaskDecision(MomMeetingTaskDto model);
        
        bool Delete(int Id);
    }
}

using EMS.Data;
using EMS.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service
{
    public interface IMinutesOfMeetingTaskCommentService : IDisposable
    {
        List<MomMeetingTaskTimeLine> GetMinutesOfMeetingByPaging(out int total, PagingService<MomMeetingTaskTimeLine> pagingService);
        MomMeetingTaskTimeLine GetMinutesOfMeetingTaskTimeLineFindById(int Id);
        MomMeetingTask GetMinutesOfMeetingTaskTimeLineWithMomTaskFindById(int Id);
        List<MomMeetingTaskTimeLine> GetMinutesOfMeetingTaskCommentByTaskId(int Id);
        bool AddMomMeetingTaskTimeLine(MomMeetingTaskTimeLineDto model);
        MomMeetingTaskTimeLine Save(MomMeetingTaskCommentsAddDto model);
        bool Delete(int Id);
    }
}

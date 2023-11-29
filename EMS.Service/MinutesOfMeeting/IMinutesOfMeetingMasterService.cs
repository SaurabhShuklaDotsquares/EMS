using System;
using System.Collections.Generic;
using EMS.Data;
using EMS.Dto;

namespace EMS.Service
{
    public interface IMinutesOfMeetingMasterService : IDisposable
    {
        List<MeetingMaster> GetMinutesOfMeetingByPaging(out int total, PagingService<MeetingMaster> pagingService);
        MeetingMaster GetMinutesOfMeetingFindById(int Id);
        MeetingMaster Save(MeetingMasterDto model);
        bool Delete(int Id);
    }
}

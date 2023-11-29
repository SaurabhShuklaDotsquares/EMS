using EMS.Data;
using EMS.Data.Model;
using EMS.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service
{
    public interface IMinutesOfMeetingService : IDisposable
    {
        List<MomMeeting> GetMinutesOfMeetingByPaging(out int total, PagingService<MomMeeting> pagingService);

        MomMeeting GetMinutesOfMeetingFindById(int Id);

        MomMeeting GetMinutesOfMeetingFindTopicId(int Id);

        List<MomMeeting> GetAllMinutesOfMeetingFindByTopicId(int Id);

        MomMeeting Save(MomMeetingDto model);

        List<MomMeetingParticipant> GetMinutesofMeetingPreviousMeetingUsersByMeetingMasterId(int id, int authorId);

        List<MomMeetingDepartment> GetMinutesofMeetingPreviousMeetingDepartmentsByMeetingMasterId(int id, int authorId);

        MomMeeting GetPreviousMinutesofMeetingByMeetingMasterId(int masterId);

        bool DeleteDocument(int id);

        bool Delete(int Id);

        Momdocument GetDocument(int id);

        MomMeetingTaskDocument GetMomMeetingTaskDocument(int id);

        bool DeleteMomMeetingTaskDocument(int id);

        MomMeeting GetLatestMinutesofMeeting();

        List<GETMOMListByMeetingMasterID_Result> GETMOMListByMeetingMasterIDSP(int Id);
    }
}
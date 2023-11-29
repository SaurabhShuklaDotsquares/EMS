using EMS.Data;
using EMS.Data.Model;
using EMS.Dto;
using EMS.Dto.EmployeeFeedback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service
{
    public interface IFeedbackService
    {
        List<EmployeeFeedback> GetfeedbackByPaging(out int total, PagingService<EmployeeFeedback> pagingSerices);        
        bool Save(FeedbackDto model);
        List<EmployeeFeedbackReason> getemployeefeedbackreason();
        List<EmployeeFeedbackRank> getemployeefeedbackrank();
        EmployeeFeedback findByData(int id);
        bool FeedbackExists(int uid);

        //List<EmployeeFeedbackReasonMapping> getemployeefeedbackreasonmapping();

        List<EmployeeFeedback> employeefeedback();

        List<UserExitProcess> userexitprocesslist();

        UserExitProcess findByUid(int uid);

        UserExitProcess saveprocess(ExitProcessDto entity);

        EmployeeFeedback employeeFeedbackbyUid(int uid);

        List<UserNoc> usernocList(int uid);
        List<NocMaster> NocMasterList();

        UserExitProcess updateExitprocess(ExitProcessDto model);

        EmployeeFeedback updateEmpFeedback(FeedbackDto model);

        UserNoc userNocData(int uid, int NocId);

        UserNoc saveNoc(NocDetailsDto model);

        UserExitProcess updateFeedbackStatus(int uid);
        UserExitProcess findById(int id);

        bool Savefeedback(EmpFeedbackDto model);
    }
}

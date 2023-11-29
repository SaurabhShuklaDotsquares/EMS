using EMS.Data;
using System;
using System.Collections.Generic;

namespace EMS.Service
{
    public interface IProjectClientFeedbackService :IDisposable
    {
        ProjectClientFeedback GetProjectClientFeedback(int CRMFeedbackId);
        ProjectClientFeedback Save(ProjectClientFeedback entity);
        ProjectClientFeedback ProjectClientFeedbackById(int Id);
        List<Project> GetProjectHavingClientFeedback();
        List<ProjectClientFeedback> GetProjectClientFeedbacksByPaging(out int total, PagingService<ProjectClientFeedback> pagingService);
        List<Project> GetProjectHavingClientFeedback(int ?pmId);
        bool DeleteDocument(int id);
        ProjectClientFeedbackDocument GetDocumentById(int id);

    }
}

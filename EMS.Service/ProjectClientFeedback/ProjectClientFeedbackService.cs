using EMS.Data;
using EMS.Repo;
using System.Collections.Generic;
using System.Linq;

namespace EMS.Service
{
    public class ProjectClientFeedbackService: IProjectClientFeedbackService
    {
        private IRepository<ProjectClientFeedback> repoProjectClientFeedback;
        private IRepository<ProjectClientFeedbackDocument> repoClientFeedbackDocument;
        public ProjectClientFeedbackService(IRepository<ProjectClientFeedback> _repoProjectClientFeedback,
            IRepository<ProjectClientFeedbackDocument> _repoClientFeedbackDocument)
        {
            repoProjectClientFeedback = _repoProjectClientFeedback;
            repoClientFeedbackDocument = _repoClientFeedbackDocument;
        }
        public ProjectClientFeedback GetProjectClientFeedback(int CRMFeedbackId)
        {
            return repoProjectClientFeedback.Query().Filter(p => p.CrmfeedbackId == CRMFeedbackId).Get().FirstOrDefault();
        }

        public ProjectClientFeedback Save(ProjectClientFeedback entity)
        {
            if (entity.Id > 0)
            {
                repoProjectClientFeedback.Update(entity);
            }
            else
            {
                repoProjectClientFeedback.Insert(entity);
            }

            return ProjectClientFeedbackById(entity.Id);

        }
        public ProjectClientFeedback ProjectClientFeedbackById(int Id)
        {
            return repoProjectClientFeedback.Query().Filter(x => x.Id == Id).Get().FirstOrDefault();
        }

        public List<Project> GetProjectHavingClientFeedback()
        {
            return repoProjectClientFeedback.Query()
                .GetQuerable()
                .GroupBy(x => x.ProjectId)
                .Select(g => g.OrderByDescending(x => x.Id).FirstOrDefault().Project).ToList();

        }
        public List<Project> GetProjectHavingClientFeedback(int? pmId)
        {
            return repoProjectClientFeedback.Query()
                .GetQuerable().Where(f=> !pmId.HasValue || f.Project.PMUid==pmId)
                .GroupBy(x => x.ProjectId)
                .Select(g => g.OrderByDescending(x => x.Id).FirstOrDefault().Project).ToList();

        }

        public List<ProjectClientFeedback> GetProjectClientFeedbacksByPaging(out int total, PagingService<ProjectClientFeedback> pagingService)
        {
            return repoProjectClientFeedback.Query().Filter(pagingService.Filter).
                OrderBy(pagingService.Sort).
                GetPage(pagingService.Start, pagingService.Length, out total).
                ToList();
        }

        public bool DeleteDocument(int id)
        {
            
            var document = repoClientFeedbackDocument.FindById(id);
            if (document != null)
            {
                repoClientFeedbackDocument.Delete(document);
                return true;
            }
            return false;
        }
        public ProjectClientFeedbackDocument GetDocumentById(int id)
        {
            return repoClientFeedbackDocument.FindById(id);
        }

        #region Dispose
        public void Dispose()
        {
            if (repoProjectClientFeedback != null)
            {
                repoProjectClientFeedback.Dispose();
                repoProjectClientFeedback = null;
            }
        }
        #endregion
    }
}

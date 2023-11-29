using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EMS.Service
{
    public class ProjectClosureService : IProjectClosureService
    {
        private IRepository<ProjectClosure> repoProjectClosure;
        private IRepository<ProjectClosureDetail> repoProjectClosureDetail;
        private IRepository<Project> repoProject;
        private IRepository<Preference> repoPreference;
        private IRepository<AbroadPM> repoAbroadPM;
        private IRepository<ProjectClosureAbroadPm> repoProjectClosureAbroadPm;

        public ProjectClosureService(IRepository<Preference> _repopreference,
            IRepository<ProjectClosure> _repoprojectcloser,
            IRepository<Project> _repoproject,
            IRepository<ProjectClosureDetail> _repoprojectcloserdetail,
            IRepository<AbroadPM> _repoAbroadPM,
            IRepository<ProjectClosureAbroadPm> _repoProjectClosureAbroadPm)
        {
            repoProjectClosure = _repoprojectcloser;
            repoProject = _repoproject;
            repoProjectClosureDetail = _repoprojectcloserdetail;
            repoPreference = _repopreference;
            repoAbroadPM = _repoAbroadPM;
            repoProjectClosureAbroadPm = _repoProjectClosureAbroadPm;
        }

        public List<ProjectClosure> GetProjectClosurePaging(out int total, PagingService<ProjectClosure> pagingService)
        {
            return repoProjectClosure.Query()
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort)
                .GetPage(pagingService.Start, pagingService.Length, out total)
                .ToList();
        }

        public List<ProjectClosure> GetProjectClosureReportPaging(out int total, PagingService<ProjectClosure> pagingService, Enums.ProjectClosureFilterType? filterType = null)
        {
            var records = repoProjectClosure.Query().Include(q => q.UserLogin)
                .Filter(pagingService.Filter)
                .GetQuerable()
                .GroupBy(x => x.ProjectID.Value)
                .Select(g => g.OrderByDescending(x => x.Id).FirstOrDefault());

            if (filterType.HasValue)
            {
                switch (filterType.Value)
                {
                    case Enums.ProjectClosureFilterType.ProjectRestarted:
                        records = records.Where(x => x.Status == (int)Enums.CloserType.Converted);
                        break;
                    case Enums.ProjectClosureFilterType.ProjectNotRestarted:
                        records = records.Where(x => x.Status != (int)Enums.CloserType.Converted);
                        break;
                    case Enums.ProjectClosureFilterType.ProjectPromising:
                        records = records.Where(x => x.Status == (int)Enums.CloserType.Pending && x.ProjectClosureReview != null &&
                                                     x.ProjectClosureReview.PromisingPercentageId == (int)Enums.ProjectClosureReviewPercentage.HundredPercent);
                        break;
                    case Enums.ProjectClosureFilterType.ProjectLessPromising:
                        records = records.Where(x => x.Status == (int)Enums.CloserType.Pending && x.ProjectClosureReview != null &&
                                                x.ProjectClosureReview.PromisingPercentageId == (int)Enums.ProjectClosureReviewPercentage.FiftyPercent);
                        break;
                }
            }

            total = records.Count();

            //return records.OrderBy(o => o.CRMUpdated).ThenByDescending(o => o.Modified)
            return records.OrderBy(o => o.CRMUpdated).ThenByDescending(c => c.DateofClosing.Value)
                            .Skip((pagingService.Start - 1) * pagingService.Length)
                            .Take(pagingService.Length)
                            .ToList();
        }

        public List<ProjectClosure> GetProjectClosure(Expression<Func<ProjectClosure, bool>> expr)
        {
            return repoProjectClosure.Query()
                .Filter(expr)
                .OrderBy(o => o.OrderByDescending(x => x.Modified))
                .Get()
                .ToList();
        }

        public List<Project> GetAllProjectsNamewise(int pmid)
        {
            return repoProject.Query().Filter(x => x.PMUid == pmid || x.ProjectOtherPm.Any(po => po.Pmuid == pmid)).Get().OrderBy(s => s.Name).ToList();
        }

        public List<Project> GetAllProjectsNamewise(int pmid,int pmuid)
        {
            return repoProject.Query().Filter(x => x.PMUid == pmid || x.PMUid == pmuid || x.ProjectOtherPm.Any(po => po.Pmuid == pmid)).Get().OrderBy(s => s.Name).ToList();
        }

        public List<AbroadPM> GetAllAbroadPM(string countryText)
        {
            List<AbroadPM> abroadPMList = new List<AbroadPM>();
            List<AbroadPM> filterAbroadPM_ByCountries = new List<AbroadPM>();
            abroadPMList = repoAbroadPM.Query().Filter(x => x.isActive == true).Get().OrderBy(s => s.Name).ToList();
            if (countryText.ToLower() == "uk")
            {
                filterAbroadPM_ByCountries = abroadPMList.Where(x => x.Country.ToLower() == countryText.ToLower().Trim() || x.Country.ToLower() == "us" || x.Country.ToLower() == "usa").ToList();
            }
            else if (countryText.ToLower() == "us" || countryText.ToLower() == "usa")
            {
                filterAbroadPM_ByCountries = abroadPMList.Where(x => x.Country.ToLower() == "us" || x.Country.ToLower() == "usa").ToList();
            }
            else
            {
                filterAbroadPM_ByCountries = abroadPMList.Where(x => x.Country.ToLower() == countryText.ToLower().Trim()).ToList();
            }
            if (filterAbroadPM_ByCountries.Count > 0)
            {
                abroadPMList = filterAbroadPM_ByCountries;
            }
            else
            {
                abroadPMList = abroadPMList.Select(x => new AbroadPM
                {
                    AutoID = x.AutoID,
                    Country = x.Country,
                    Name = x.Name,
                    Email = x.Email,
                    isDefaultForEmail = false
                }).ToList();
            }
            return abroadPMList;
        }

        public List<ProjectClosureAbroadPm> GetProjectClosureAbroadPMByProjectId(int projectClosureId)
        {
            return repoProjectClosureAbroadPm.Query().Filter(x => x.ProjectClosureId == projectClosureId).Get().ToList();
        }


        public void SaveProjectClosureAbroadPMB(List<ProjectClosureAbroadPm> entity)
        {
            var ProjectClosure = repoProjectClosure.FindById(entity.Select(T => T.ProjectClosureId).FirstOrDefault());
            if (ProjectClosure != null)
            {
                if (ProjectClosure.ProjectClosureAbroadPm.Any())
                {
                    repoProjectClosureAbroadPm.ChangeEntityCollectionState(ProjectClosure.ProjectClosureAbroadPm, ObjectState.Deleted);
                }
                entity.ForEach(x => repoProjectClosureAbroadPm.ChangeEntityState(x, ObjectState.Added));
                repoProjectClosureAbroadPm.SaveChanges();
            }
        }

        public void DeleteProjectClosureAbroadPMByProjectClosureId(int Id)
        {
            ProjectClosure entity = repoProjectClosure.FindById(Id);
            if (entity != null)
            {
                repoProjectClosure.ChangeEntityCollectionState(entity.ProjectClosureAbroadPm, ObjectState.Deleted);
                repoProjectClosure.ChangeEntityCollectionState(entity.ProjectClosureDetails, ObjectState.Deleted);
                //repoProjectClosure.Delete(Id);
            }
        }
        //public List<ProjectClosureAbroadPm> DeleteProjectClosureAbroadPMById(int projectClosurePMId)
        //{
        //    ProjectClosureAbroadPm entity = repoProjectClosureAbroadPm.FindById(projectClosurePMId);
        //    if (entity != null)
        //    {
        //        repoProjectClosure.ChangeEntityCollectionState(entity.ProjectClosureAbroadPm, ObjectState.Deleted);
        //        repoProjectClosureAbroadPm.Delete(projectClosurePMId);
        //    }
        //    return repoProjectClosureAbroadPm.Query().Filter(x => x.ProjectClosureId == projectClosureId).Get().ToList();
        //}


        public ProjectClosure Save(ProjectClosureDto model)
        {
            ProjectClosure entity = model.Id > 0 ? repoProjectClosure.FindById(model.Id) : new ProjectClosure();

            if (entity == null || (entity.Id > 0 && entity.PMID != model.PMUid))
            {
                return null;
            }

            entity.NextStartDate = model.NextStartDate.ToDateTime("dd/MM/yyyy");
            entity.DateofClosing = model.DateOfClosing.ToDateTime("dd/MM/yyyy");
            entity.CRMStatus = model.CRMStatusId;
            entity.OtherActualDeveloper = model.OtherActualDeveloper;
            entity.ProjectID = model.ProjectID;
            entity.Status = model.ChangeStatusId > 0 ? model.ChangeStatusId : model.Status;
            entity.Uid_BA = model.Uid_BA;
            entity.Uid_Dev = model.Uid_Dev;
            entity.Reason = model.Reason;
            entity.Suggestion = model.Suggestion;
            entity.ProjectLiveUrl = model.LiveUrl ? model.ProjectLiveUrl : null;
            entity.ClientQuality = (byte)model.ClientQuality;
            entity.ProjectUrlAbsenseReason = model.LiveUrl ? null : model.ProjectUrlAbsenseReason;
            entity.Country = model.Country.ToString();
            entity.Modified = DateTime.Now;
            entity.Uid_TL = model.Uid_TL > 0 ? model.Uid_TL : entity.Uid_TL;
            entity.IsCovid19 = model.IsCovid19;

            if (!String.IsNullOrEmpty(model.DeadResponseDate) && model.IsPermanentDead==false)
            {
                entity.DeadResponseDate = model.DeadResponseDate.ToDateTime("dd/MM/yyyy");
            }
            else
            {
                entity.DeadResponseDate = null;
            }

            if (model.Id > 0)
            {
                repoProjectClosure.SaveChanges();
            }
            else
            {
                entity.Created = DateTime.Now;
                entity.PMID = model.PMUid;
                entity.AddedBy = model.AddedBy;
                entity.CRMUpdated = true;
                repoProjectClosure.Insert(entity);
            }
            return projectClosureFindById(entity.Id);
        }
        public ProjectClosure SaveByUK(ProjectClosureDto model,int UKPMId)
        {
            ProjectClosure entity = model.Id > 0 ? repoProjectClosure.FindById(model.Id) : new ProjectClosure();

            if (entity == null || (entity.Id > 0 && (entity.PMID != model.PMUid && entity.PMID != UKPMId)))
            {
                return null;
            }

            entity.NextStartDate = model.NextStartDate.ToDateTime("dd/MM/yyyy");
            entity.DateofClosing = model.DateOfClosing.ToDateTime("dd/MM/yyyy");
            entity.CRMStatus = model.CRMStatusId;
            entity.OtherActualDeveloper = model.OtherActualDeveloper;
            entity.ProjectID = model.ProjectID;
            entity.Status = model.ChangeStatusId > 0 ? model.ChangeStatusId : model.Status;
            entity.Uid_BA = model.Uid_BA;
            entity.Uid_Dev = model.Uid_Dev;
            entity.Reason = model.Reason;
            entity.Suggestion = model.Suggestion;
            entity.ProjectLiveUrl = model.LiveUrl ? model.ProjectLiveUrl : null;
            entity.ClientQuality = (byte)model.ClientQuality;
            entity.ProjectUrlAbsenseReason = model.LiveUrl ? null : model.ProjectUrlAbsenseReason;
            entity.Country = model.Country.ToString();
            entity.Modified = DateTime.Now;
            entity.Uid_TL = model.Uid_TL > 0 ? model.Uid_TL : entity.Uid_TL;
            entity.IsCovid19 = model.IsCovid19;

            if (!String.IsNullOrEmpty(model.DeadResponseDate) && model.IsPermanentDead == false)
            {
                entity.DeadResponseDate = model.DeadResponseDate.ToDateTime("dd/MM/yyyy");
            }
            else
            {
                entity.DeadResponseDate = null;
            }

            if (model.Id > 0)
            {
                repoProjectClosure.SaveChanges();
            }
            else
            {
                entity.Created = DateTime.Now;
                entity.PMID = UKPMId;
                entity.AddedBy = model.AddedBy;
                entity.CRMUpdated = true;
               repoProjectClosure.Insert(entity);
            }
            return projectClosureFindById(entity.Id);
        }

        public ProjectClosure Save(ProjectClosure entity)
        {
            if (entity.Id > 0)
            {
                repoProjectClosure.Update(entity);
            }
            else
            {
                repoProjectClosure.Insert(entity);
            }

            return projectClosureById(entity.Id);
        }

        public bool UpdateCRMStatus(int id)
        {
            var closureEntity = repoProjectClosure.FindById(id);
            if (closureEntity != null)
            {
                closureEntity.CRMUpdated = true;
                repoProjectClosure.SaveChanges();
                return true;
            }
            return false;
        }


        public bool UpdateCRMStatus(ProjectClosure projectClosure)
        {
            var closureEntity = repoProjectClosure.FindById(projectClosure.Id);

            try
            {
                closureEntity.CRMUpdated = true;
                //closureEntity.CRMStatus = projectClosure.Status;
                closureEntity.Project.Status = Common.GetEMSProjectStatusFromCRMStatusEnum(closureEntity.CRMStatus.ToString());
                repoProjectClosure.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        private ProjectClosure projectClosureById(int Id)
        {
            return repoProjectClosure.Query().Filter(x => x.Id == Id).Get().FirstOrDefault();
        }

        public ProjectClosure projectClosureFindById(int Id)
        {
            return repoProjectClosure.FindById(Id);
        }

        public void Delete(int Id)
        {
            ProjectClosure entity = repoProjectClosure.FindById(Id);
            if (entity != null)
            {
                repoProjectClosure.ChangeEntityCollectionState(entity.ProjectClosureAbroadPm, ObjectState.Deleted);
                repoProjectClosure.ChangeEntityCollectionState(entity.ProjectClosureDetails, ObjectState.Deleted);
                repoProjectClosure.Delete(Id);
            }
        }

        public List<ProjectClosure> GetProjectClosureOnDate(int AddedByUid, DateTime NextStartDate)
        {
            return repoProjectClosure.Query().Filter(s => (s.Uid_BA == AddedByUid || s.Uid_TL == AddedByUid) && s.NextStartDate == NextStartDate).Get().OrderByDescending(x => x.NextStartDate).ToList();
        }

        public List<ProjectClosureDetail> GetProjectClosureDetailOnDate(int AddedByUid, DateTime NextStartDate)
        {
            return repoProjectClosureDetail.Query().Filter(s => s.AddedByUid == AddedByUid && s.NextStartDate == NextStartDate).Get().OrderByDescending(x => x.NextStartDate).ToList();
        }

        public ProjectClosureDetail SaveDetail(ProjectClousreDetailDto model)
        {
            var projectClosure = projectClosureFindById(model.ProjectClousreId);

            if (projectClosure != null && projectClosure.PMID == model.PMUid)
            {
                var detail = new ProjectClosureDetail();
                detail.ProjectClosureId = model.ProjectClousreId;
                detail.Reason = model.Reason;
                detail.NextStartDate = model.NextStartDate.ToDateTime("dd/MM/yyyy");
                detail.Created = DateTime.Now;
                detail.AddedByUid = model.AddedByUid;

                projectClosure.NextStartDate = detail.NextStartDate;
                projectClosure.Modified = DateTime.Now;

                repoProjectClosureDetail.Insert(detail);

                return detail;
            }
            return null;
        }

        public Project GetProjectNameById(int Id)
        {
            return repoProject.FindById(Id);
        }

        public List<ProjectClosureDetail> GetProjectClosureDetail(int Id)
        {
            var detail = repoProjectClosureDetail.Query().Filter(s => s.ProjectClosureId == Id).Get().OrderByDescending(x => x.NextStartDate).ToList();
            return detail;
        }

        public Preference GetDataByPmuid(int pmid)
        {
            return repoPreference.Query().Filter(s => s.pmid == pmid).Get().FirstOrDefault();
        }

        public ProjectClosure UpdateProjectStatus(ProjectClosureStatusDto model)
        {
            var entity = projectClosureFindById(model.ProjectClosureId);
            if (entity != null)
            {
                entity.Status = model.ChangeStatusId;
                entity.ProjectClosureDetails.Add(new ProjectClosureDetail()
                {
                    NextStartDate = DateTime.Today,
                    Created = DateTime.Now,
                    Reason = model.Reason
                });
                repoProjectClosure.SaveChanges();

                return entity;
            }
            return null;
        }

        public ProjectClosure GetDataByProjectID(int projectID)
        {
            return repoProjectClosure.Query()
                .Filter(P => P.ProjectID == projectID && P.Status == (int)Enums.CloserType.Pending)
                .OrderBy(o => o.OrderByDescending(x => x.Modified))
                .GetQuerable()
                .FirstOrDefault();
        }

        public int GetUnapprovedClosureCount(int pmUid, int baTlId)
        {
            return repoProjectClosure.Query()
                         .Filter(x => (baTlId == 0 || x.Uid_TL == baTlId || x.Uid_BA == baTlId || x.AddedBy == baTlId || x.Uid_TL == null || x.Uid_BA == null) &&
                                         x.PMID == pmUid && x.CRMUpdated == false)
                         .GetQuerable()
                         .GroupBy(x => x.ProjectID.Value)
                         .Select(g => g.OrderByDescending(x => x.Id).FirstOrDefault())
                         .Count();
        }
        public List<ProjectClosure> GetProjectClosedList(int uid, DateTime? startDate, DateTime? endDate, string type)
        {
            var expr = PredicateBuilder.True<ProjectClosure>();
            if(startDate.HasValue)
            {
                expr = expr.And(p=>p.DateofClosing>=startDate.Value);
            }
            if(endDate.HasValue)
            {
                expr = expr.And(p => p.DateofClosing <= endDate.Value);
            }
            expr = expr.And(p=> p.Uid_TL == uid || p.Uid_BA == uid || p.Uid_Dev==uid 
            || p.Project.ProjectDevelopers.Any(pd => pd.Uid == uid));

            if (type.Equals("unique"))
            {
                return repoProjectClosure.Query().Include(q => q.UserLogin)
                .Filter(expr)
                .Get()
                .GroupBy(x => x.ProjectID.Value)
                .Select(g => g.OrderByDescending(x => x.Id).FirstOrDefault()).ToList();
            }
            else
            {
                return repoProjectClosure.Query().Include(q => q.UserLogin)
                .Filter(expr)
                .Get().ToList();
            }
            
        }


        public void Dispose()
        {
            if (repoAbroadPM != null)
            {
                repoAbroadPM.Dispose();
                repoAbroadPM = null;
            }

        }

    }
}

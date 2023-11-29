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
    public class ProjectClosureReviewService : IProjectClosureReviewService
    {
        #region Fields and Constructor

        private IRepository<ProjectClosureReview> repoProjectClosureReview;
        private IRepository<ProjectClosure> repoProjectClosure;
        private IRepository<UserLogin> repoUserLogin;
        private IRepository<ProjectDeveloper> repoProjectDeveloper;

        public ProjectClosureReviewService(
            IRepository<ProjectClosureReview> _repoProjectClosureReview,
            IRepository<ProjectClosure> _repoProjectClosure,
            IRepository<UserLogin> _repoUserLogin,
            IRepository<ProjectDeveloper> _repoProjectDeveloper)
        {
            repoProjectClosureReview = _repoProjectClosureReview;
            repoProjectClosure = _repoProjectClosure;
            repoUserLogin = _repoUserLogin;
            repoProjectDeveloper = _repoProjectDeveloper;
        }

        #endregion

        public ProjectClosureReview Get(int id)
        {
            return repoProjectClosureReview.Query()
                .Filter(x => x.ProjectClosureId == id)
                .GetQuerable()
                .FirstOrDefault();
        }

        public ProjectClosureReview GetById(int id)
        {
            return repoProjectClosureReview.FindById(id);
        }

        public List<UserLogin> GetReviewUsers(int pmUid)
        {
            var userIds = repoProjectClosureReview.Query()
                            .Filter(x => x.ProjectClosure.PMID == pmUid)
                            .GetQuerable()
                            .Select(x => x.CreateByUid)
                            .Distinct()
                            .ToList();
            if (userIds.Count > 0)
            {
                return repoUserLogin.Query()
                                .Filter(x => userIds.Contains(x.Uid))
                                .Get()
                                .ToList();
            }

            return new List<UserLogin>();
        }

        public List<ProjectClosureReview> GetReviewPaging(out int total, PagingService<ProjectClosureReview> pagingService)
        {
            var records = repoProjectClosureReview.Query()
                          .Filter(pagingService.Filter)
                          .Include(x => x.ProjectClosure)
                          .Include(x => x.ProjectClosure.Project)
                          .Include(x => x.ProjectClosure.UserLogin1)
                          .GetQuerable()
                          .GroupBy(g => g.ProjectClosure.ProjectID.Value)
                          .Select(g => g.OrderByDescending(x => x.CreateDate).FirstOrDefault());
            
            total = records.Count();
            return records.OrderBy(o => o.NextStartDate)
                            .Skip((pagingService.Start - 1) * pagingService.Length)
                            .Take(pagingService.Length)
                            .ToList();
        }

        public List<ProjectClosureReviewSummaryDto> GetReviewSummary(PagingService<ProjectClosureReview> pagingService)
        {
            int total;
            var records = GetReviewPaging(out total, pagingService);

            return records.GroupBy(x => x.ProjectClosure.PMID.Value)
                    .Select(g => new ProjectClosureReviewSummaryDto
                    {
                        PMId = g.Key,
                        PMName = g.FirstOrDefault().ProjectClosure.UserLogin1?.Name,
                        ProjectLessPromising = g.Count(x => x.PromisingPercentageId == (int)Enums.ProjectClosureReviewPercentage.FiftyPercent),
                        ProjectPromising = g.Count(x => x.PromisingPercentageId == (int)Enums.ProjectClosureReviewPercentage.HundredPercent),
                        ProjectNotSure = g.Count(x => x.PromisingPercentageId == (int)Enums.ProjectClosureReviewPercentage.NotSure || 
                                                        x.PromisingPercentageId == (int)Enums.ProjectClosureReviewPercentage.NotApplicable),

                        OccupancyIncreaseLessPromising = g.Where(x => x.PromisingPercentageId == (int)Enums.ProjectClosureReviewPercentage.FiftyPercent)
                                                          .Sum(x => x.DeveloperCount),
                        OccupancyIncreasePromising = g.Where(x => x.PromisingPercentageId == (int)Enums.ProjectClosureReviewPercentage.HundredPercent)
                                                      .Sum(x => x.DeveloperCount),
                        OccupancyIncreaseNotSure = g.Where(x => x.PromisingPercentageId == (int)Enums.ProjectClosureReviewPercentage.NotSure || 
                                                                x.PromisingPercentageId == (int)Enums.ProjectClosureReviewPercentage.NotApplicable)
                                                    .Sum(x => x.DeveloperCount),
                    })
                    .OrderBy(x => x.PMName)
                    .ToList();
        }

        public ProjectClosureReview Save(ProjectClosureReviewDto model)
        {
            var closureEntity = repoProjectClosure.FindById(model.ProjectClosureId);

            if (closureEntity == null)
            {
                return null;
            }

            ProjectClosureReview reviewEntity = closureEntity.ProjectClosureReview ?? new ProjectClosureReview();

            reviewEntity.NextStartDate = model.NextStartDate.ToDateTime("dd/MM/yyyy");
            reviewEntity.Comments = model.Comments;
            reviewEntity.PromisingPercentageId = model.PromisingPercentageId;
            reviewEntity.DeveloperCount = model.DeveloperCount.Value;
            reviewEntity.ModifyByUid = model.CurrentUserId;
            reviewEntity.ModifyDate = DateTime.Now;

            if (reviewEntity.ProjectClosureId == 0)
            {
                reviewEntity.ProjectClosureId = closureEntity.Id;
                reviewEntity.CreateDate = DateTime.Now;
                reviewEntity.CreateByUid = model.CurrentUserId;
            }

            closureEntity.ProjectClosureReview = reviewEntity;
            repoProjectClosure.SaveChanges();

            return Get(reviewEntity.ProjectClosureId);
        }

        public ProjectClosureReview Update(ProjectClosureReview entity)
        {
            repoProjectClosureReview.Update(entity);

            return Get(entity.ProjectClosureId);
        }
        /// <summary>
        /// Gets occupancy for given dates
        /// </summary>
        /// <param name="projectionWeek">Projection week with start date and end Date</param>
        /// <returns>occupancy count</returns>
        public int GetForcastOccupancyForWeek(Expression<Func<ProjectClosureReview, bool>> expr)
        {
            //    var expr = PredicateBuilder.True<ProjectClosureReview>()
            //    .And(x => x.ProjectClosure.Status == (int)Enums.CloserType.Pending && x.NextStartDate.HasValue &&
            //((DateTime)x.NextStartDate).Date >= projectionWeek.StartDate && ((DateTime)x.NextStartDate).Date <= projectionWeek.EndDate);
            //    if (RoleId == (int)Enums.UserRoles.Director || RoleId == (int)Enums.UserRoles.UKBDM)
            //    {

            //    }
            //    else if (RoleId == (int)Enums.UserRoles.PM)
            //    {
            //        expr = expr.And(x => x.ProjectClosure.PMID == uid);
            //    }
            //    else if(RoleId == (int)Enums.UserRoles.BA || RoleId == (int)Enums.UserRoles.SD || RoleId == (int)Enums.UserRoles.TL)
            //    {
            //        expr = expr.And(x => x.ProjectClosure.PMID == uid);
            //    }
            //    else
            //    {

            //    }

            var records = repoProjectClosureReview.Query()
                         .Filter(expr)
                         .Get()
                         .GroupBy(g => g.ProjectClosure.ProjectID.Value)
                         .Select(g => g.OrderByDescending(x => x.CreateDate).FirstOrDefault().DeveloperCount).Sum();

            return records;
        }

        public int GetRunningDevelopersCount(Expression<Func<ProjectDeveloper, bool>> expr)
        {
           return repoProjectDeveloper.Query()
                .Filter(expr)
                .Get()
                .Count();

        }

        public int GetConvertedClosureCount(Expression<Func<ProjectClosureReview, bool>> expr)
        {
            var records = repoProjectClosureReview.Query()
                         .Filter(expr)
                         .Get()
                         .GroupBy(g => g.ProjectClosure.ProjectID.Value)
                         .Select(g => g.OrderByDescending(x => x.CreateDate).FirstOrDefault().DeveloperCount).Count();

            return records;
        }

        ///// <summary>
        ///// Gets occupancy for given dates
        ///// </summary>
        ///// <param name="projectionWeek">Projection week with start date and end Date</param>
        ///// <returns>occupancy count</returns>
        //public int GetForcastOccupancyForWeek(ProjectionWeek projectionWeek)
        //{
        //    var expr = PredicateBuilder.True<ProjectClosureReview>().And(x => x.ProjectClosure.Status == (int)Enums.CloserType.Pending && x.NextStartDate.HasValue &&
        //    ((DateTime)x.NextStartDate).Date >= projectionWeek.StartDate && ((DateTime)x.NextStartDate).Date <= projectionWeek.EndDate);
        //    var records = repoProjectClosureReview.Query()
        //                 //.Filter(p=>p.NextStartDate<=projectionWeek.EndDate && p.NextStartDate >=projectionWeek.StartDate)
        //                 .Filter(expr)
        //                 .Include(x => x.ProjectClosure)
        //                 .Include(x => x.ProjectClosure.Project)
        //                 .Include(x => x.ProjectClosure.UserLogin1)
        //                 .GetQuerable()
        //                 .GroupBy(g => g.ProjectClosure.ProjectID.Value)
        //                 .Select(g => g.OrderByDescending(x => x.CreateDate).FirstOrDefault().DeveloperCount).Sum();

        //    return records;
        //}
    }
}

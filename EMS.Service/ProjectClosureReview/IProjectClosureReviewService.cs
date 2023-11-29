using EMS.Data;
using EMS.Dto;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EMS.Service
{
    public interface IProjectClosureReviewService
    {
        ProjectClosureReview GetById(int id);
        ProjectClosureReview Get(int id);
        List<UserLogin> GetReviewUsers(int pmUid);
        List<ProjectClosureReview> GetReviewPaging(out int total, PagingService<ProjectClosureReview> pagingService);
        List<ProjectClosureReviewSummaryDto> GetReviewSummary(PagingService<ProjectClosureReview> pagingService);

        ProjectClosureReview Save(ProjectClosureReviewDto model);
        ProjectClosureReview Update(ProjectClosureReview entity);

        int GetForcastOccupancyForWeek(Expression<Func<ProjectClosureReview, bool>> expression);
        //int GetForcastOccupancyForWeek(ProjectionWeek projectionWeek);

        int GetRunningDevelopersCount(Expression<Func<ProjectDeveloper, bool>> expr);
        int GetConvertedClosureCount(Expression<Func<ProjectClosureReview, bool>> expr);

    }
}

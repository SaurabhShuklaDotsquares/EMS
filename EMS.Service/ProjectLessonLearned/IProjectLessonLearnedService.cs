using System;
using System.Collections.Generic;
using EMS.Data;
using EMS.Dto;

namespace EMS.Service
{
    public interface IProjectLessonLearnedService : IDisposable
    {
        ProjectLesson GetLessonById(int id);

        List<ProjectLessonTopic> GetLessonTopics();

        ProjectLesson Save(ProjectLessonDto model);

        List<ProjectLesson> GetLessonsByPaging(out int total, PagingService<ProjectLesson> pagingService);
    }
}




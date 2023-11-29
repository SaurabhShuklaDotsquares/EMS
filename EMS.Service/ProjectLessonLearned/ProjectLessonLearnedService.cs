using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EMS.Service
{
    public class ProjectLessonLearnedService : IProjectLessonLearnedService
    {
        #region "Fields"

        private readonly IRepository<ProjectLesson> repoProjectLesson;
        private readonly IRepository<ProjectLessonLearned> repoProjectLessonLearned;
        private readonly IRepository<ProjectLessonTopic> repoProjectLessonTopic;

        #endregion

        #region "Cosntructor"

        public ProjectLessonLearnedService(IRepository<ProjectLesson> _repoProjectLesson, 
            IRepository<ProjectLessonLearned> _repoProjectLessonLearned,
            IRepository<ProjectLessonTopic> _repoProjectLessonTopic)
        {
            repoProjectLesson = _repoProjectLesson;
            repoProjectLessonLearned = _repoProjectLessonLearned;
            repoProjectLessonTopic = _repoProjectLessonTopic;
        }

        #endregion

        public List<ProjectLessonTopic> GetLessonTopics()
        {
            return repoProjectLessonTopic.Query()
                .Filter(x => x.Active)
                .OrderBy(o => o.OrderBy(x => x.TopicGroup).ThenBy(x => x.Name))
                .Get().ToList();
        }

        public ProjectLesson GetLessonById(int id)
        {
            return repoProjectLesson.FindById(id);
        }

        public ProjectLesson Save(ProjectLessonDto model)
        {
            ProjectLesson lessonEntity = null;
            var currentDateTime = DateTime.Now;
            if (model.Id == 0)
            {
                lessonEntity = new ProjectLesson
                {
                    ProjectId = model.ProjectId,
                    CreateByUid = model.CurrentUserId,
                    ModifyByUid = model.CurrentUserId,
                    CreateDate = currentDateTime,
                    ModifyDate = currentDateTime
                };
                
                if (model.LearnedLessons.Any())
                {
                    foreach (var item in model.LearnedLessons)
                    {
                        lessonEntity.ProjectLessonLearneds.Add(new ProjectLessonLearned
                        {
                            ProjectLessonTopicId=item.ProjectLessonTopicId,
                            WhatLearned= item.WhatLearned,
                            WhatWentBad= item.WhatWentBad,
                            WhatWentGood= item.WhatWentGood,
                            WhatImpacted= item.WhatImpacted
                        });
                    }
                }

                repoProjectLesson.InsertGraph(lessonEntity);
            }
            
            return lessonEntity;
        }

        public List<ProjectLesson> GetLessonsByPaging(out int total, PagingService<ProjectLesson> pagingService)
        {
            return repoProjectLesson.Query()
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort)
                .GetPage(pagingService.Start, pagingService.Length, out total)
                .ToList();
        }

        #region "Dispose"

        public void Dispose()
        {
            if (repoProjectLessonLearned != null)
            {
                repoProjectLessonLearned.Dispose();
            }
            if (repoProjectLessonTopic != null)
            {
                repoProjectLessonTopic.Dispose();
            }
        }

        #endregion
    }
}

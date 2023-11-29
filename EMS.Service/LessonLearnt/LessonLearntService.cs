using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMS.Service
{
    public class LessonLearntService : ILessonLearntService
    {
        private readonly IRepository<LessonLearnt> repoLessonLearnt;
        public LessonLearntService(IRepository<LessonLearnt> _repoLessonLearnt)
        {
            this.repoLessonLearnt = _repoLessonLearnt;
        }

        /// <summary>
        /// get data by id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>LessonLearnt</returns>
        public LessonLearnt GetById(int id)
        {
            //return repoLessonLearnt.FindById(id);

            return repoLessonLearnt.Query().Include(x => x.Project).Include(x => x.CreatedBy).Filter(x => x.Id == id).Get().FirstOrDefault();
        }

        /// <summary>
        /// get data by id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>LessonLearntDto</returns>
        public LessonLearntDto GetDtoById(int id)
        {
            var entity = repoLessonLearnt.FindById(id);
            if (entity != null)
            {
                var model = new LessonLearntDto
                {
                    Id = entity.Id,
                    ProjectId = entity.ProjectId,
                    WhatLearnt = entity.WhatLearnt,
                    CreatedById = entity.CreatedById,
                    Created = entity.Created,
                    Modified = entity.Modified,
                };

                return model;
            }

            return null;
        }

        /// <summary>
        /// add and edit Lesson Learnt
        /// </summary>
        /// <param name="model"></param>
        /// <returns>LessonLearnt</returns>
        public LessonLearnt Save(LessonLearntDto model)
        {
            var entity = new LessonLearnt();
            if (model.Id > 0)
            {
                entity = GetById(model.Id);
                if (entity != null)
                {
                    entity.ProjectId = model.ProjectId;
                    entity.WhatLearnt = model.WhatLearnt;
                    entity.Modified = DateTime.Now;

                    repoLessonLearnt.Update(entity);
                }
            }
            else
            {
                var date = DateTime.Now;
                entity.ProjectId = model.ProjectId;
                entity.WhatLearnt = model.WhatLearnt;
                entity.CreatedById = model.CreatedById;
                entity.Created = date;
                entity.Modified = date;

                repoLessonLearnt.Insert(entity);
            }

            return entity;
        }

        /// <summary>
        /// Get data filter with paging
        /// </summary>
        /// <param name="total"></param>
        /// <param name="pagingService"></param>
        /// <returns>List of Lesson Learnt</returns>
        public List<LessonLearnt> GetByPaging(out int total, PagingService<LessonLearnt> pagingService)
        {
            return repoLessonLearnt.Query()
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort)
                .GetPage(pagingService.Start, pagingService.Length, out total)
                .ToList();
        }

        /// <summary>
        ///  Delete Lesson Learnt by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>bool</returns>
        public bool DeleteById(int id)
        {
            bool result = false;
            LessonLearnt LessonLearnt = repoLessonLearnt.FindById(id);
            if (LessonLearnt != null)
            {
                repoLessonLearnt.Delete(LessonLearnt);
                result = true;
            }

            return result;
        }


        public void Dispose()
        {
            if (repoLessonLearnt != null)
            {
                repoLessonLearnt.Dispose();
            }
        }


    }
}

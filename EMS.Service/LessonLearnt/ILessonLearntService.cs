using EMS.Data;
using EMS.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service
{
    public interface ILessonLearntService : IDisposable
    {
        /// <summary>
        /// get data by id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>LessonLearnt</returns>
        LessonLearnt GetById(int id);

        /// <summary>
        /// get data by id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>LessonLearntDto</returns>
        LessonLearntDto GetDtoById(int id);

        /// <summary>
        /// add and edit Lesson Learnt
        /// </summary>
        /// <param name="model"></param>
        /// <returns>LessonLearnt</returns>
        LessonLearnt Save(LessonLearntDto model);

        /// <summary>
        /// Get data filter with paging
        /// </summary>
        /// <param name="total"></param>
        /// <param name="pagingService"></param>
        /// <returns>List of Lesson Learnt</returns>
        List<LessonLearnt> GetByPaging(out int total, PagingService<LessonLearnt> pagingService);

        /// <summary>
        ///  Delete Lesson Learnt by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>bool</returns>
        bool DeleteById(int id);
    }
}

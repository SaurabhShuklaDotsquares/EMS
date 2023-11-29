using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{
    public class ProjectLessonDto
    {
        public ProjectLessonDto()
        {
            ProjectList = new List<SelectListItem>();
            LessonTopicList = new SelectList(new List<SelectListItem>());
            LearnedLessons = new List<ProjectLessonLearnedDto>();
        }

        public int Id { get; set; }

        [DisplayName("Project")]
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        
        public string CreateByName { get; set; }

        public string CreateDate { get; set; }

        public int CurrentUserId { get; set; }

        public List<ProjectLessonLearnedDto> LearnedLessons { get; set; }

        public List<SelectListItem> ProjectList { get; set; }
        public SelectList LessonTopicList { get; set; }

    }

    public class ProjectLessonLearnedDto
    {
        public int ProjectLessonId { get; set; }

        [DisplayName("Lesson Topic")]
        public int ProjectLessonTopicId { get; set; }
        public string ProjectLessonTopicName { get; set; }

        public byte TopicGroup { get; set; }
        public string TopicGroupName { get; set; }

        // [AllowHtml] ** no need ot AllowHTML in .net Core https://github.com/aspnet/Mvc/issues/324#issuecomment-50082022**
        public string WhatWentGood { get; set; }

        // [AllowHtml] ** no need ot AllowHTML in .net Core https://github.com/aspnet/Mvc/issues/324#issuecomment-50082022**
        public string WhatWentBad { get; set; }

        // [AllowHtml] ** no need ot AllowHTML in .net Core https://github.com/aspnet/Mvc/issues/324#issuecomment-50082022**
        public string WhatLearned { get; set; }

        // [AllowHtml] ** no need ot AllowHTML in .net Core https://github.com/aspnet/Mvc/issues/324#issuecomment-50082022**
        public string WhatImpacted { get; set; }
        
    }

    public class ProjectLessonLearnedIndexDto
    {
        public bool ShowAddNewOption { get; set; }
        public List<SelectListItem> PMUserList { get; set; }

        public ProjectLessonLearnedIndexDto()
        {
            PMUserList = new List<SelectListItem>();
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Dto
{
    public class TimesheetDto
    {
        public int SNo { get; set; }
        public int Id { get; set; }
        public string AddedDate { get; set; }
        public string Name { get; set; }
        public string project { get; set; }
        public string Description { get; set; }
        public string WorkHours { get; set; }
        public int ProjectId { get; set; }
        public int DeveloperId { get; set; }
        public string AddedDateEdit { get; set; }
        public string WorkHoursEdit { get; set; }
        public bool IsReviewed { get; set; }
        public string ReviewedBy { get; set; }
        public string ReviewedDate { get; set; }
        public string ReviewStatus { get; set; }
        public string RowColor { get; set; }
        public string InsertDate { get; set; }
        public string Source { get; set; }
        public bool IsRelatedProjectCMMI { get; set; }
        public bool IsNotWithin10DaysRange { get; set; }
    }
    public class ResponseData
    {
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public List<TimesheetDto> TimeSheetList { get; set; }
        public List<ProjectListDto> ProjectList { get; set; }

        public int DefaultProjectID { get; set; }
        public int DefaultVirtualDeveloperID { get; set; }
        public Boolean IsOtherSelected { get; set; }

        public List<DeveloperListDto> DeveloperList { get; set; }

        public bool IsUKPM { get; set; }
        //public string TotalWorkingHours { get; set; }
    }

    public class ProjectListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class HomeProjectListDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class DeveloperListDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Dto
{
    public class TimeSheetReviewDto
    {

        public int UserTimeSheetID { get; set; }
        public string AddedDate { get; set; }
        public string UserName { get; set; }
        public int Uid { get; set; }
        public int ProjectId { get; set; }//add by hemendra
        public string ProjectName { get; set; }
        public int DeveloperId { get; set; }//add by hemendra
        public string DeveloperName { get; set; }
        public string WorkHoours { get; set; }
        public string Description { get; set; }
        public string ReviewBy { get; set; }

    }

    public class ProjectUserReport
    {
        public List<TimeSheetReviewDto> timeSheets { get; set; }
        public int PageCount { get; set; }
        public int CurrentPageIndex { get; set; }
        public int PageSize { get; set; }
    }


}

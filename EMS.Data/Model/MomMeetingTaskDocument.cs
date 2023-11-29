using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Data
{
   public partial class MomMeetingTaskDocument
    {
        public int Id { get; set; }
        public string DocumentPath { get; set; }
        public DateTime AddedDate { get; set; }
        public int? MomMeetingTaskId { get; set; }
    }
}

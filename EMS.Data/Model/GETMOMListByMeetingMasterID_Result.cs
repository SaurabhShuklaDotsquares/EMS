using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Data.Model
{
    
    public partial class GETMOMListByMeetingMasterID_Result
    {
        public int Id { get; set; }
        public string MeetingTitle { get; set; }
        public int MeetingMasterID { get; set; }
        public string Agenda { get; set; }
        public string VenueName { get; set; }
        public System.DateTime DateOfMeeting { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public int AuthorByUid { get; set; }
        public string AuthorBy { get; set; }
        public int ChairedByUID { get; set; }
        public string ChairedBy { get; set; }
        public string participants { get; set; }
        public string MomDocuments { get; set; }
        public int MeetingTime { get; set; }

    }
}

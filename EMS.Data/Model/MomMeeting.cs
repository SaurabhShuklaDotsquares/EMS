using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class MomMeeting
    {
        public MomMeeting()
        {
            MomMeetingDepartment = new HashSet<MomMeetingDepartment>();
            MomMeetingParticipant = new HashSet<MomMeetingParticipant>();
            MomMeetingTasks = new HashSet<MomMeetingTask>();
            MomMeetingTaskTimeLines = new HashSet<MomMeetingTaskTimeLine>();
            Momdocuments = new HashSet<Momdocument>();
        }

        public int Id { get; set; }
        public int MeetingMasterID { get; set; }
        public DateTime DateOfMeeting { get; set; }
        public int MeetingTime { get; set; }
        public string VenueName { get; set; }
        public string Agenda { get; set; }
        public string Notes { get; set; }
        public int ChairedByUID  { get; set; }
        public int AuthorByUID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public byte ParticipantType { get; set; }
        public string MeetingTitle { get; set; }
        public TimeSpan? MeetingStartTime { get; set; }

        public virtual UserLogin UserLogin  { get; set; }
        public virtual UserLogin UserLogin1  { get; set; }
        public virtual MeetingMaster MeetingMaster { get; set; }
        public virtual ICollection<MomMeetingDepartment> MomMeetingDepartment { get; set; }
        public virtual ICollection<MomMeetingParticipant> MomMeetingParticipant { get; set; }
        public virtual ICollection<MomMeetingTask> MomMeetingTasks { get; set; }
        public virtual ICollection<MomMeetingTaskTimeLine> MomMeetingTaskTimeLines { get; set; }
        public virtual ICollection<Momdocument> Momdocuments { get; set; } 
    }
}

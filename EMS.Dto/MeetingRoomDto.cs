using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{
    public class MeetingRoomDto
    {
        public bool IsDeletedAllow;
        public string ThemeColor;

        public string Description { get; set; }
        public DateTime End { get; set; }
        public string EventID { get; set; }
        public string Attendee { get; set; }
        public string Subject { get; set; }
        public DateTime Start { get; set; }
        public string SubjectTitle { get; set; }
        public string CreatedBy { get; set; }
        public string Location { get; set; }
        public int ConferenceRoomId { get; set; }
        public string MeetingSubject { get; set; }
        public string TimeDifference { get; set; }
        public string MeetingTime { get; set; }
        public string StartDateFormat { get { return Start.ToString("yyyy-MM-dd HH:mm"); } }
        public string EndDateFormat { get { return End.ToString("yyyy-MM-dd HH:mm"); } }
    }

    public class BooKMeetingRoomDto
    {
        public string Subject { get; set; }
        public int EventID { get; set; }
        public int ConferenceRoomId { get; set; }
        public string Description { get; set; }
        public DateTime End { get; set; }
        public DateTime Start { get; set; }
        public string AttendeeName { get; set; }
        public string Title { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Date { get; set; }
        public List<CommonListType> TimeSlot { get; set; }
        public List<SelectListItem> ConferenceRoomList { get; set; }
        public List<SelectListItem> OfficeList { get; set; }
    }

    public class CommonListType
    {
        public int Slot_Id { get; set; }
        public string Text { get; set; }
        public bool IsSelect { get; set; }
    }

    public class AddMeetingRoomDto
    {
        public int ConferenceRoomId { get; set; }
        public string CompanyOfficeId { get; set; }
        public string Name { get; set; }
        public string ThemeColor { get; set; }
    }

    public class ColorRoomsDto
    {
        public string MeetingRoom { get; set; }
        public string Class { get; set; }
    }
}

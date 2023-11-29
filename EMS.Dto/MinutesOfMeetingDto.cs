using EMS.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using static EMS.Core.Enums;

namespace EMS.Dto {
    public class MeetingMasterDto {
        public int Id { get; set; }
        public string Title { get; set; }
        public int CreateByUid { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
    public class MomMeetingDto {
        public MomMeetingDto() {
            PaticipantList = new List<SelectListItem>();
            GroupList = new List<SelectListItem>();
            ChairedByList = new List<SelectListItem>();
            AuthorByList = new List<SelectListItem>();
            MomDocuments = new List<MomdocumentDto>();
            
        }
        public int Id { get; set; }
        public int MeetingMasterID { get; set; }
        public DateTime DateOfMeeting { get; set; }
        public string DateOfMeetings { get; set; }
        public int MeetingTime { get; set; }
        public string VenueName { get; set; }
        public string Agenda { get; set; }
        public string Notes { get; set; }
        public int ChairedByUID { get; set; }
        public int AuthorByUID { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public MomMeetingParticipantType ParticipantType { get; set; }
        public string MeetingTitle { get; set; }
        public int MeetingMaster { get; set; }
        public string MeetingMasterTitle { get; set; }
        public int[] Paticipants { get; set; }
        public int[] Groups { get; set; }
        public int ChairedBy { get; set; }
        public string MeetingStartTime { get; set; }
        //public List<TaskAssignedTo> TaskAssigntoes { get; set; }
        public List<SelectListItem> PaticipantList { get; set; }
        public List<SelectListItem> GroupList { get; set; }

        public List<SelectListItem> ChairedByList { get; set; }

        public List<SelectListItem> AuthorByList { get; set; }

        public MomMeetingStatus Status { get; set; }

        public bool IsComplete { get; set; }

        public string selectedParticpants { get; set; }

        // public string selectedParticpantsForEmails { get; set; }

        public string selectedGroup { get; set; }
        public bool SendEmail { get; set; }

        public List<MomdocumentDto> MomDocuments { get; set; }
        public string StrPaticipant { get; set; }
        public string ChairedName { get; set; }
        public List<MomMeetingTaskDto> MomMeetingTasks { get; set; }
    }

    public class MomMeetingTaskDto {
        public MomMeetingTaskDto() {
            PaticipantList = new List<SelectListItem>();
            MomMeetingTaskDocuments = new List<MomMeetingTaskDocumentDto>();
        }
        public int Id { get; set; }
        public int MomMeetingId { get; set; }
        public string MeetingTitle { get; set; }
        public string Task { get; set; }
        public MomMeetingStatus Status { get; set; }
        public string Remark { get; set; }
        public int[] Paticipants { get; set; }
        public string PaticipantsList { get; set; }
        public DateTime? TargetDate { get; set; }
        public string TargetDates { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public List<SelectListItem> PaticipantList { get; set; }
        public List<MomMeetingTaskDocumentDto> MomMeetingTaskDocuments { get; set; }
        public int CommentByUId { get; set; }

        public Priority Priority { get; set; }
        public string Decision { get; set; }


    }
    public class MomMeetingTaskTimeLineDto {
        public int Id { get; set; }
        public int CommentedByUid { get; set; }
        public string CommentedBy { get; set; }
        public string MeetingTitle { get; set; }
        public int MomMeetingTaskId { get; set; }
        public int MomMeetingId { get; set; }
        public MomMeetingStatus Status { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public decimal? TaskCommentId { get; set; }

    }
    public class MomMeetingTaskCommentsDto {
        public MomMeetingTaskCommentsDto() {
            MomMeetingTask = new MomMeetingTaskDto();
            MomMeetingTaskTimeLineDto = new List<MomMeetingTaskTimeLineDto>();
        }
        public MomMeetingTaskDto MomMeetingTask { get; set; }
        public List<MomMeetingTaskTimeLineDto> MomMeetingTaskTimeLineDto { get; set; }
    }

    public class MomMeetingTaskDecisionDTO {
        public MomMeetingTaskDecisionDTO() {
            MomMeeting = new List<MomMeeting>();
        }

        public List<MomMeeting> MomMeeting { get; set; }
    }


    public class MomMeetingTaskCommentsAddDto {
        public int Id { get; set; }
        public int MomMeetingId { get; set; }
        public int MomMeetingTaskId { get; set; }
        public string PaticipantsList { get; set; }
        public string MeetingTitle { get; set; }
        public string Task { get; set; }
        public string TargetDate { get; set; }
        public string CurrentStatus { get; set; }
        public string Remark { get; set; }
        public MomMeetingStatus Status { get; set; }
        public string Comment { get; set; }

        public string LastComment { get; set; }
        public int CommentedByUid { get; set; }

        public string TargetDates { get; set; }

        public string Decision { get; set; }
        public int[] Paticipants { get; set; }

        public List<SelectListItem> PaticipantList { get; set; }
        public Priority Priority { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class LeadNotesDto
    {
        public string LeadId { get; set; }
        public string NextChaseDate { get; set; }
        public List<LeadNote> LeadNoteList { get; set; }
        public LeadNotesDto()
        {
            LeadNoteList = new List<LeadNote>();
        }
    }

    public class LeadNote
    {
        public int SNo { get; set; }
        public int lead_id { get; set; }
        public string notes_details { get; set; }
        public string notes_time { get; set; }
        public string user_name { get; set; }
    }

}

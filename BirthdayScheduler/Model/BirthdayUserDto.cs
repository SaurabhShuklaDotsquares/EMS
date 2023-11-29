using System;
using System.Collections.Generic;
using System.Text;

namespace BirthdayScheduler.Model
{
   public class BirthdayUserDto
    {
        public int Uid { get; set; }
        public string UserName { get; set; }
        public DateTime? DOB { get; set; }
        public int? PMUid { get; set; }
        public int? Role { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string EmailOffice { get; set; }
        public string EmailPersonal { get; set; }
        public long Mobile { get; set; }
        public DateTime? MarriageDate { get; set; }
        public string Gender { get; set; }
        public string ProfilePicture { get; set; }
        public bool Isactive { get; set; }
        public bool IsResigned { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class AvailUser
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Uid { get; set; }
        public int UserId { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModify { get; set; }
        public string Ip { get; set; }
        public bool? IsCurrent { get; set; }

        public virtual UserLogin U { get; set; }
        public virtual UserLogin User { get; set; }
    }
}

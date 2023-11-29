using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Client
    {
        public Client()
        {
            Forecasting = new HashSet<Forecasting>();
            Project = new HashSet<Project>();
        }

        public int ClientId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Msn { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string Ip { get; set; }
        public int? PMUid { get; set; }

        public virtual ICollection<Forecasting> Forecasting { get; set; }
        public virtual ICollection<Project> Project { get; set; }
    }
}

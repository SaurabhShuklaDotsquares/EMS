using System;
using System.Collections.Generic;

namespace EMS.Data.Model
{
    public partial class UserWebNotificationToken
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual UserLogin User { get; set; }
    }
}

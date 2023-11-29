using EMS.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EMS.Web.Code.LIBS
{
    public static class SelectListHelper
    {
        public static SelectList GetLeaveStatus(string selected = "")
        {
            var leaveStatus = new SelectList(new[]
            {
                new {ID=(int)Enums.LeaveStatus.Pending, Name="Pending"},
                new {ID=(int)Enums.LeaveStatus.Approved, Name="Approved"},
                new {ID=(int)Enums.LeaveStatus.Cancelled, Name="Cancelled"},
                new {ID=(int)Enums.LeaveStatus.UnApproved, Name="UnApproved"}
            }, "ID", "Name", (int)Enums.LeaveStatus.Pending);
            return leaveStatus;
        }

    }
}
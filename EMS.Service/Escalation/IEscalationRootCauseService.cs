using EMS.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service
{
    public interface IEscalationRootCauseService 
    {
        List<EscalationRootCause> GetEscalationsRootCauseList();
    }
}

using EMS.Data;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMS.Service
{
    public class EscalationRootCauseService : IEscalationRootCauseService
    {
        private readonly IRepository<EscalationRootCause> repoEscalationRootCause;

        public EscalationRootCauseService(IRepository<EscalationRootCause> _repoEscalationRootCause)
        {
            repoEscalationRootCause = _repoEscalationRootCause;
        }

        public List<EscalationRootCause> GetEscalationsRootCauseList()
        {
            return repoEscalationRootCause.Query().Filter(x=>(bool)x.IsActive).Get().ToList();
        }


        //public void Dispose()
        //{
        //    throw new NotImplementedException();
        //}
    }
}

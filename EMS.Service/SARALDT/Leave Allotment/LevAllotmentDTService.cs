using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EMS.Repo;
using EMS.Data.saralDT;

namespace EMS.Service.SARALDT
{
    public class LevAllotmentDTService : ILevAllotmentDTService
    {
        private IRepository<LevAllotment> repoLevAllotment;
        public LevAllotmentDTService(IRepository<LevAllotment> repoLevAllotment)
        {
            this.repoLevAllotment = repoLevAllotment;
        }

        public List<LevAllotment> GetDTLeaveAllotmentByAttendanceId(int? AttendanceId)
        {
            return repoLevAllotment.Query().Filter(x => x.Empid == AttendanceId).GetSaralDT().ToList();
        }
        public int GetDTMaxAllotmentValue(int AttendanceId)
        {
            return repoLevAllotment.Query().Filter(x => x.Empid == AttendanceId).GetSaralDT().Max(x => x.Empdetid);
        }
        public void Dispose()
        {
            if (repoLevAllotment != null)
            {
                repoLevAllotment.Dispose();
                repoLevAllotment = null;
            }
        }
    }
}

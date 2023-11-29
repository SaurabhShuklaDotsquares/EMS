using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EMS.Repo;
using EMS.Data.saral;

namespace EMS.Service.SARAL
{
    public class LevAllotmentService : ILevAllotmentService
    {
        private IRepository<LevAllotment> repoLevAllotment;
        public LevAllotmentService(IRepository<LevAllotment> repoLevAllotment)
        {
            this.repoLevAllotment = repoLevAllotment;
        }

        public List<LevAllotment> GetLeaveAllotmentByAttendanceId(int? AttendanceId)
        {
            return repoLevAllotment.Query().Filter(x => x.Empid == AttendanceId).GetSaral().ToList();
        }
        public int GetMaxAllotmentValue(int AttendanceId)
        {
            return repoLevAllotment.Query().Filter(x => x.Empid == AttendanceId).GetSaral().Max(x => x.Empdetid);
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

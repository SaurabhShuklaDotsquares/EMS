using EMS.Data.saral;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMS.Service.SARAL
{
    public class LevMonthdetService : ILevMonthdetService
    {
        private IRepository<LevMonthdet> repoLevMonthdet;
        public LevMonthdetService(IRepository<LevMonthdet> repoLevMonthdet)
        {
            this.repoLevMonthdet = repoLevMonthdet;
        }

        public List<LevMonthdet> GetLeaveMonthDetailsByAttendanceId(int? AttendanceId)
        {
            return repoLevMonthdet.Query().Filter(x => x.Empid == AttendanceId).GetSaral().ToList();
        }
        public LevMonthdet GetLeaveMonthDetail(int? AttendanceId, int monthYear)
        {
            return repoLevMonthdet.Query().Filter(x => x.Empid == AttendanceId && x.Monthyear == monthYear).GetSaral().FirstOrDefault();
        }
        public void Save(LevMonthdet entity)
        {
            //if (entity.Empid != 0)
            //{
            //    repoLevMonthdet.ChangeEntityStateSaral<LevDetails>(entity, ObjectState.Added);
            //    repoLevMonthdet.InsertGraphSaral(entity);
            //}
            if (entity.Empid > 0 && entity.Monthyear > 0)
            {
                repoLevMonthdet.ChangeEntityStateSaral<LevMonthdet>(entity, ObjectState.Modified);
                repoLevMonthdet.SaveChangesSaral();
            }
        }
        public void Dispose()
        {
            if (repoLevMonthdet != null)
            {
                repoLevMonthdet.Dispose();
                repoLevMonthdet = null;
            }
        }
    }
}

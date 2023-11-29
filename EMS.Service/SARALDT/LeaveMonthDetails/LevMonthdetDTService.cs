using EMS.Data.saralDT;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMS.Service.SARALDT
{
    public class LevMonthdetDTService : ILevMonthdetDTService
    {
        private IRepository<LevMonthdet> repoLevMonthdet;
        public LevMonthdetDTService(IRepository<LevMonthdet> repoLevMonthdet)
        {
            this.repoLevMonthdet = repoLevMonthdet;
        }

        public List<LevMonthdet> GetLeaveMonthDetailsByAttendanceId(int? AttendanceId)
        {
            return repoLevMonthdet.Query().Filter(x => x.Empid == AttendanceId).GetSaralDT().ToList();
        }
        public LevMonthdet GetLeaveMonthDetail(int? AttendanceId, int monthYear)
        {
            return repoLevMonthdet.Query().Filter(x => x.Empid == AttendanceId && x.Monthyear == monthYear).GetSaralDT().FirstOrDefault();
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
                repoLevMonthdet.ChangeEntityStateSaralDT<LevMonthdet>(entity, ObjectState.Modified);
                repoLevMonthdet.SaveChangesSaralDT();
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

using EMS.Data;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMS.Service
{
    public class TeamHierarchyService : ITeamHierarchyService
    {
        #region "Fields"
        private IRepository<UserLogin> repoUserMaster;
        private IRepository<TeamHierarchy> repoTeamHierarchy;

        #endregion
        #region "Constructor"
        public TeamHierarchyService(IRepository<UserLogin> _repoUserMaster, IRepository<TeamHierarchy> _repoTeamHierarchy)
        {
            this.repoUserMaster = _repoUserMaster;
            this.repoTeamHierarchy = _repoTeamHierarchy;
        }
        #endregion
        public void Dispose()
        {
            if (repoTeamHierarchy != null)
            {
                repoTeamHierarchy.Dispose();
                repoTeamHierarchy = null;
            }
        }

        public List<TeamHierarchy> GetTeamHierarchyList()
        {
            return repoTeamHierarchy.Query().Get().ToList();
        }
        public TeamHierarchy GetTeamHierarchyById(int Id)
        {
            return repoTeamHierarchy.Query().Filter(x => x.Id == Id).Get().FirstOrDefault();
        }

        public TeamHierarchy GetTeamHierarchyByTlId(int TlId)
        {

            return repoTeamHierarchy.Query().Filter(x => x.TlId == TlId).Get().OrderByDescending(x => x.ModifyDate).FirstOrDefault();

        }

        public int GetMemberCountOnDate(int Id, DateTime date, bool isAllTeam = false)
        {
            if (isAllTeam)
            {
                return repoTeamHierarchy.Query().Filter(x => x.Pmuid == Id && x.AddDate.Date == date.Date && x.IsAllTeam == true).Get().Select(x => x.TotalEmployees).FirstOrDefault();
            }
            else
            {
                return repoTeamHierarchy.Query().Filter(x => x.TlId == Id && x.AddDate.Date == date.Date).Get().Select(x => x.TotalEmployees).FirstOrDefault();
            }
        }
        public void SaveTeamHierarchy(TeamHierarchy entity)
        {
            if (entity != null)
            {
                var data = GetTeamHierarchyByTlId(entity.TlId);
                if (data != null && data.Id > 0)
                {
                    var teamHierarchyEntity = repoTeamHierarchy.FindById(data.Id);
                    repoTeamHierarchy.Update(teamHierarchyEntity, entity);
                }
                else
                {
                    repoTeamHierarchy.InsertGraph(entity);
                }
                repoTeamHierarchy.SaveChanges();
            }
        }
    }
}

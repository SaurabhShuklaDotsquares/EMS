using EMS.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service
{
    public interface ITeamHierarchyService : IDisposable
    {
        List<TeamHierarchy> GetTeamHierarchyList();
        TeamHierarchy GetTeamHierarchyById(int Id);
        TeamHierarchy GetTeamHierarchyByTlId(int TlId);
        int GetMemberCountOnDate(int Id, DateTime date,bool isAllTeam = false);
        void SaveTeamHierarchy(TeamHierarchy entity);
    }
}

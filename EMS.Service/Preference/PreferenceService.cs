using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Service
{
    public class PreferenceService : IPreferenceService
    {
        #region "Fields"
        private IRepository<Preference> repoPreference;
        #endregion

        #region "Cosntructor"
        public PreferenceService(IRepository<Preference> _repoPreference)
        {
            this.repoPreference = _repoPreference;

        }
        #endregion

        public Preference GetDataByPmid(int pmid)
        {
            return repoPreference.Query().Filter(P => P.pmid == pmid).Get().FirstOrDefault();

        }

        public void Save(Preference entity)
        {
            if (entity.PreferenceId == 0)
            {
                repoPreference.ChangeEntityState<Preference>(entity, ObjectState.Added);
                repoPreference.InsertGraph(entity);
            }
            else
            {
                repoPreference.ChangeEntityState<Preference>(entity, ObjectState.Modified);
                repoPreference.SaveChanges();
            }

        }

        #region "Dispose"
        public void Dispose()
        {
            if (repoPreference != null)
            {
                repoPreference.Dispose();
                repoPreference = null;
            }
        }
        #endregion

    }
}

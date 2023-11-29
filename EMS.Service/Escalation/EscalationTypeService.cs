using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using EMS.Repo;
using EMS.Dto;
using EMS.Core;

namespace EMS.Service
{
    public class EscalationTypeService:IEscalationTypeService
    {
        #region "Fields"
        private IRepository<EscalationType> repoEscalationType;
        #endregion

        #region "Cosntructor"
        public EscalationTypeService(IRepository<EscalationType> _repoEscalationType)
        {
            this.repoEscalationType = _repoEscalationType;
        }
        #endregion
        public List<EscalationType> GetEscalationTypeList()
        {
            return repoEscalationType.Query().Filter(x => x.IsActive == true).Get().ToList();
        }
        public EscalationType GetEscalationTypeById(int id)
        {
            return repoEscalationType.Query().Get().FirstOrDefault(x => x.Id == id);
        }

        public void Dispose()
        {
            if (repoEscalationType != null)
            {
                repoEscalationType.Dispose();
            }
        }
    }
}

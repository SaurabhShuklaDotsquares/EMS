using EMS.Data;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service
{
    public class TypeMasterService: ITypeMasterService
    {

        #region "Fields"
        private IRepository<TypeMaster> repo;
        #endregion

        #region "Cosntructor"
        public TypeMasterService(IRepository<TypeMaster> _repo)
        {
            this.repo = _repo;
        }

        public List<TypeMaster> GetType(string TypeGroup)
        {            
            return repo.Query().Filter(T => T.TypeGroup.ToLower() == TypeGroup.ToLower()).Get().ToList();
        }
        #endregion
    }
}

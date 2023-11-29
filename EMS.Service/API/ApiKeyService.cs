using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using EMS.Repo;

namespace EMS.Service
{
    public class ApiKeyService : IApiKeyService
    {
        #region "Fields"
        private IRepository<ServiceAuth> repoServiceAuth;
        #endregion

        #region "Cosntructor"
        public ApiKeyService(IRepository<ServiceAuth> _repoServiceAuth)
        {
            this.repoServiceAuth = _repoServiceAuth;
        }
        #endregion
        public ServiceAuth GetApiKey()
        {
            return repoServiceAuth.Query().Get().FirstOrDefault();
        }

        #region "Dispose"
        public void Dispose()
        {
            if (repoServiceAuth != null)
            {
                repoServiceAuth.Dispose();
                repoServiceAuth = null;
            }
        }
        #endregion
    }
}

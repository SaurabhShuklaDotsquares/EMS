using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using EMS.Repo;

namespace EMS.Service
{
    public class LoginService : ILoginService
    {
        #region "Fields"
        private IRepository<UserLogin> repoUserMaster;
        #endregion

        #region "Cosntructor"
        public LoginService(IRepository<UserLogin> _repoUserMaster)
        {
            this.repoUserMaster = _repoUserMaster;
        }
        #endregion
        public UserLogin GetLoginDeatils(string email, string password)
        {
            return repoUserMaster.Query().Filter(x => (x.UserName.ToLower() == email.ToLower() || x.EmailOffice.ToLower() == email.ToLower()) && x.IsActive == true && String.Compare(x.PasswordKey, password, false) == 0).Get().FirstOrDefault();
        }

        public UserLogin GetLoginDeatilByEmail(string email)
        {
            return repoUserMaster.Query().Filter(x => x.EmailOffice.ToLower() == email.ToLower()).Get().FirstOrDefault();
        }

        public UserLogin GetLoginDeatilByUserName(string username)
        {
            return repoUserMaster.Query().Filter(x => x.UserName.ToLower() == username.ToLower() || x.EmailOffice.ToLower() == username.ToLower() && x.IsActive == true).Get().FirstOrDefault();
        }
        public UserLogin GetLoginDeatilByUserNameOREmail(string username)
        {
            return repoUserMaster.Query().Filter(x => x.UserName.ToLower() == username.ToLower() || x.EmailOffice.ToLower() == username.ToLower()).Get().FirstOrDefault();
        }

        public void save(UserLogin user)
        {
            repoUserMaster.SaveChanges();
        }

        #region "Dispose"
        public void Dispose()
        {
            if (repoUserMaster != null)
            {
                repoUserMaster.Dispose();
                repoUserMaster = null;
            }
        }
        #endregion
        public void ChangePassword(UserLogin userLogin)
        {
            if (userLogin != null)
            {
                repoUserMaster.ChangeEntityState<UserLogin>(userLogin, ObjectState.Modified);
                repoUserMaster.SaveChanges();
            }
        }
        public List<UserLogin> GetPasswordKeyList()
        {
            return repoUserMaster.Query().Filter(x => string.IsNullOrEmpty(x.PasswordKey.ToLower()) && x.IsActive == true).Get().ToList();
        }

        public void Updatepasswordkey(List<UserLogin> userLogin)
        {   
                repoUserMaster.ChangeEntityCollectionState(userLogin.ToList(), ObjectState.Modified);
                repoUserMaster.SaveChanges();
         
        }

    }
}

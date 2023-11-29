using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Service
{
    public interface ILoginService : IDisposable
    {
        UserLogin GetLoginDeatils(string email, string password);
        UserLogin GetLoginDeatilByEmail(string email);

        UserLogin GetLoginDeatilByUserName(string name);

        UserLogin GetLoginDeatilByUserNameOREmail(string username);

        void save(UserLogin user);
        List<UserLogin> GetPasswordKeyList();
        void Updatepasswordkey(List<UserLogin> userLogin);
       
    }
}

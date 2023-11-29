using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using EMS.Repo;
using EMS.Dto;
using System.Linq.Expressions;

namespace EMS.Service
{
    public interface IAppraiseService : IDisposable
    {
        List<EmployeeAppraise> GetData();
        List<EmployeeAppraise> GetAppraiseByPaging(out int total, PagingService<EmployeeAppraise> pagingSerices);
        EmployeeAppraise GetAppraiseData(int id);
        void Delete(int id);
        EmployeeAppraise Save(AppraiseDto model);
        List<EmployeeAppraise> GetAppraises(Expression<Func<EmployeeAppraise, bool>> filter);

        List<EmployeeAppraise> GetAppraiseUserIdById(int id);
        List<EmployeeAppraise> GetAppraiseUserIdByPMId(int id);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;

namespace EMS.Service
{
  public interface IDepartmentService : IDisposable
    {
        List<Department> GetDepartments();
        Department GetDepartmentByCode(string deptCode);
        List<Department> GetDepartments(PagingService<Department> pagingService);

        Department DepartmentFindById(int deptId);
        Department GetDepartmentById(int deptId);
        List<Department> GetActiveDepartments();
        bool Save(Department depatment);
        List<string> GetNames(List<int> depId);
    }
}

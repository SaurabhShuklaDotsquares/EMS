using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using EMS.Repo;

namespace EMS.Service
{
    public class DepartmentService : IDepartmentService
    {
        #region "Fields"
        private IRepository<Department> repoDepartment;
        #endregion

        #region "Cosntructor"
        public DepartmentService(IRepository<Department> _repoDepartment)
        {
            this.repoDepartment = _repoDepartment;
        }
        #endregion
        public Department GetDepartmentByCode(string deptCode)
        {
            return repoDepartment.Query().Filter(x => x.Deptcode == deptCode).Get().FirstOrDefault();
        }
        public List<Department> GetDepartments()
        {
            return repoDepartment.Query().Get().ToList();
        }
        public List<Department> GetDepartments(PagingService<Department> pagingService)
        {
            return repoDepartment.Query()
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort)
                .Get()
                .ToList();
            //.GetPage(pagingService.Start, pagingService.Length, out total)

        }

        public Department DepartmentFindById(int deptId)
        {
            return repoDepartment.FindById(deptId);
        }

        public Department GetDepartmentById(int deptId)
        {
            return repoDepartment.Query().Get().FirstOrDefault(x => x.DeptId == deptId);
        }

        public bool Save(Department department)
        {
            bool isExist;
            isExist = repoDepartment.Query().Get().Any(entity => entity.DeptId != department.DeptId && (entity.Name.ToLower().Equals(department.Name.ToLower()) || entity.Deptcode.ToLower().Equals(department.Deptcode.ToLower())));
            if (!isExist)
            {
                if (department.DeptId != 0)
                {
                    repoDepartment.ChangeEntityState<Department>(department, ObjectState.Modified);
                }
                else
                {
                    repoDepartment.ChangeEntityState<Department>(department, ObjectState.Added);
                }
                repoDepartment.SaveChanges();
                return true;
            }
            return false;
        }

        public List<Department> GetActiveDepartments()
        {
            return repoDepartment.Query().Get().Where(x => x.IsActive == true).ToList();
        }

        #region "Dispose"
        public void Dispose()
        {
            if (repoDepartment != null)
            {
                repoDepartment.Dispose();
                repoDepartment = null;
            }
        }

        public List<string> GetNames(List<int> depId)
        {
            return repoDepartment.Query().Get().Where(a => depId.Contains(a.DeptId)).Select(x=>x.Name).ToList();
        }
        #endregion
    }
}

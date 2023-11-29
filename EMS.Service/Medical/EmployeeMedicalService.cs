using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
namespace EMS.Service
{
    public class EmployeeMedicalService : IEmployeeMedicalService
    {

        private readonly IRepository<EmployeeMedicalData> _repoEmployeeMedicalData;

        public EmployeeMedicalService(IRepository<EmployeeMedicalData> repoEmployeeMedicalData)
        {
            _repoEmployeeMedicalData = repoEmployeeMedicalData;
        }

        public ICollection<EmployeeMedicalData> GetEmployeeMedicals(bool isActive = false)
        {
            return _repoEmployeeMedicalData.Query().Filter(e => (e.IsActive || e.IsActive == isActive) && e.UserLogin.IsActive==true).Get().ToList();
        }
        public List<EmployeeMedicalData> GetEmployeeMedicals(out int total, PagingService<EmployeeMedicalData> pagingService)
        {
            return _repoEmployeeMedicalData.Query()
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort)
                .GetPage(pagingService.Start, pagingService.Length, out total).ToList();
        }
        public EmployeeMedicalData GetEmployeeMedicalData(int id)
        {
            return _repoEmployeeMedicalData.FindById(id);
        }

        public EmployeeMedicalData GetEmployeeMedicalDataByUserid(int Uid)
        {
            return _repoEmployeeMedicalData.Query().Filter(u => u.UserId == Uid).AsTracking().Get().FirstOrDefault();
        }
        public EmployeeMedicalData Save(EmployeeMedicalData employeeMedicalData, bool isNew = true)
        {
            if (employeeMedicalData.Id == 0)
            {
                _repoEmployeeMedicalData.InsertGraph(employeeMedicalData);
            }
            else
            {
                foreach (var emp in employeeMedicalData.EmployeeRelativeMedicalDatas.Where(x => x.Id == 0))
                {
                    _repoEmployeeMedicalData.ChangeEntityState<EmployeeRelativeMedicalData>(emp, ObjectState.Added);
                }
                foreach (var emp in employeeMedicalData.EmployeeRelativeMedicalDatas.Where(x => x.Id > 0))
                {
                    _repoEmployeeMedicalData.ChangeEntityState<EmployeeRelativeMedicalData>(emp, ObjectState.Modified);
                }

                var EmployeeMedicalEntity = _repoEmployeeMedicalData.FindById(employeeMedicalData.Id);
                var EmployeeMedicalpdated = _repoEmployeeMedicalData.Update(EmployeeMedicalEntity, employeeMedicalData);
                _repoEmployeeMedicalData.SaveChanges();
            }

            return employeeMedicalData;
        }
        public bool Delete(int id)
        {
            _repoEmployeeMedicalData.Delete(id);
            return true;
        }
        public bool Active(int id)
        {
            var category = _repoEmployeeMedicalData.FindById(id);
            if (category != null)
            {
                category.IsActive = !(category.IsActive);
                Save(category, false);
            }
            return true;
        }

        public void Dispose()
        {
            if (_repoEmployeeMedicalData != null)
            {
                _repoEmployeeMedicalData.Dispose();
            }
        }

    }
}

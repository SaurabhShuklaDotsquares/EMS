using EMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service
{
   public interface IEmployeeMedicalService:IDisposable
    {

        ICollection<EmployeeMedicalData> GetEmployeeMedicals(bool isActive = false);
        List<EmployeeMedicalData> GetEmployeeMedicals(out int total, PagingService<EmployeeMedicalData> pagingService);
        EmployeeMedicalData GetEmployeeMedicalData(int id);
        EmployeeMedicalData GetEmployeeMedicalDataByUserid(int Uid);
        EmployeeMedicalData Save(EmployeeMedicalData employeeMedicalData, bool isNew = true);
        bool Delete(int id);
        bool Active(int id);
    }
}

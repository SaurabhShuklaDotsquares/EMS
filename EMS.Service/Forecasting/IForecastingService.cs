using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;

namespace EMS.Service
{
    public interface IForecastingService : IDisposable
    {
        List<Forecasting> GetForecastingList(out int total, PagingService<Forecasting> pagingService);
        void Delete(int id);
        void ChangeStatus(int id, int status);
        List<Project> GetProjects();
        List<ProjectLead> GetProjectLeads();
        List<Department> GetDepartment();
        Forecasting GetForecastingById(int id);
        void SaveForecasting(Forecasting forecastingDB);
        List<Client> GetClientData(string[] clientId);
        void SaveDepartmentList();
        void AddForecastingDepartment(Department department);
        //double GetTotalForecasting(PagingService<Forecasting> pagingService);
    }
}

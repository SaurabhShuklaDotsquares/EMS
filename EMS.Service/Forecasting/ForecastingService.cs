using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using EMS.Repo;
using System.Linq.Expressions;
using EMS.Core;

namespace EMS.Service
{
    public class ForecastingService : IForecastingService
    {
        #region "Fields"
        private IRepository<Forecasting> repoForecasting;
        private IRepository<ProjectLead> repoProjectLead;
        private IRepository<Department> repoDepartment;
        private IRepository<Project> repoProject;
        private IRepository<Client> repoClient;
        #endregion

        #region "Cosntructor"
        public ForecastingService(IRepository<Forecasting> _repoForecasting, IRepository<ProjectLead> _repoProjectLead, IRepository<Department> _repoDepartment, IRepository<Project> _repoProject, IRepository<Client> _repoClient)
        {
            this.repoForecasting = _repoForecasting;
            this.repoProjectLead = _repoProjectLead;
            this.repoDepartment = _repoDepartment;
            this.repoProject = _repoProject;
            this.repoClient = _repoClient;
        }
        #endregion

        public List<Forecasting> GetForecastingList(out int total, PagingService<Forecasting> pagingService)
        {
            return repoForecasting.Query()
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort)
                .GetPage(pagingService.Start, pagingService.Length, out total)
                .ToList();
        }

        public void Delete(int id)
        {
            var obj = repoForecasting.FindById(id);
            /*Changed By Tabassum */
            // foreach (Department dept in obj.Departments.ToList())
            foreach (ForecastingDepartment dept in obj.ForecastingDepartment.ToList())
            {
                //obj.Departments.Remove(dept);
                //  obj.ForecastingDepartment.Select(q=>q.Department).ToList().Remove(dept);
                obj.ForecastingDepartment.Remove(dept);


            }
            /*END*/
            repoForecasting.Delete(id);
        }
        public void ChangeStatus(int id, int status)
        {
            var obj = repoForecasting.FindById(id);
            obj.Status = status;
            repoForecasting.SaveChanges();
        }

        public List<ProjectLead> GetProjectLeads()
        {
            return repoProjectLead.Query().Get().ToList();
        }
        public List<Project> GetProjects()
        {
            return repoProject.Query().Get().OrderByDescending(T => T.ModifyDate).ToList();
        }
        public List<Department> GetDepartment()
        {
            return repoDepartment.Query().Get().OrderBy(R => R.Name).ToList();
        }
        public List<Client> GetClientData(string[] clientId)
        {
            var names = clientId;
            return repoClient.Query().Get().Where(P => (!clientId.Contains(P.ClientId.ToString()))).OrderByDescending(P => P.ModifyDate).ToList();
        }
        public Forecasting GetForecastingById(int id)
        {
            return repoForecasting.FindById(id);
        }
        public void SaveForecasting(Forecasting entity)
        {
            if (entity.Id == 0)
            {
                repoForecasting.ChangeEntityState<Forecasting>(entity, ObjectState.Added);
                repoForecasting.InsertGraph(entity);
            }
            else
            {
                repoForecasting.ChangeEntityState<Forecasting>(entity, ObjectState.Modified);
                repoForecasting.SaveChanges();
            }
        }

        public void AddForecastingDepartment(Department department)
        {
            repoDepartment.ChangeEntityState<Department>(department, ObjectState.Added);
            repoDepartment.InsertGraph(department);
        }

        public void SaveDepartmentList()
        {
            repoDepartment.SaveChanges();
        }
        //public double GetTotalForecasting(PagingService<Forecasting> pagingService)
        //{
        //    return (repoForecasting.Query()
        //        .Filter(pagingService.Filter).GetQuerable().Count());
        //}

        #region "Dispose"
        public void Dispose()
        {
            if (repoForecasting != null)
            {
                repoForecasting.Dispose();
                repoForecasting = null;
            }
        }
        #endregion
    }
}

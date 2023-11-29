using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using EMS.Repo;

namespace EMS.Service
{
    public class ComponentService : IComponentService
    {
        #region "Fileds"
        private IRepository<Component> repoComponent;
        #endregion

        #region "Constructor"
        public ComponentService(IRepository<Component> repoComponent)
        {
            this.repoComponent = repoComponent;
        }
        #endregion


        public List<Component> GetComponentByPaging(out int total, PagingService<Component> pagingServices)
        {
            return repoComponent.Query().
                Filter(pagingServices.Filter).
                OrderBy(pagingServices.Sort).
                GetPage(pagingServices.Start, pagingServices.Length, out total).
                ToList();
        }

        public bool Save(Component entity)
        {
            if (entity.Id == 0)
            {
                repoComponent.ChangeEntityState(entity, ObjectState.Added);
            }
            else
            {
                repoComponent.ChangeEntityState(entity, ObjectState.Modified);
            }
            repoComponent.SaveChanges();
            return false;
        }

        public Component GetComponentById(int id)
        {
            return repoComponent.Query().Get().FirstOrDefault(x => x.Id == id);
        }

        public void Delete(Component entity)
        {
            if (entity.Id > 0)
            {
                repoComponent.ChangeEntityState<Component>(entity, ObjectState.Deleted);
                repoComponent.Delete(entity.Id);
            }
        }

        public List<Component> GetList()
        {
            List<Component> list = new List<Component>();
            list = repoComponent.Query().Get().OrderByDescending(x => x.Modified).ToList();
            return list;
        }

        #region Dispose
        public void Dispose()
        {
            if (repoComponent != null)
            {
                repoComponent.Dispose();
                repoComponent = null;
            }
        }





        #endregion
    }
}

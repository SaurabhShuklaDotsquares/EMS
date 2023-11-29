using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using EMS.Repo;
using EMS.Core;
namespace EMS.Service
{
    public class CurrentOpeningService : ICurrentOpeningService
    {
        #region "Fields"
        private IRepository<CurrentOpening> repoCurrentOpening;

        #endregion

        #region "Cosntructor"
        public CurrentOpeningService(IRepository<CurrentOpening> _repoCurrentOpening)
        {
            this.repoCurrentOpening = _repoCurrentOpening;
        }


        #endregion

        #region "Dispose"
        public void Dispose()
        {
            if (repoCurrentOpening != null)
            {
                repoCurrentOpening.Dispose();
                repoCurrentOpening = null;
            }
        }
        #endregion
        public CurrentOpening GetCurrentOpeningById(int? id)
        {
            return repoCurrentOpening.Query()
                   .Filter(x => x.Id == id).Get().FirstOrDefault();
        }
        public List<CurrentOpening> GetCurrentOpenings()
        {
            return repoCurrentOpening.Query().Get()
                .ToList();
        }
        public List<CurrentOpening> GetCurrentOpeningByPage(out int total, PagingService<CurrentOpening> pagingService)
        {
            return repoCurrentOpening.Query()
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort)
                .GetPage(pagingService.Start, pagingService.Length, out total)
                .ToList();
        }
        public void Delete(CurrentOpening entity)
        {
            if (entity != null)
            {
                repoCurrentOpening.Delete(entity.Id);
                repoCurrentOpening.SaveChanges();
            }
        }
        public void Save(CurrentOpening entity)
        {
            if (entity.Id == 0)
            {
                repoCurrentOpening.ChangeEntityState<CurrentOpening>(entity, ObjectState.Added);
            }
            else
            {
                repoCurrentOpening.ChangeEntityState<CurrentOpening>(entity, ObjectState.Modified);
            }
            repoCurrentOpening.SaveChanges();
        }

    }
}

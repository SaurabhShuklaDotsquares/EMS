using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EMS.Data;
using EMS.Repo;

namespace EMS.Service.LibraryManagement
{
    public class SalesKitTypeService : ISalesKitTypeService
    {
        private IRepository<SalesKitType> repoSalesKitType;
        public SalesKitTypeService(IRepository<SalesKitType> _repoSalesKitType)
        {
            repoSalesKitType = _repoSalesKitType;
        }
        public void Dispose()
        {
            if (repoSalesKitType != null)
            {
                repoSalesKitType.Dispose();
                repoSalesKitType = null;
            }
        }
        public List<SalesKitType> GetSalesKitTypeByPaging(out int total, PagingService<SalesKitType> pagingServices)
        {
            return repoSalesKitType.Query().Filter(pagingServices.Filter)
                .OrderBy(pagingServices.Sort)
                .GetPage(pagingServices.Start, pagingServices.Length, out total)
                .ToList();
        }

        public List<SalesKitType> GetSalesKitType()
        {
            return repoSalesKitType.Query().Filter(ul => ul.ParentId == 0 && ul.IsActive == true).Get().ToList();
        }

        public List<SalesKitType> GetSubSalesKitType(int SalesKitId)
        {
            return repoSalesKitType.Query().Filter(ul => ul.ParentId == SalesKitId && ul.IsActive == true).Get().ToList();
        }
        public SalesKitType GetSubSalesKitDetail(int Id)
        {
            return repoSalesKitType.Query().Filter(ul => ul.SalesKitId == Id && ul.IsActive == true).Get().FirstOrDefault();
        }
        public SalesKitType GetSalesKitTypeDetail(int Id)
        {
            return repoSalesKitType.Query().Filter(ul => ul.SalesKitId == Id).Get().FirstOrDefault();
        }

        public bool Save(SalesKitType salesKitType)
        {

            if (salesKitType.SalesKitId == 0)
            {
                var AlreadyExist = repoSalesKitType.Query().Filter(p => p.Name.Trim().ToLower().Equals(salesKitType.Name.Trim().ToLower()) && p.ParentId == salesKitType.ParentId).Get().Count();
                if (AlreadyExist == 0)
                {
                    repoSalesKitType.ChangeEntityState<SalesKitType>(salesKitType, ObjectState.Added);
                    repoSalesKitType.SaveChanges();
                    return true;
                }
                return false;
            }
            else
            {
                repoSalesKitType.ChangeEntityState<SalesKitType>(salesKitType, ObjectState.Modified);
                repoSalesKitType.SaveChanges();
                return true;
            }
            
        }
        public bool UpdateStatus(SalesKitType salesKitType)
        {
            if (salesKitType.SalesKitId == 0)
            {
                repoSalesKitType.ChangeEntityState<SalesKitType>(salesKitType, ObjectState.Added);
            }
            else
            {
                repoSalesKitType.ChangeEntityState<SalesKitType>(salesKitType, ObjectState.Modified);
            }
            repoSalesKitType.SaveChanges();
            return true;

        }
        public void Delete(int salesKitId)
        {
            SalesKitType lead = repoSalesKitType.FindById(salesKitId);

            repoSalesKitType.ChangeEntityState(lead, ObjectState.Deleted);
            repoSalesKitType.SaveChanges();
        }

        public int GetLastDisplayOrder(int? parentId)
        {
            return repoSalesKitType.Query().Filter(x => x.ParentId == parentId).Get().Max(x => x.DisplayOrder) ?? 0;
        }
    }
}

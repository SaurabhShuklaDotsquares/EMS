using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EMS.Data;
using EMS.Repo;

namespace EMS.Service.LibraryManagement
{
    public class CvsTypeService : ICvsTypeService
    {
        private IRepository<CvsType> repoCvsType;
        public CvsTypeService(IRepository<CvsType> _repoCvsType)
        {
            repoCvsType = _repoCvsType;
        }

        public void Dispose()
        {
            if (repoCvsType != null)
            {
                repoCvsType.Dispose();
                repoCvsType = null;
            }
        }
        public List<CvsType> GetCVsTypeByPaging(out int total, PagingService<CvsType> pagingServices)
        {
            return repoCvsType.Query().Filter(pagingServices.Filter)
                .OrderBy(pagingServices.Sort)
                .GetPage(pagingServices.Start, pagingServices.Length, out total)
                .ToList();
        }

        public List<CvsType> GetCvsType()
        {
            return repoCvsType.Query().Filter(x => x.IsActive == true).Get().OrderBy(x => x.DisplayOrder).ToList();
        }

        public CvsType GetCvsTypeDetail(int Id)
        {
            return repoCvsType.Query().Filter(ul => ul.CvsId == Id && ul.IsActive == true).Get().FirstOrDefault();
        }
        public CvsType GetCvsTypeRecordDetail(int Id)
        {
            return repoCvsType.Query().Filter(ul => ul.CvsId == Id).Get().FirstOrDefault();
        }
        public bool Save(CvsType cvsType)
        {
            if (cvsType.CvsId == 0)
            {
                var AlreadyExist = repoCvsType.Query().Filter(p => p.Name.Trim().ToLower().Equals(cvsType.Name.Trim().ToLower())).Get().Count();
                if (AlreadyExist == 0)
                {
                    repoCvsType.ChangeEntityState<CvsType>(cvsType, ObjectState.Added);
                    repoCvsType.SaveChanges();
                    return true;
                }
                return false;
            }
            else
            {
                repoCvsType.ChangeEntityState<CvsType>(cvsType, ObjectState.Modified);
                repoCvsType.SaveChanges();
                return true;
            }

        }

        public bool UpdateStatus(CvsType cvsType)
        {
            if (cvsType.CvsId == 0)
            {
                repoCvsType.ChangeEntityState<CvsType>(cvsType, ObjectState.Added);
            }
            else
            {
                repoCvsType.ChangeEntityState<CvsType>(cvsType, ObjectState.Modified);
            }
            repoCvsType.SaveChanges();
            return true;

        }

        public int GetLastDisplayOrder()
        {
            return repoCvsType.Query().Get().Max(x => x.DisplayOrder) ?? 0;
        }

    }
}

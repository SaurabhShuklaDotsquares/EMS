using EMS.Data;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service
{
    public class VirtualDeveloperService : IVirtualDeveloperService
    {
        #region "Fields"
        private IRepository<VirtualDeveloper> repoVirtualDeveloper;
        private IRepository<VirtualDeveloperCategory> repoVirtualDeveloperCategory;
        #endregion

        #region "Cosntructor"
        public VirtualDeveloperService(IRepository<VirtualDeveloper> _repoVirtualDeveloper, 
                                       IRepository<VirtualDeveloperCategory> _repoVirtualDeveloperCategory)
        {
            this.repoVirtualDeveloper = _repoVirtualDeveloper;
            this.repoVirtualDeveloperCategory = _repoVirtualDeveloperCategory;
        }
        #endregion

        public List<VirtualDeveloper> GetDefaultVDeveloper()
        {
            return repoVirtualDeveloper.Query().Filter(P => P.Ismain == false && P.isactive == true).Get().OrderByDescending(P => P.PMUid).ToList();
        }
        public VirtualDeveloper GetVirtualDeveloperById(int id)
        {
            return repoVirtualDeveloper.Query().Get().FirstOrDefault(P => P.VirtualDeveloper_ID == id);
        }

        public List<VirtualDeveloper> GetVirtualDeveloperByPM(int pmID)
        {
            return repoVirtualDeveloper.Query().Filter(P => P.PMUid == pmID).Get().OrderBy(p => p.Ismain).ThenBy(P => P.VirtualDeveloper_Name).ToList();
        }
        public List<VirtualDeveloper> GetVirtualDeveloperByPaging(out int total, PagingService<VirtualDeveloper> pagingServices)
        {
            return repoVirtualDeveloper.Query().Filter(pagingServices.Filter)
                .OrderBy(pagingServices.Sort)
                .GetPage(pagingServices.Start, pagingServices.Length, out total)
                .ToList();
        }

        public bool Save(VirtualDeveloper virtualDeveloper)
        {
            bool IsAlreadyExist = false;            
            // IsAlreadyExist = repoVirtualDeveloper.Query().Get().Any(p => p.VirtualDeveloper_ID != virtualDeveloper.VirtualDeveloper_ID && p.emailid.Equals(virtualDeveloper.emailid) && p.PMUid == virtualDeveloper.PMUid);
            IsAlreadyExist = repoVirtualDeveloper.Query().Get().Any(p => p.VirtualDeveloper_ID != virtualDeveloper.VirtualDeveloper_ID && p.emailid==virtualDeveloper.emailid && p.PMUid == virtualDeveloper.PMUid);


            if (!IsAlreadyExist)
            {
                if (virtualDeveloper.VirtualDeveloper_ID == 0)
                {
                    virtualDeveloper.CreationDate = DateTime.Now;
                    repoVirtualDeveloper.ChangeEntityState<VirtualDeveloper>(virtualDeveloper, ObjectState.Added);
                }
                else
                {
                    repoVirtualDeveloper.ChangeEntityState<VirtualDeveloper>(virtualDeveloper, ObjectState.Modified);
                }
                repoVirtualDeveloper.SaveChanges();
                return true;
            }
            return false;
        }



        public List<VirtualDeveloper> GetAllVirtualDevelopers()
        {
            return repoVirtualDeveloper.Query().Get().OrderBy(p => p.Ismain).ThenBy(P => P.VirtualDeveloper_Name).ToList();
        }


        public VirtualDeveloper GetVirtualDeveloperByName(string vdName, bool isMain)
        {
            return repoVirtualDeveloper.Query().Filter(d => d.VirtualDeveloper_Name == vdName && d.Ismain == isMain && d.isactive == true).Get().FirstOrDefault();
        }

        public List<VirtualDeveloper> GetVirtualDeveloperByPMUid(int PMUid)
        {
            return repoVirtualDeveloper.Query().Filter(T => (T.Ismain == true || T.Ismain == null) && T.PMUid == PMUid).Get().OrderBy(p => p.VirtualDeveloper_Name).ToList();
        }

        public List<VirtualDeveloperCategory> GetVirtualDeveloperCategoryByPMUid(int PMUid)
        {
            return repoVirtualDeveloperCategory.Query().Filter(T => T.Pmuid == PMUid).Get().OrderBy(p => p.VirtualDeveloperName).ToList();
        }

        public List<VirtualDeveloper> GetVirtualDeveloperByCategory(int catId)
        {
            return repoVirtualDeveloper.Query().Filter(T => T.VirtualDeveloperCategoryId == catId).Get().OrderBy(p => p.VirtualDeveloper_Name).ToList();
        }

        #region "Dispose"
        public void Dispose()
        {
            if (repoVirtualDeveloper != null)
            {
                repoVirtualDeveloper.Dispose();
                repoVirtualDeveloper = null;
            }
            if (repoVirtualDeveloperCategory != null)
            {
                repoVirtualDeveloperCategory.Dispose();
                repoVirtualDeveloperCategory = null;
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using EMS.Repo;
using EMS.Dto;
using EMS.Core;

namespace EMS.Service
{
    public class DomainTypeService : IDomainTypeService
    {
        #region "Fields"
        private IRepository<DomainType> repoDomainType;
        private IRepository<LibraryIndustry> repoLibraryIndustry;
        #endregion

        #region "Constructor"
        public DomainTypeService(IRepository<DomainType> _repoDomainType, IRepository<LibraryIndustry> _repoLibraryIndustry)
        {
            this.repoDomainType = _repoDomainType;
            this.repoLibraryIndustry = _repoLibraryIndustry;
        }
        #endregion
        public List<DomainType> GetDomainList()
        {
            return repoDomainType.Query().Get().ToList();
        }
        public DomainType GetDomainById(int id)
        {
            return repoDomainType.Query().Get().FirstOrDefault(x => x.DomainId == id);
        }
        public List<DomainType> GetDomainsByIds(int[] ids)
        {
            return repoDomainType.Query().Filter(x => ids.Contains(x.DomainId)).GetQuerable().ToList();
        }

        public DomainType Save(DomainTypeDto model)
        {
            DomainType domain = null;
            if (model.DomainId > 0)
            {
                domain = GetDomainById(model.DomainId);

                if (domain == null)
                {
                    return null;
                }

            }
            else
            {
                domain = new DomainType()
                {
                    AddDate = DateTime.Now,
                    IsActive = true,
                    Alias=model.DomainName.ToSelfURL()
                };

            }

            domain.DomainName = model.DomainName;
            domain.IsActive = model.IsActive;
            

            if (domain.DomainId > 0)
            {
                repoDomainType.Update(domain);
            }
            else
            {
                repoDomainType.Insert(domain);
            }

            return domain;
        }

        public void UpdateStatus(int id)
        {
            var domain = GetDomainById(id);
            if (domain != null && domain.DomainId > 0)
            {
                domain.IsActive = domain.IsActive.HasValue ? !domain.IsActive.Value : false;
                repoDomainType.Update(domain);
            }
        }

        public List<DomainType> GetDomainsByPaging(out int total, PagingService<DomainType> pagingService)
        {
            return repoDomainType.Query()
                .Filter(pagingService.Filter).
                OrderBy(pagingService.Sort).
                GetPage(pagingService.Start, pagingService.Length, out total).
                ToList();
        }

        public bool IsTechnologyExists(int domainId, string title)
        {
            return repoDomainType.Query()
                .Filter(x => x.DomainId != domainId && x.DomainName == title)
                .GetQuerable().Any();
        }

        public DomainType GetDomainByName(string domainName)
        {
            return repoDomainType.Query().Filter(T => T.DomainName.Contains(domainName.ToLower())).Get().FirstOrDefault();
        }

        public List<KeyValuePair<int, string>> GetLibraryDomainList()
        {
            return repoLibraryIndustry.Query()
                    .Filter(lt => (lt.Library.LibraryTypeId == 1 || lt.Library.LibraryTypeId == 3) && lt.Library.IsActive==true)
                    .GetQuerable()
                    .GroupBy(lt => lt.IndustryId)
                    .Select(lt => new KeyValuePair<int, string>(lt.Key, $"{lt.First().Industry.DomainName} ({lt.Count()})"))
                    .ToList();
        }

        #region "Dispose"
        public void Dispose()
        {
            if (repoDomainType != null)
            {
                repoDomainType.Dispose();
                repoDomainType = null;
            }
        }


        #endregion
    }
}

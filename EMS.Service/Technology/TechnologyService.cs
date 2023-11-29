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
    public class TechnologyService : ITechnologyService
    {
        #region "Fields"
        private IRepository<Technology> repoTechnology;
        private IRepository<LibraryTechnology> repoLibraryTechnology;
        #endregion

        #region "Cosntructor"
        public TechnologyService(IRepository<Technology> _repoTechnology, IRepository<LibraryTechnology> _repoLibraryTechnology)
        {
            this.repoTechnology = _repoTechnology;
            this.repoLibraryTechnology = _repoLibraryTechnology;
        }
        #endregion
             

        public List<Technology> GetTechnologyList()
        {
            return repoTechnology.Query().Filter(x => x.IsActive == true).Get().ToList();
        }
        public Technology GetTechnologyById(int id)
        {
            return repoTechnology.Query().Get().FirstOrDefault(x => x.TechId == id);
        }
        public List<Technology> GetTechnologiesByIds(int[] ids)
        {
            return repoTechnology.Query().Filter(x => ids.Contains(x.TechId)).GetQuerable().ToList();
        }

        public Technology Save(TechnologyDto model)
        {
            Technology technology = null;
            if (model.TechId > 0)
            {
                technology = GetTechnologyById(model.TechId);

                if (technology == null)
                {
                    return null;
                }

            }
            else
            {
                technology = new Technology()
                {
                    AddDate = DateTime.Now,
                    IsActive = true,
                    Alias = model.Title.ToSelfURL()
            };

            }
            technology.ModifyDate = DateTime.Now;
            technology.Title = model.Title;
            technology.IsActive = model.IsActive;
            


            if (technology.TechId > 0)
            {
                repoTechnology.Update(technology);
            }
            else
            {
                repoTechnology.Insert(technology);
            }

            return technology;
        }

        public void UpdateStatus(int id)
        {
            var technology = GetTechnologyById(id);
            if (technology != null && technology.TechId > 0)
            {
                technology.IsActive = technology.IsActive.HasValue ? !technology.IsActive.Value : false;
                technology.ModifyDate = DateTime.Now;
                repoTechnology.Update(technology);
            }
        }

        public List<Technology> GetTechnologyByPaging(out int total, PagingService<Technology> pagingService)
        {
            return repoTechnology.Query()
                .Filter(pagingService.Filter).
                OrderBy(pagingService.Sort).
                GetPage(pagingService.Start, pagingService.Length, out total).
                ToList();
        }

        public bool IsTechnologyExists(int techId, string title)
        {
            return repoTechnology.Query()
                .Filter(x => x.TechId != techId && x.Title == title)
                .GetQuerable().Any();
        }

        public Technology GetTechnologyByName(string techname)
        {
            return repoTechnology.Query().Filter(T => T.Title.Contains(techname.ToLower())).Get().FirstOrDefault();
        }
        public List<KeyValuePair<int, string>> GetLibraryTechnologyList()
        {
            return repoLibraryTechnology.Query()
                    .Filter(lt => (lt.Library.LibraryTypeId == 1 || lt.Library.LibraryTypeId == 3) && lt.Library.IsActive==true)
                    .GetQuerable()
                    .GroupBy(lt => lt.TechnologyId)
                    .Select(lt => new KeyValuePair<int, string>(lt.Key, $"{lt.First().Technology.Title} ({lt.Count()})"))
                    .ToList();
        }

        #region "Dispose"
        public void Dispose()
        {
            if (repoTechnology != null)
            {
                repoTechnology.Dispose();
                repoTechnology = null;
            }
        }


        #endregion
    }
}

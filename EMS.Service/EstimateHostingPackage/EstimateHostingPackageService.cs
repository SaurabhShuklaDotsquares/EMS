using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMS.Service
{
    public class EstimateHostingPackageService : IEstimateHostingPackageService
    {
        private readonly IRepository<EstimateHostingPackage> repoEstimateHostingPackage;
        private readonly IRepository<EstimateHostingPackageTechnology> repoEstimateHostingPackageTechnology;
        private readonly IRepository<EstimateCountry> repoEstimateCountry;

        public EstimateHostingPackageService(IRepository<EstimateHostingPackage> _repoEstimateHostingPackage,
            IRepository<EstimateHostingPackageTechnology> _repoEstimateHostingPackageTechnology,
            IRepository<EstimateCountry> _repoEstimateCountry)
        {
            this.repoEstimateHostingPackage = _repoEstimateHostingPackage;
            this.repoEstimateHostingPackageTechnology = _repoEstimateHostingPackageTechnology;
            this.repoEstimateCountry = _repoEstimateCountry;
        }

        #region [Paging]
        public List<EstimateHostingPackage> GetByPaging(out int total, PagingService<EstimateHostingPackage> pagingSerices)
        {
            return repoEstimateHostingPackage.Query().
                Filter(pagingSerices.Filter).
                OrderBy(pagingSerices.Sort).
                GetPage(pagingSerices.Start, pagingSerices.Length, out total).
                ToList();
        }
        #endregion

        #region [Get By Id]
        public EstimateHostingPackageDto GetById(int id)
        {
            var entity = repoEstimateHostingPackage.FindById(id);
            if (entity != null)
            {
                var model = new EstimateHostingPackageDto();
                model.Id = entity.Id;
                model.Name = entity.Name;
                model.PackageDetail = entity.PackageDetail;
                model.CountryId = entity.CountryId;
                model.HostingCost = entity.HostingCost;
                model.HostingCostType = entity.HostingCostType;
                model.SetupCost = entity.SetupCost;
                model.SetupCostType = entity.SetupCostType;
                model.IsActive = entity.IsActive;
                model.CurrencyId = entity.CurrencyId;
                model.EstimateTechnologyIds = entity.EstimateHostingPackageTechnology.Select(x => x.EstimateTechnologyId).ToList();
                model.CurrSign = entity.Currency?.CurrSign;

                return model;
            }

            return null;
        }
        #endregion

        #region [Save]
        public bool Save(EstimateHostingPackageDto model)
        {
            bool isSave = false;

            var entity = repoEstimateHostingPackage.Query().Filter(x => x.Id == model.Id).Get().FirstOrDefault();

            if (entity == null)
            {
                entity = new EstimateHostingPackage
                {
                    Name = model.Name,
                    PackageDetail = model.PackageDetail,
                    CountryId = model.CountryId,
                    HostingCost = model.HostingCost,
                    HostingCostType = model.HostingCostType,
                    SetupCost = model.SetupCost,
                    SetupCostType = model.SetupCostType,
                    CurrencyId = model.CurrencyId,
                    IsActive = model.IsActive,
                    ModifiedDate = DateTime.Now,
                    CreatedDate = DateTime.Now
                };
                var EstimateHostingPackageTechnologyList = new List<EstimateHostingPackageTechnology>();
                foreach (var item in model.EstimateTechnologyIds)
                {
                    EstimateHostingPackageTechnologyList.Add(new EstimateHostingPackageTechnology
                    {
                        EstimateTechnologyId = item
                    });
                }

                entity.EstimateHostingPackageTechnology = EstimateHostingPackageTechnologyList;
                repoEstimateHostingPackage.Insert(entity);
                isSave = true;
            }
            else
            {
                if (entity.EstimateHostingPackageTechnology.Count() > 0)
                {
                    repoEstimateHostingPackageTechnology.DeleteBulk(entity.EstimateHostingPackageTechnology.ToList());
                }

                entity.Name = model.Name;
                entity.PackageDetail = model.PackageDetail;
                entity.CountryId = model.CountryId;
                entity.HostingCost = model.HostingCost;
                entity.HostingCostType = model.HostingCostType;
                entity.SetupCost = model.SetupCost;
                entity.SetupCostType = model.SetupCostType;
                entity.CurrencyId = model.CurrencyId;
                entity.IsActive = model.IsActive;
                entity.ModifiedDate = DateTime.Now;

                var EstimateHostingPackageTechnologyList = new List<EstimateHostingPackageTechnology>();
                foreach (var item in model.EstimateTechnologyIds)
                {
                    EstimateHostingPackageTechnologyList.Add(new EstimateHostingPackageTechnology
                    {
                        EstimateTechnologyId = item
                    });
                }
                entity.EstimateHostingPackageTechnology = EstimateHostingPackageTechnologyList;

                repoEstimateHostingPackage.Update(entity);
                isSave = true;
            }

            return isSave;
        }
        #endregion

        #region [Get Data By Technologies]
        public List<EstimateHostingPackage> GetByTechnologies(List<int> technologies, int? countryId = null)
        {
            var entity = repoEstimateHostingPackage.Query().Filter(x => x.IsActive && x.EstimateHostingPackageTechnology.Any(a => technologies.Count(t => t == a.EstimateTechnologyId) > 0) && (countryId == null || x.CountryId == countryId)).Get().ToList();

            return entity;
        }

        public List<EstimateCountry> GetByCountryByTechnologies(List<int> technologies)
        {
            var entity = repoEstimateCountry.Query().Filter(x => x.IsActive && x.EstimateHostingPackage.Any(es => es.IsActive && es.EstimateHostingPackageTechnology.Any(a => technologies.Count(t => t == a.EstimateTechnologyId) > 0))).Get().ToList();

            return entity;
        }
        #endregion

        #region [Dispose]
        public void Dispose()
        {
            if (repoEstimateHostingPackage != null)
            {
                repoEstimateHostingPackage.Dispose();
            }
        }
        #endregion
    }
}

using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EMS.Service
{
    public class ProductLandingService : IProductLandingService
    {
        #region "Fields and Cosntructor"

        private readonly IRepository<ProductLanding> repoProductLanding;
        private readonly IRepository<ProductLandingScreenshot> repoProductLandingScreenshot;

        public ProductLandingService(IRepository<ProductLanding> _repoProductLanding, IRepository<ProductLandingScreenshot> _repoProductLandingScreenshot)
        {
            repoProductLanding = _repoProductLanding;
            repoProductLandingScreenshot = _repoProductLandingScreenshot;
        }

        #endregion

        public ProductLanding GetProductLandingById(int id)
        {
            return repoProductLanding.FindById(id);
        }

        public ProductLanding Save(ProductLandingDto model)
        {
            ProductLanding plEntity = model.Id > 0 ? GetProductLandingById(model.Id) : new ProductLanding();

            if (model.Id > 0 && (plEntity == null || (plEntity.CreateByUid != model.CurrentUserId && plEntity.UserLogin.PMUid != model.PMUserId) || 
                plEntity.Status != (byte)Enums.ProductLandingPageStatus.Draft))
            {
                return null;
            }

            plEntity.ProductName = model.ProductName;
            plEntity.HighlightText = model.HighlightText;
            plEntity.AboutProduct = model.AboutProduct;
            plEntity.Features = model.Features;
            plEntity.Feature1 = model.Feature1;
            plEntity.Feature2 = model.Feature2;
            plEntity.Feature3 = model.Feature3;
            plEntity.Testimonials = model.Testimonials;
            plEntity.ServiceDetail = model.ServiceDetail;
            plEntity.TechnologyDetail = model.TechnologyDetail;
            plEntity.Status = model.IsDraft ? (byte)Enums.ProductLandingPageStatus.Draft : (byte)Enums.ProductLandingPageStatus.Published;
            plEntity.ModifyByUid = model.CurrentUserId;
            plEntity.ModifyDate = DateTime.Now;

            if (model.Screenshots.Count > 0)
            {
                model.Screenshots.FindAll(x => x.Id == 0 && x.ScreenshotUrl.HasValue())
                     .ForEach(x => plEntity.ProductLandingScreenshots.Add(new ProductLandingScreenshot
                     {
                         ScreenshotUrl = x.ScreenshotUrl,
                         Description = x.Description
                     }));
            }

            if (plEntity.Id == 0)
            {
                plEntity.CreateByUid = model.CurrentUserId;
                plEntity.CreateDate = DateTime.Now;

                repoProductLanding.InsertGraph(plEntity);
            }
            else
            {
                plEntity.CreateDate = plEntity.Status == (byte)Enums.ProductLandingPageStatus.Published ? DateTime.Now : plEntity.CreateDate;
                model.Screenshots = model.Screenshots.FindAll(x => x.Id > 0);
                if (model.Screenshots.Count != 0 || plEntity.ProductLandingScreenshots.Count != 0)
                {
                    var deletedScreenShotIds = new List<long>();
                    foreach (var item in plEntity.ProductLandingScreenshots)
                    {
                        var screenShot = model.Screenshots.FirstOrDefault(x => x.Id == item.Id);

                        if (screenShot != null)
                        {
                            item.Description = screenShot.Description;
                        }
                        else
                        {
                            deletedScreenShotIds.Add(item.Id);
                        }
                    }

                    if (deletedScreenShotIds.Count > 0)
                    {
                        repoProductLanding.ChangeEntityCollectionState(plEntity.ProductLandingScreenshots.Where(x => deletedScreenShotIds.Contains(x.Id)).ToList(), ObjectState.Deleted);
                    }
                }

                repoProductLanding.SaveChanges();
            }

            return plEntity;
        }

        public void AddScreenshot(int id, string screenshotUrl, string description)
        {
            var screenshotEntity = new ProductLandingScreenshot
            {
                ScreenshotUrl = screenshotUrl,
                Description = description,
                ProductLandingId = id
            };
            repoProductLandingScreenshot.Insert(screenshotEntity);
        }

        public List<ProductLanding> GetProductLandingByPaging(out int total, PagingService<ProductLanding> pagingService)
        {
            return repoProductLanding.Query()
                    .Filter(pagingService.Filter)
                    .OrderBy(pagingService.Sort)
                    .GetPage(pagingService.Start, pagingService.Length, out total)
                    .ToList();
        }

        #region "Dispose"

        public void Dispose()
        {
            if (repoProductLanding != null)
            {
                repoProductLanding.Dispose();
            }
        }

        #endregion
    }
}
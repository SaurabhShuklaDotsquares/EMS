using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Dto;
using EMS.Data;
using EMS.Repo;
using System.Web;
using System.Net;

namespace EMS.Service
{
    public class BucketModelService : IBucketModelService
    {
        #region "Fields"
        private IRepository<BucketModel> repoBucketModel;
        #endregion
        #region Construtor
        public BucketModelService(IRepository<BucketModel> repoBucketModel)
        {
            this.repoBucketModel = repoBucketModel;
        }
        #endregion
        public BucketModel GetBucketModelById(int id)
        {
            return repoBucketModel.Query()
                        .Filter(x => x.BucketId == id)
                        .GetQuerable()
                        .FirstOrDefault();
        }

        public bool IsBucketModelNameAndCodeExists(int bucketId,string name,string code)
        {
            return repoBucketModel.Query()
                .Filter(x => x.BucketId !=bucketId && (x.ModelName == name || x.ModelCode==code))
                .GetQuerable().Any();
        }
        public List<BucketModel> GetBucketModelByPaging(out int total, PagingService<BucketModel> pagingService)
        {
            return repoBucketModel.Query()
                .Filter(pagingService.Filter).
                OrderBy(pagingService.Sort).
                GetPage(pagingService.Start, pagingService.Length, out total).
                ToList();
        }

        public void UpdateStatus(int id)
        {
            var bucketModel = GetBucketModelById(id);
            if (bucketModel != null && bucketModel.BucketId > 0)
            {
                bucketModel.IsActive = bucketModel.IsActive.HasValue ? !bucketModel.IsActive.Value : false;
                bucketModel.ModifyDate = DateTime.Now;
                repoBucketModel.Update(bucketModel);
            }
        }

        public BucketModel Save(BucketModelDto model)
        {
            BucketModel bucketModel = null;
            if (model.BucketId > 0)
            {
                bucketModel = GetBucketModelById(model.BucketId);
                if (bucketModel == null)
                {
                    return null;
                }
            }
            else
            {
                bucketModel = new BucketModel()
                {
                    AddDate = DateTime.Now,
                    IsActive = true
                };

            }
            bucketModel.ModelName = model.ModelName;
            bucketModel.ModifyDate = DateTime.Now;
            bucketModel.ModelCode = model.ModelCode.ToUpper();
            bucketModel.IsActive = model.IsActive;
            bucketModel.IP = model.IP;
            if (bucketModel.BucketId > 0)
            {
                repoBucketModel.Update(bucketModel);
            }
            else
            {
                repoBucketModel.Insert(bucketModel);
            }

            return bucketModel;
        }
        public void Dispose()
        {
            if (repoBucketModel != null)
            {
                repoBucketModel.Dispose();
                repoBucketModel = null;
            }
        }
        public List<BucketModel> GetData()
        {
            return repoBucketModel.Query().Get().ToList();
        }
    }
}

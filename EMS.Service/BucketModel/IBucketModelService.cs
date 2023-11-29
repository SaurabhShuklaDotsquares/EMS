using System;
using System.Collections.Generic;
using EMS.Data;
using EMS.Dto;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service
{
    public interface IBucketModelService: IDisposable
    {
        BucketModel GetBucketModelById(int id);
        bool IsBucketModelNameAndCodeExists(int bucketId, string name, string code);
        List<BucketModel> GetBucketModelByPaging(out int total, PagingService<BucketModel> pagingService);
        void UpdateStatus(int id);
        BucketModel Save(BucketModelDto model);
        List<BucketModel> GetData();
    }
}

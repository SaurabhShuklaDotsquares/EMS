using EMS.Data;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMS.Service
{
    public class EstimateHourService : IEstimateHourService
    {

        IRepository<EstimateHour> repoEstimateHour;
        IRepository<EstimateHourFileNameType> repoEstimateHourFileNameType;
        public EstimateHourService(IRepository<EstimateHour> _repoEstimateHour,IRepository<EstimateHourFileNameType> _repoEstimateHourFileNameType)
        {
            repoEstimateHour = _repoEstimateHour;
            repoEstimateHourFileNameType = _repoEstimateHourFileNameType;
        }

        public EstimateHour GetEstimateHourById(int id)
        {
            return repoEstimateHour.FindById(id);
        }

        public List<EstimateHour> GetEstimateHourByPaging(out int total, PagingService<EstimateHour> pagingServices)
        {
            return repoEstimateHour.Query()
                    .AsTracking()
                    .Filter(pagingServices.Filter)
                    .OrderBy(pagingServices.Sort)
                    .GetPage(pagingServices.Start, pagingServices.Length, out total)
                    .ToList();
        }

        public List<EstimateHour> GetEstimateHourList()
        {
           return repoEstimateHour.Query()
                .OrderBy(o => o.OrderByDescending(x => x.Modified))
                .Get()
                .ToList();
        }

        public EstimateHour Save(EstimateHour entity)
        {
            if (entity.Id == 0)
            {
                entity.Created = DateTime.Now;
                entity.Modified = DateTime.Now;
                repoEstimateHour.InsertGraph(entity);
            }
            else
            {
                entity.Modified = DateTime.Now;
                var estimateHourEntity = repoEstimateHour.FindById(entity.Id);
                var estimateHourUpdated = repoEstimateHour.Update(estimateHourEntity, entity);
                
                
                //if (entity.User_Tech != null && entity.User_Tech.Any())
                //{
                //    entity.User_Tech.ToList().ForEach(a =>
                //    {
                //        userloginUpdated.User_Tech.Add(a);
                //    });
                //}

                repoEstimateHour.SaveChanges();
            }
            return entity;
        }

        public List<EstimateHourFileNameType> GetEstimateHourFileNameTypes()
        {
            return repoEstimateHourFileNameType.Query().Get().ToList();
        }

        public EstimateHourFileNameType SaveEstimateHourFileNameType(EstimateHourFileNameType entity)
        {
            entity = repoEstimateHourFileNameType.InsertCallback(entity);
            return entity;
        }
        public EstimateHourFileNameType GetEstimateHourFileNameTypeById(int id)
        {
            return repoEstimateHourFileNameType.FindById(id);
        }

    }
}

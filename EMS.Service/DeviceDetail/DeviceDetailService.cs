using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EMS.Service
{
    public class DeviceDetailService : IDeviceDetailService
    {
        #region Constructor and Member

        private IRepository<DeviceDetail> repoDeviceDetail;

        public DeviceDetailService(IRepository<DeviceDetail> _repoDeviceDetail)
        {
            repoDeviceDetail = _repoDeviceDetail;
        }

        #endregion

        public List<DeviceDetail> GetDeviceDetailByPaging(out int total, PagingService<DeviceDetail> pagingService)
        {
            return repoDeviceDetail.Query()
                    .AsTracking()
                    .Filter(pagingService.Filter)
                    .OrderBy(pagingService.Sort)
                    .GetPage(pagingService.Start, pagingService.Length, out total)
                    .ToList();
        }

        public DeviceDetail GetDeviceDetailById(int id)
        {
            return repoDeviceDetail.FindById(id);
        }

        public DeviceDetail SaveAssignDevice(AssignDeviceDto model)
        {
            DeviceDetail entity = new DeviceDetail();
            entity.DeviceId = model.DeviceId;
            entity.Condition = model.Condition;
            entity.AssignedToUid = model.AssignedToUid;
            entity.CreateByUid = model.CreateByUid;
            entity.ModifyByUid = model.ModifyByUid;
            entity.AssignedDateTime = model.AssignedDateTime.ToDateTime("dd/MM/yyyy").Value;
            entity.CreateDate = DateTime.Now;
            entity.ModifyDate = DateTime.Now;
            entity.SerialNumber = model.SerialNumber;
            repoDeviceDetail.Insert(entity);
            return entity;
        }

        public DeviceDetail UpdateReturnDevice(ReturnDeviceDto model)
        {
            DeviceDetail entity = GetDeviceDetailById(model.DeviceDetailId);
            if (entity != null)
            {
                entity.SubmitDateTime = DateTime.ParseExact(model.ReturnDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                entity.SubmitToUid = model.ReturnToUid;
                repoDeviceDetail.SaveChanges();
            }
            return entity;
        }
        public DeviceDetail UpdateConditon(AssignDeviceDto model)
        {
            DeviceDetail entity = GetDeviceDetailById(model.Id);
            if(entity!=null)
            {
                entity.Condition = model.Condition;
                repoDeviceDetail.SaveChanges();
            }
            return entity;
        }

        #region "Dispose"

        public void Dispose()
        {
            if (repoDeviceDetail != null)
            {
                repoDeviceDetail.Dispose();
            }
        }

        #endregion
    }
}

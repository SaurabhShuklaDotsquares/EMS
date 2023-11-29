using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using EMS.Core;

namespace EMS.Service
{
    public class DeviceService : IDeviceService
    {
        #region Constructor and Member

        private IRepository<Device> repoDevice;

        public DeviceService(IRepository<Device> _repoDevice)
        {
            repoDevice = _repoDevice;
        }

        #endregion

        public List<Device> GetDeviceByPaging(out int total, PagingService<Device> pagingService)
        {
            return repoDevice.Query()
                    .Filter(pagingService.Filter)
                    .OrderBy(pagingService.Sort)
                    .GetPage(pagingService.Start, pagingService.Length, out total)
                    .ToList();
        }

        public Device GetDeviceById(int id)
        {
            return repoDevice.FindById(id);
           
        }

        public bool Delete(int id)
        {
            if (id > 0)
            {
                Device device = GetDeviceById(id);
                if (device != null)
                {
                    repoDevice.Delete(id);
                    return true;
                }
            }
            return false;
        }

        public Device Save(DeviceDataDto model)
        {
            Device entity = model.Id > 0 ? GetDeviceById(model.Id) : new Device();
            if (entity != null)
            {

                if (entity.Id > 0 && model.Quantity < entity.DeviceDetails.Count(x => !x.SubmitDateTime.HasValue))
                {
                    return null;
                }

                entity.DeviceType = model.DeviceType;
                entity.Name = model.Name;
                entity.Quantity = model.Quantity ?? 0;
                entity.Condition = model.Condition;
                entity.SimNumber = entity.DeviceType == (byte)Enums.DeviceType.Sim ? model.SimNumber : null;
                entity.SimNetwork = entity.DeviceType == (byte)Enums.DeviceType.Sim ? model.SimNetwork : null;
                entity.IsActive = true;
                entity.ModifyByUid = model.CurrentUserId;
                entity.ModifyDate = DateTime.Now;

                if (entity.Id == 0)
                {
                    entity.CreateByUid = model.CurrentUserId;
                    entity.CreateDate = DateTime.Now;
                    entity.PMUid = model.PMUid;

                    repoDevice.Insert(entity);
                }
                else
                {
                    repoDevice.SaveChanges();
                }
            }
            return entity;
        }

        public List<Device> GetDeviceList(int deviceType, int pmUid)
        {
            return repoDevice.Query()
                      .Filter(x => x.DeviceType == deviceType && x.PMUid == pmUid)
                      .Get().ToList();
        }

        #region "Dispose"

        public void Dispose()
        {
            if (repoDevice != null)
            {
                repoDevice.Dispose();
            }
        }

        #endregion
    }
}

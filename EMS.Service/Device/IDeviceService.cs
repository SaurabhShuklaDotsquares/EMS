
using EMS.Data;
using EMS.Dto;
using System;
using System.Collections.Generic;

namespace EMS.Service
{
    public interface IDeviceService: IDisposable
    {

        //DeviceDetail UpdateStatus(DeviceStatusDto model);
        //List<DeviceDetail> GetDiviceDetailByPaging(out int total, PagingService<DeviceDetail> pagingSerices);
        ////DeviceDetail Save(DeviceDto deviceDto);
        //List<Device> GetDevices();
        //List<Accessory> GetAccessories();
        //List<Sim> GetAllSim();
        List<Device> GetDeviceByPaging(out int total, PagingService<Device> pagingService);

        Device GetDeviceById(int id);

        bool Delete(int id);

        Device Save(DeviceDataDto model);

        List<Device> GetDeviceList(int deviceType, int pmUid);
    }
}

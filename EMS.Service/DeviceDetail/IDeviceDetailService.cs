using EMS.Data;
using EMS.Dto;
using System.Collections.Generic;

namespace EMS.Service
{
    public interface IDeviceDetailService
    {
        List<DeviceDetail> GetDeviceDetailByPaging(out int total, PagingService<DeviceDetail> pagingService);

        DeviceDetail GetDeviceDetailById(int id);

        DeviceDetail SaveAssignDevice(AssignDeviceDto model);

        DeviceDetail UpdateReturnDevice(ReturnDeviceDto model);

        DeviceDetail UpdateConditon(AssignDeviceDto model);
    }
}

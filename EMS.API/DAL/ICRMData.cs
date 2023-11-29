using EMS.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.API.DAL
{
    public interface ICRMData
    {
        ResponseModel<string> UpdateCRMData(int pmuid, ProjectInfoReqModel CrmProjectModel);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using EMS.Dto;

namespace EMS.Service
{
   public interface IEscalationTypeService: IDisposable
    {
       List<EscalationType> GetEscalationTypeList();
       EscalationType GetEscalationTypeById(int id);
    }
}

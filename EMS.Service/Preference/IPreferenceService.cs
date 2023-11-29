using EMS.Data;
using EMS.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service
{
    public interface IPreferenceService : IDisposable
    {
        Preference GetDataByPmid(int pmid);
        void Save(Preference entity);
    }
}

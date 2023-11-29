using EMS.Data;
using EMS.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service
{
    public interface IKRAServices : IDisposable
    {
        KraDto SaveList(KraDto data);
        Kra GetDesignationById(int designationId);
        List<Kra> GetKradataByDesgnationId(int designationId);
        KraDto DeleteList(KraDto kraDto);

        
    }
}

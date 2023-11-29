using EMS.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service.LibraryManagement
{
    public interface ICvsTypeService :IDisposable
    {
        List<CvsType> GetCVsTypeByPaging(out int total, PagingService<CvsType> pagingServices);
        List<CvsType> GetCvsType();
        bool UpdateStatus(CvsType cvsType);
        bool Save(CvsType cvsType);
        CvsType GetCvsTypeDetail(int Id);
        CvsType GetCvsTypeRecordDetail(int Id);
        int GetLastDisplayOrder();
    }
}

using EMS.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service.LibraryManagement
{
    public interface ISalesKitTypeService : IDisposable
    {
        List<SalesKitType> GetSalesKitTypeByPaging(out int total, PagingService<SalesKitType> pagingServices);
        List<SalesKitType> GetSalesKitType();
        List<SalesKitType> GetSubSalesKitType(int ParentId);
        SalesKitType GetSubSalesKitDetail(int Id);
        SalesKitType GetSalesKitTypeDetail(int Id);
        bool UpdateStatus(SalesKitType salesKitType);
        bool Save(SalesKitType salesKitType);
        void Delete(int salesKitId);
        int GetLastDisplayOrder(int? parentId);


    }
}

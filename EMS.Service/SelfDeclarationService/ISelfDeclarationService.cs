using EMS.Data;
using EMS.Dto;
using System;
using System.Collections.Generic;
using System.Text;


namespace EMS.Service
{
    public interface ISelfDeclarationService
    {
        SelfDeclaration GetSelfDeclarationById(int id);
        SelfDeclaration GetSelfDeclarationByUid(int uid);
        SelfDeclaration Save(SelfDeclarationDto model);
        List<SelfDeclaration> GetSelfDeclarationByPaging(out int total, PagingService<SelfDeclaration> pagingService);
    }
}

using EMS.Data;
using EMS.Dto;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service
{
    public interface ISubjectMasterExpertService: IDisposable
    {

        void SaveEdit(Sme sme);
       
        Sme GetSmeById(int id);
        void Delete(Sme sme);
        List<UserLogin> GetUsers1(int PMId);
        List<string> GetSubjectMasterExpertData();
        Sme GetSmeDataByExpert(string subjectMatterExpert);
       
        List<Sme> GetAllSmeData(out int total, PagingService<Sme> pagingSerices);
        List<Sme> GetAllSmeDatas(string SubjectMatter);
        IEnumerable<UserLogin> GetUsersByIds(int?[] userIds);
        List<Sme> GetSubjectMatterList();
    }
}

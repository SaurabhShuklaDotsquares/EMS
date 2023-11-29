using EMS.Data;
using EMS.Dto;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EMS.Service
{
   public interface IVaccinationService
    {
        List<Vaccine> GetAllVaccineTypes();
        VaccinationStatus Save(VaccinationDto vModel);
        VaccinationStatus GetVaccinationStatusById(int id);
        VaccinationStatus GetVaccinationStatusByUserId(int userId);
        List<VaccinationStatus> GetVaccinationDocByPaging(out int total, PagingService<VaccinationStatus> pagingSerices);
        List<VaccinationStatus> GetVaccinationCertificateList(Expression<Func<VaccinationStatus, bool>> expr);
    }
}

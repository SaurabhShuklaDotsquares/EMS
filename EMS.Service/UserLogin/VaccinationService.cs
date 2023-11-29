using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace EMS.Service
{
    public class VaccinationService : IVaccinationService
    {
        private IRepository<Vaccine> _repoVaccine;
        private IRepository<VaccinationStatus> _repoVaccinationStatus;
        public VaccinationService(IRepository<Vaccine> repoVaccine, IRepository<VaccinationStatus> repoVaccinationStatus)
        {
            _repoVaccine = repoVaccine;
            _repoVaccinationStatus = repoVaccinationStatus;
        }


        public List<Vaccine> GetAllVaccineTypes()
        {
            return _repoVaccine.Query().Get().ToList();
        }

        public VaccinationStatus Save(VaccinationDto vModel)
        {
            bool isUpdate = false;
            VaccinationStatus vaccinationStatus = null;
            vaccinationStatus = GetVaccinationStatusByUserId(vModel.UserLoginId);
            if (vaccinationStatus != null)
            {
                vaccinationStatus.VaccinationTypeId = vModel.VaccinationTypeId;
                vaccinationStatus.VaccinatedTypeId = vModel.SelectedDose;
                vaccinationStatus.VaccinationDose1Date = vModel.Dose1Date.ToDateTime();
                vaccinationStatus.VaccinationDose2Date = vModel.Dose2Date.ToDateTime();
                vaccinationStatus.UpdatedCertificate = vModel.FileName;
                vaccinationStatus.DeclarationName = vModel.DeclartionText;
                vaccinationStatus.AddedDate = DateTime.Now;
                vaccinationStatus.IsActive = true;
                isUpdate = true;
            }

            else
            {
                vaccinationStatus = new VaccinationStatus()
                {
                    Uid = vModel.UserLoginId,
                    VaccinationTypeId = vModel.VaccinationTypeId,
                    VaccinatedTypeId = vModel.SelectedDose,
                    VaccinationDose1Date = vModel.Dose1Date.ToDateTime(),
                    VaccinationDose2Date = vModel.Dose2Date.ToDateTime(),
                    UpdatedCertificate = vModel.FileName,
                    DeclarationName = vModel.DeclartionText,
                    AddedDate = DateTime.Now,
                    IsActive = true,
                };
            }

            if (isUpdate)
            {
                _repoVaccinationStatus.Update(vaccinationStatus);
            }
            else
            {
                _repoVaccinationStatus.Insert(vaccinationStatus);
            }

            return vaccinationStatus;
        }

        public VaccinationStatus GetVaccinationStatusById(int id)
        {
            return _repoVaccinationStatus.Query().Filter(x => x.Id == id).Get().FirstOrDefault();
        }
        public VaccinationStatus GetVaccinationStatusByUserId(int userId)
        {
            return _repoVaccinationStatus.Query().Filter(x => x.Uid == userId).Get().FirstOrDefault();
        }
        public List<VaccinationStatus> GetVaccinationDocByPaging(out int total, PagingService<VaccinationStatus> pagingSerices)
        {
            return _repoVaccinationStatus.Query().
                Filter(pagingSerices.Filter).
                OrderBy(pagingSerices.Sort).
                GetPage(pagingSerices.Start, pagingSerices.Length, out total).
                ToList();
        }

        public List<VaccinationStatus> GetVaccinationCertificateList(Expression<Func<VaccinationStatus, bool>> expr)
        {
            return _repoVaccinationStatus.Query().Filter(expr).Get().ToList();
        }
    }
}

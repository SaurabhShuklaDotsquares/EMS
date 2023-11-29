using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMS.Service
{
    public class SelfDeclarationService: ISelfDeclarationService
    {
        #region "Fields"
        private IRepository<SelfDeclaration> repoSelfDeclaration;
        private IRepository<UserLogin> repoUserLoginService;
        #endregion

        #region "Cosntructor"
        public SelfDeclarationService(IRepository<SelfDeclaration> _repoSelfDeclaration, IRepository<UserLogin> _repoUserLoginService)
        {
            this.repoSelfDeclaration = _repoSelfDeclaration;
            this.repoUserLoginService = _repoUserLoginService;
        }
        #endregion

        public SelfDeclaration GetSelfDeclarationById(int id)
        {
            return repoSelfDeclaration.FindById(id);
        }
        public SelfDeclaration GetSelfDeclarationByUid(int uid)
        {
            return repoSelfDeclaration.Query().Filter(s=>s.Uid==uid).Get().FirstOrDefault();
        }

        public SelfDeclaration Save(SelfDeclarationDto model)
        {
            SelfDeclaration entity = model.Id > 0 ? GetSelfDeclarationById(model.Id) : new SelfDeclaration();
            if (entity == null)
            {
                return null;
            }
            var user = GetLoginUserByUid(model.Uid);

            entity.Name = user.Name;
            entity.EmpCode = user.EmpCode;
            entity.Dob = model.Dob.ToDateTime("dd/MM/yyyy");
            entity.JoiningDate = user.JoinedDate; ;
            entity.MobileNumber = model.MobileNumber;
            entity.EmailPersonal = model.EmailPersonal;
            entity.Address = model.Address;
            entity.LocalAddress= model.LocalAddress;
            entity.RecentlyInJaipur = model.RecentlyInJaipur;
            entity.Location = model.Location;
            entity.Purpose = model.Purpose;
            entity.HasDiseaseSymptoms = model.HasDiseaseSymptoms;
            entity.SymptomsEndDate = model.SymptomsEndDate.ToDateTime("dd/MM/yyyy");
            entity.SymptomsStartDate = model.SymptomsStartDate.ToDateTime("dd/MM/yyyy");
            entity.Uid = model.Uid;
            entity.DeclarationName = $"I {entity.Name} hereby, acknowledge that I understand and accept all rules, regulations and guidelines to be followed in ofﬁce. I also acknowledge that the above information is true and valid to the best of my knowledge.";
            entity.Ip = model.Ip;

            entity.HasCoughSymptoms = model.HasCoughSymptoms;
            entity.HasFeverSymptoms = model.HasFeverSymptoms;
            entity.HasBreathingSymptoms = model.HasBreathingSymptoms;
            entity.HasSmellAndTasteSymptoms = model.HasSmellAndTasteSymptoms;
            entity.HasDiabetesProblem = model.HasDiabetesProblem;
            entity.HasHypertensionProblem = model.HasHypertensionProblem;
            entity.HasLungProblem = model.HasLungProblem;
            entity.HasHeartProblem = model.HasHeartProblem;
            entity.HasKidneyProblem = model.HasKidneyProblem;
            entity.HasTraveledInternationally = model.HasTraveledInternationally;

            if (entity.Id>0)
            {
                repoSelfDeclaration.SaveChanges();
            }
            else
            {
                entity.AddDate = DateTime.Now;
                repoSelfDeclaration.Insert(entity);
            }
            return entity;
        }


        public List<SelfDeclaration> GetSelfDeclarationByPaging(out int total, PagingService<SelfDeclaration> pagingService)
        {
            return repoSelfDeclaration.Query()
                .Filter(pagingService.Filter).
                OrderBy(pagingService.Sort).
                GetPage(pagingService.Start, pagingService.Length, out total).
                ToList();
        }

        public UserLogin GetLoginUserByUid(int uid)
        {
            return repoUserLoginService.FindById(uid);
        }

        #region "Dispose"
        public void Dispose()
        {
            if (repoSelfDeclaration != null)
            {
                repoSelfDeclaration.Dispose();
                repoSelfDeclaration = null;
            }
        }


        #endregion
    }
}

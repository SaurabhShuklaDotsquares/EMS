using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMS.Service
{
    public class SubjectMasterExpertService : ISubjectMasterExpertService
    {
        private readonly IRepository<Sme> _repoSme;
        private readonly IRepository<UserLogin> _repoUser;

        public SubjectMasterExpertService(IRepository<Sme> repoSme, IRepository<UserLogin> repoUser)
        {
            _repoSme = repoSme;
            _repoUser = repoUser;
           
        }

        public void SaveEdit(Sme smeEntity)
        {
            try
            {
                if (smeEntity.Id == 0)
                {
                    _repoSme.ChangeEntityState<Sme>(smeEntity, ObjectState.Added);
                    _repoSme.InsertGraph(smeEntity);
                }
                else
                {
                    _repoSme.ChangeEntityState<Sme>(smeEntity, ObjectState.Modified);
                }

                _repoSme.SaveChanges();
            }
            catch (Exception ex)
            {
                
                throw new Exception("Error while saving the entity.", ex);
            }
        }

      

        public void Delete(Sme smeEntity)
        {
            if (smeEntity.Id > 0)
            {
                _repoSme.ChangeEntityState<Sme>(smeEntity, ObjectState.Deleted);
                _repoSme.Delete(smeEntity.Id);
            }
        }



        public Sme GetSmeById(int id)
        {
            try
            {
                var sme = _repoSme.FindById(id);
                return sme;
            }
            catch (Exception)
            {
                // Handle exception or logging here
                return null;
            }
        }


        public void Dispose()
        {
            _repoSme.Dispose();
        }

       

      

        public List<UserLogin> GetUsers1(int PMId)
        {
            return _repoUser.Query().Filter(x => x.IsActive == true && x.PMUid == PMId).Get().OrderBy(x => x.Name).ToList();
        }

        public List<string> GetSubjectMasterExpertData()
        {
            return _repoSme.Query()               
                .Get()
                .OrderBy(x => x.SubjectMatterExpert)
                .Select(x => x.SubjectMatterExpert)
                .ToList();
        }

        public Sme GetSmeDataByExpert(string subjectMatterExpert)
        {
            return _repoSme.Query()
                .Filter(x =>x.SubjectMatterExpert == subjectMatterExpert.Trim())
                .Get()
                .FirstOrDefault();
        }

        public List<Sme> GetAllSmeData(out int total, PagingService<Sme> pagingSerices)
        {
            return _repoSme.Query().Filter(pagingSerices.Filter).
                 OrderBy(pagingSerices.Sort).
                 GetPage(pagingSerices.Start, pagingSerices.Length, out total).
                 ToList();
        }


        public List<Sme> GetAllSmeDatas(string SubjectMatter)
        {
            var expr = PredicateBuilder.True<Sme>();
            string[] arr = SubjectMatter.Split(',');

            if (SubjectMatter != null && SubjectMatter.Length > 0 && SubjectMatter != "null")
            {
                expr = expr.And(e => arr.Contains(e.Id.ToString()));
            }

            return _repoSme.Query().
                 Filter(expr).Get().
                 ToList();
        }

        public IEnumerable<UserLogin> GetUsersByIds(int?[] userIds)
        {
            var users = _repoUser.Query() 
                .Filter(u => userIds.Contains(u.Uid))
                .Get()
                .ToList();

            return users;
        }

        public List<Sme> GetSubjectMatterList()
        {
            return _repoSme.Query().Get().ToList();
        }
               
    }

}

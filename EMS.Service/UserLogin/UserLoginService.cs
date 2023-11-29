using System;
using System.Collections.Generic;
using System.Linq;
using EMS.Data;
using EMS.Repo;
using EMS.Core;
using EMS.Dto;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Service
{
    public class UserLoginService : IUserLoginService
    {
        #region "Fields"
        private IRepository<UserLogin> repoUserMaster;
        private IRepository<User_Tech> _repoUserTech;
        private IRepository<Role> RepoRole;
        private IRepository<RoleCategory> RepoRoleCate;
        private IRepository<Designation> RepoDesi;
        private IRepository<DailyThought> repoDailyThought;
        private IRepository<ProjectDeveloper> repoProjectDeveloper;
        private readonly IRepository<UserDocument> userDocument;
        private IRepository<ExperienceType> _repoExperienceType;
        private IRepository<DomainExperts> _repoDomainExperts;
        private Microsoft.Extensions.Configuration.IConfiguration _configuration;
        #endregion

        #region "Cosntructor"
        public UserLoginService(IRepository<DailyThought> _repoDailyThought, IRepository<UserLogin> _repoUserMaster, IRepository<Role> _RepoRole,
            IRepository<ProjectDeveloper> _repoProjectDeveloper, IRepository<UserDocument> _userDocument, IRepository<RoleCategory> _RepoRoleCate, IRepository<Designation> _RepoDesi, IRepository<ExperienceType> repoExperienceType, IRepository<User_Tech> repoUserTech, IRepository<DomainExperts> repoDomainExperts, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            repoUserMaster = _repoUserMaster;
            RepoRole = _RepoRole;
            repoDailyThought = _repoDailyThought;
            repoProjectDeveloper = _repoProjectDeveloper;
            userDocument = _userDocument;
            RepoRoleCate = _RepoRoleCate;
            RepoDesi = _RepoDesi;
            _repoExperienceType = repoExperienceType;
            _repoUserTech = repoUserTech;
            _repoDomainExperts = repoDomainExperts;
            _configuration = configuration;
        }
        #endregion

        public UserLogin GetLoginDeatils(string email, string password)
        {
            return repoUserMaster.Query().Filter(x => (x.UserName.ToLower() == email.ToLower() || x.EmailOffice.ToLower() == email.ToLower()) && String.Compare(x.PasswordKey, password, false) == 0 && x.IsActive == true).Get().FirstOrDefault();
        }

        public UserLogin GetLoginDeatilByEmail(string email)
        {
            return repoUserMaster.Query().Filter(x => x.EmailOffice.ToLower() == email.ToLower()).Get().FirstOrDefault();
        }
        public UserLogin GetUserByUserName(string name)
        {
            return repoUserMaster.Query().Filter(T => T.UserName.Equals(name)).Get().FirstOrDefault();
        }
        public UserLogin GetUserInfoByID(int Uid)
        {
            return repoUserMaster.Query().Filter(x => x.Uid == Uid).Get().FirstOrDefault();
        }

        public bool CheckAdditionalAccess(int uid)
        {
            if (uid > 0)
            {
                var userInfo = GetUserInfoByID(uid);
                var IsAdditionalAccess = userInfo != null ? userInfo.IsExpensesAllowed.HasValue ? userInfo.IsExpensesAllowed.Value : false : false;
                return IsAdditionalAccess;
            }
            else
            {
                return false;
            }
        }
        public List<UserLogin> GetUserInfoByID(int[] Uid)
        {
            return repoUserMaster.Query().Filter(x => Uid.Contains(x.Uid)).Get().ToList();
        }
        public List<UserLogin> GetUserByRole(int RoleId, bool IsActive = false)
        {
            return repoUserMaster.Query().Filter(R => R.RoleId == RoleId && (R.IsActive == true || R.IsActive == IsActive)).Get().OrderBy(T => T.UserName).ToList();
        }
        public List<UserLogin> GetUserByDesignation(int[] Designation, bool IsActive = false)
        {
            return repoUserMaster.Query().Filter(R => Designation.Contains(R.DesignationId.Value) && (R.IsActive == true || R.IsActive == IsActive)).Get().OrderBy(T => T.UserName).ToList();
        }
        public List<UserLogin> GetUsersByRoleOld(int id, int roleId)
        {
            List<UserLogin> list = null;
            try
            {
                if (roleId == (int)Enums.UserRoles.PM)
                {
                    return repoUserMaster.Query()
                            .Filter(T => (T.IsActive == true
                        && RoleValidator.TL_RoleIds.Contains(T.RoleId.Value) && T.PMUid == id))
                        .Get().OrderBy(T => T.Name).ToList();
                }
                else if (roleId == (int)Enums.UserRoles.HRBP)
                {
                    return repoUserMaster.Query()
                        .Filter(T => T.IsActive == true
                        && (T.RoleId == (int)Enums.UserRoles.PM)).Get().OrderBy(T => T.Name).ToList();
                }
                else if (RoleValidator.TL_RoleIds.Contains(roleId))
                {
                    return repoUserMaster.Query().Filter(T => T.IsActive == true && T.TLId == id
                    && RoleValidator.TL_RoleIds.Contains(T.RoleId.Value)).Get().OrderBy(T => T.Name).ToList();
                }
                else
                {
                    return repoUserMaster.Query().Filter(T => T.IsActive == true && T.PMUid == id
                    && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.Name).ToList();
                }
            }
            catch
            {

            }
            return list;
        }
        public List<UserLogin> GetUsersByRole(int id, int roleId, int _DesignationId)
        {
            List<UserLogin> list = null;
            try
            {
                if (roleId == (int)Enums.UserRoles.PM)
                {
                    return repoUserMaster.Query()
                            .Filter(T => (T.IsActive == true
                        && (RoleValidator.TL_Technical_DesignationIds.Contains(T.DesignationId.Value)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(T.DesignationId.Value)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(T.DesignationId.Value)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(T.DesignationId.Value)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(T.DesignationId.Value)
            || RoleValidator.TL_Sales_DesignationIds.Contains(T.DesignationId.Value)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(T.DesignationId.Value)
            || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(T.DesignationId.Value)
            || RoleValidator.TL_HR_DesignationIds.Contains(T.DesignationId.Value))
                        && T.PMUid == id))
                        .Get().OrderBy(T => T.Name).ToList();
                }
                else if (roleId == (int)Enums.UserRoles.HRBP)
                {
                    return repoUserMaster.Query()
                        .Filter(T => T.IsActive == true
                        && (T.RoleId == (int)Enums.UserRoles.PM)).Get().OrderBy(T => T.Name).ToList();
                }
                else if (RoleValidator.TL_Technical_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_Sales_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_HR_DesignationIds.Contains(_DesignationId))
                {
                    return repoUserMaster.Query().Filter(T => T.IsActive == true && T.TLId == id
                    && (RoleValidator.TL_Technical_DesignationIds.Contains(T.DesignationId.Value)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(T.DesignationId.Value)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(T.DesignationId.Value)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(T.DesignationId.Value)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(T.DesignationId.Value)
            || RoleValidator.TL_Sales_DesignationIds.Contains(T.DesignationId.Value)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(T.DesignationId.Value)
            || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(T.DesignationId.Value)
            || RoleValidator.TL_HR_DesignationIds.Contains(T.DesignationId.Value))
                    ).Get().OrderBy(T => T.Name).ToList();
                }
                else
                {
                    return repoUserMaster.Query().Filter(T => T.IsActive == true && T.PMUid == id
                    && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.Name).ToList();
                }
            }
            catch
            {

            }
            return list;
        }
        public List<UserLogin> GetUsersListByAllRole(int ID, int _RoleId)
        {            
            if (_RoleId == (int)Enums.UserRoles.PM)
            {
                return repoUserMaster.Query().Filter(T => T.IsActive == true && (T.PMUid == ID || T.TLId == ID) && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.Name).ToList();
            }
            else if (_RoleId == (int)Enums.UserRoles.UKPM)
            {
                return repoUserMaster.Query().Filter(T => T.IsActive == true && (T.PMUid == ID || T.TLId == ID || T.Uid == ID) && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.Name).ToList();
            }
            else if (RoleValidator.HR_RoleIds.Contains(_RoleId))
            {
                return repoUserMaster.Query().Filter(T => T.IsActive == true).Get().OrderBy(T => T.Name).ToList();
            }
            else if (RoleValidator.TL_RoleIds.Contains(_RoleId))
            {
                int[] sdList = repoUserMaster.Query().Filter(T => T.TLId == ID).Get().Select(T => T.Uid).ToArray();
                return repoUserMaster.Query().Filter(T => (T.IsActive == true) && (T.TLId == ID || sdList.Contains((int)T.TLId)) && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(A => A.Name).ToList();
            }
            //else if (RoleValidator.DV_RoleIds.Contains(_RoleId))
            //{
            //    return repoUserMaster.Query().Filter(T => (T.IsActive == true) && T.TLId == ID && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(A => A.Name).ToList();
            //}
            else if (_RoleId == (int)Enums.UserRoles.UKBDM)
            {
                var PMUser = repoUserMaster.Query().Filter(T => T.Uid == ID).Get().FirstOrDefault();
                int pmuid = PMUser.PMUid.HasValue ? PMUser.PMUid.Value : 0;
                return repoUserMaster.Query().Filter(T => (T.IsActive == true) && T.PMUid == pmuid && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(A => A.Name).ToList();
            }
            else
                return repoUserMaster.Query().Filter(T => T.IsActive == true && T.PMUid == ID && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.Name).ToList();
        }
        public List<UserLogin> GetUsersListByAllDesignation(int ID, int _RoleId,int _Designation)
        {
            if (_RoleId == (int)Enums.UserRoles.PM)
            {
                return repoUserMaster.Query().Filter(T => T.IsActive == true && (T.PMUid == ID || T.TLId == ID) && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.Name).ToList();
            }
            else if (_RoleId == (int)Enums.UserRoles.UKPM)
            {
                return repoUserMaster.Query().Filter(T => T.IsActive == true && (T.PMUid == ID || T.TLId == ID || T.Uid == ID) && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.Name).ToList();
            }
            else if (RoleValidator.HR_RoleIds.Contains(_RoleId))
            {
                return repoUserMaster.Query().Filter(T => T.IsActive == true).Get().OrderBy(T => T.Name).ToList();
            }
            else if (RoleValidator.TL_Technical_DesignationIds.Contains(_Designation)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(_Designation)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(_Designation)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(_Designation)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(_Designation)
            || RoleValidator.TL_Sales_DesignationIds.Contains(_Designation)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(_Designation)
            || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(_Designation)
            || RoleValidator.TL_HR_DesignationIds.Contains(_Designation))
            {
                int[] sdList = repoUserMaster.Query().Filter(T => T.TLId == ID).Get().Select(T => T.Uid).ToArray();
                return repoUserMaster.Query().Filter(T => (T.IsActive == true) && (T.TLId == ID || sdList.Contains((int)T.TLId)) && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(A => A.Name).ToList();
            }
            //else if (RoleValidator.DV_RoleIds.Contains(_RoleId))
            //{
            //    return repoUserMaster.Query().Filter(T => (T.IsActive == true) && T.TLId == ID && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(A => A.Name).ToList();
            //}
            else if (_RoleId == (int)Enums.UserRoles.UKBDM)
            {
                var PMUser = repoUserMaster.Query().Filter(T => T.Uid == ID).Get().FirstOrDefault();
                int pmuid = PMUser.PMUid.HasValue ? PMUser.PMUid.Value : 0;
                return repoUserMaster.Query().Filter(T => (T.IsActive == true) && T.PMUid == pmuid && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(A => A.Name).ToList();
            }
            else
                return repoUserMaster.Query().Filter(T => T.IsActive == true && T.PMUid == ID && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.Name).ToList();
        }

        public List<UserLogin> GetAllUsersList()
        {
            return repoUserMaster.Query().Get().OrderBy(T => T.Name).ThenByDescending(T => T.ModifyDate).ToList();
        }

        public List<UserLogin> GetAllActiveUsersList()
        {
            return repoUserMaster.Query().Filter(u => u.IsActive == true).Get().OrderBy(T => T.Name).ThenByDescending(T => T.ModifyDate).ToList();
        }
        public List<ExperienceType> GetAllExperienceTypeList()
        {
            return _repoExperienceType.Query().Filter(u => u.IsActive == true).Get().OrderBy(T => T.Id).ToList();
        }


        // public List<UserLogin> GetUserListByRolesAndDepartments(int[] roleIds, )

        public List<UserLogin> GetUserByUKBDMUid(int PMUid)
        {
            return repoUserMaster.Query().Filter(T => T.IsActive.Value == true && T.PMUid == PMUid).Get().OrderBy(T => T.Name).ToList();
        }
        public List<UserLogin> GetPMAndPMOUsers(bool IsActive = false)
        {
            return repoUserMaster.Query().Filter(R => (R.RoleId == (int)Enums.UserRoles.PMO || R.RoleId == (int)Enums.UserRoles.PM || R.RoleId == (int)Enums.UserRoles.HRBP) && (R.IsActive == true || R.IsActive == IsActive)).Get().OrderBy(T => T.UserName).ToList();
        }
        public List<UserLogin> GetPMUsers(bool IsActive = false)
        {
            return repoUserMaster.Query().Filter(R => (R.RoleId == (int)Enums.UserRoles.Director || R.RoleId == (int)Enums.UserRoles.PM || R.RoleId == (int)Enums.UserRoles.HRBP || R.RoleId == (int)Enums.UserRoles.UKPM) && (R.IsActive == true || R.IsActive == IsActive)).Get().OrderBy(T => T.UserName).ToList();
        }
        public List<UserLogin> GetPMAndPMOHRUsers(bool IsActive = false)
        {
            return repoUserMaster.Query().Filter(R => (R.RoleId == (int)Enums.UserRoles.PMO || R.RoleId == (int)Enums.UserRoles.PM || R.RoleId == (int)Enums.UserRoles.Director
            || RoleValidator.HR_RoleIds.Contains(R.RoleId.Value)) && (R.IsActive == true || R.IsActive == IsActive)).Get().OrderBy(T => T.UserName).ToList();
        }
        public List<UserLogin> GetPMAndPMOHRDirectorUsers(bool IsActive = false)
        {
            return repoUserMaster.Query().Filter(R => (R.RoleId == (int)Enums.UserRoles.PMO || R.RoleId == (int)Enums.UserRoles.PM
            || RoleValidator.HR_RoleIds.Contains(R.RoleId.Value) || R.RoleId == (int)Enums.UserRoles.Director) && (R.IsActive == true || R.IsActive == IsActive)).Get().OrderBy(T => T.UserName).ToList();
        }
        public List<UserLogin> GetTLSDUsers(int PMUid)
        {            
            return repoUserMaster.Query().Filter(T => (RoleValidator.TL_Technical_DesignationIds.Contains(T.DesignationId.Value)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(T.DesignationId.Value)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(T.DesignationId.Value)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(T.DesignationId.Value)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(T.DesignationId.Value)
            || RoleValidator.TL_Sales_DesignationIds.Contains(T.DesignationId.Value)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(T.DesignationId.Value)
            || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(T.DesignationId.Value)
            || RoleValidator.TL_HR_DesignationIds.Contains(T.DesignationId.Value)
            || T.RoleId == (int)Enums.UserRoles.PM || T.RoleId == (int)Enums.UserRoles.PMO || T.RoleId == (int)Enums.UserRoles.Director) && T.IsActive == true && (T.PMUid == PMUid || T.Uid == PMUid)).Get().OrderBy(A => A.Name).ToList();
        }
        //public List<UserLogin> GetWorkAlternators(int PMUid, int Uid)
        //{
        //    return repoUserMaster.Query().Filter(u => u.PMUid == PMUid  && u.RoleId != (int)Enums.UserRoles.HR).Get().ToList();
        //}
        public bool DeleteDocumentFile(int documentId)
        {
            userDocument.Delete(documentId);
            return true;
        }

        public List<string> GetPfDocumentFile(int userid)
        {
            return userDocument.Query().Filter(x => x.Uid == userid).Get().Select(x => x.DocumentPath).ToList();

        }

        public List<UserLogin> GetWorkAlternators(int PMUid, int Uid, int RoleId)
        {
            //&& u.RoleId != (int)Enums.UserRoles.HR (Remove)
            bool RoleType = RoleValidator.HR_RoleIds.Contains(RoleId);
            return RoleType ? repoUserMaster.Query().Filter(u => u.IsActive == true && (u.PMUid == PMUid || u.RoleId == RoleId)).Get().OrderBy(A => A.Name).ToList() : repoUserMaster.Query().Filter(u => u.IsActive == true && u.PMUid == PMUid && u.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(A => A.Name).ToList();
        }

        public List<UserLogin> GetUsersByPM(int PMUid)
        {
            return repoUserMaster.Query()
                                .Filter(T => T.IsActive == true && (T.PMUid == PMUid || T.TLId == PMUid || T.Uid == PMUid))
                                .OrderBy(o => o.OrderBy(x => x.Name))
                                .Get().ToList();
        }
        public List<UserLogin> GetUsersByDepartment(int pmid, int DepartmentId)
        {
            if (pmid > 0)
            {
                return repoUserMaster.Query()
                                .Filter(T => T.IsActive == true && (T.PMUid == pmid || T.TLId == pmid || T.Uid == pmid) && T.DeptId == DepartmentId)
                                .OrderBy(o => o.OrderBy(x => x.Name))
                                .Get().ToList();
            }
            else
            {
                return repoUserMaster.Query()
                                    .Filter(T => T.IsActive == true && (T.DeptId == DepartmentId))
                                    .OrderBy(o => o.OrderBy(x => x.Name))
                                    .Get().ToList();
            }
        }
        public List<UserLogin> GetTLUsers(int TLId)
        {
            return repoUserMaster.Query().Filter(T => T.IsActive == true && T.TLId == TLId).Get().OrderBy(A => A.Name).ToList();
        }

        public List<UserLogin> GetUsersByRoles(int[] roleIds, int? PMUid)
        {
            var users = new List<UserLogin>();

            if (roleIds != null && roleIds.Length > 0)
            {
                users = repoUserMaster.Query()
                            .Filter(u => u.RoleId.HasValue && roleIds.Contains(u.RoleId.Value) && u.IsActive == true &&
                                   (!PMUid.HasValue || u.PMUid == PMUid.Value || u.Uid == PMUid.Value))
                                   .Get().ToList();
            }

            return users;
        }

        public bool UserTechDeleted(UserLogin userloginUpdated)
        {
            repoUserMaster.ChangeEntityCollectionState(userloginUpdated.User_Tech, ObjectState.Deleted);
            userloginUpdated.User_Tech.Clear();
            repoUserMaster.SaveChanges();
            return true;
        }

        public bool UserDomainDeleted(UserLogin userloginUpdated)
        {
            repoUserMaster.ChangeEntityCollectionState(userloginUpdated.DomainExperts, ObjectState.Deleted);
            userloginUpdated.DomainExperts.Clear();
            repoUserMaster.SaveChanges();
            return true;
        }


        public void Save(UserLogin entity)
        {
            if (entity.Uid == 0)
            {
                entity.AddDate = DateTime.Now;
                repoUserMaster.InsertGraph(entity);
            }
            else
            {
                var userloginEntity = repoUserMaster.FindById(entity.Uid);
                var userloginUpdated = repoUserMaster.Update(userloginEntity, entity);


                //repoUserMaster.ChangeEntityCollectionState(userloginUpdated.User_Tech, ObjectState.Deleted);
                //userloginUpdated.User_Tech.Clear();

                if (entity.User_Tech != null && entity.User_Tech.Any())
                {
                    entity.User_Tech.ToList().ForEach(a =>
                    {
                        userloginUpdated.User_Tech.Add(a);
                    });
                }

                repoUserMaster.SaveChanges();


            }

        }

        public void UpdateStatus(UserLogin entity)
        {
            repoUserMaster.Update(entity);
        }

        public List<UserLogin> GetUsers(bool IsActive = false)
        {
            return repoUserMaster.Query().Filter(T => (T.IsActive.HasValue ? T.IsActive.Value : true)).Get().OrderBy(t => t.Name).ThenByDescending(T => T.ModifyDate).ToList();

        }

        public UserLogin GetUser(int hrmId, string empCode)
        {
            return repoUserMaster.Query().Filter(T => T.HRMId == hrmId && T.EmpCode == empCode).Get().FirstOrDefault();
        }

        public void ChangePassword(UserLogin userLogin)
        {
            if (userLogin != null)
            {
                repoUserMaster.ChangeEntityState<UserLogin>(userLogin, ObjectState.Modified);
                repoUserMaster.SaveChanges();
            }

        }

        public string CheckUser(string email, string username)
        {
            var officeEmail = repoUserMaster.Query().Filter(x => x.EmailOffice == email).Get().ToList();
            var user = repoUserMaster.Query().Filter(x => x.UserName == username).Get().ToList();
            if (officeEmail.Count() > 0)
            {
                return "emailerror";
            }
            else if (user.Count() > 0)
            {
                return "usernameerror";
            }
            else
            {
                return "noerror";
            }

        }
        public bool CheckUserAttendanceId(string email, int attendanceId)
        {
            var user = repoUserMaster.Query().Filter(x => x.AttendenceId == attendanceId && x.EmailOffice != email).Get().ToList();
            if (user.Count() > 0)
            {
                return false;
            }

            else
            {
                return true;
            }

        }

        public bool CheckHRMId(int hrmId)
        {
            return repoUserMaster.Query().Filter(u => u.HRMId == hrmId).Get().Any();
        }

        public bool CheckAttendanceId(int attendanceId)
        {
            return repoUserMaster.Query().Filter(u => u.AttendenceId == attendanceId).Get().Any();
        }

        public UserLogin GetUserByAttendanceId(int attendanceId)
        {
            return repoUserMaster.Query().Filter(x => x.AttendenceId == attendanceId).Get().FirstOrDefault();
        }

        public bool CheckEmpCode(string empCode)
        {
            return repoUserMaster.Query().Filter(u => u.EmpCode == empCode).Get().Any();
        }

        public ICollection<UserLogin> GetUsersList(out int total, PagingService<UserLogin> pagingService)
        {
            return repoUserMaster.Query()
                .Include(x => x.Designation)
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort)
                .GetPage(pagingService.Start, pagingService.Length, out total)
                .ToList();
        }

        public List<Role> GetRolesWithoutPM(bool IsActive = false)
        {
            return RepoRole.Query().Filter(R => (R.IsActive == IsActive || R.IsActive == true) && R.RoleName.ToLower() != "project manager").Get().OrderBy(T => T.RoleName).ToList();
        }
        public List<Role> GetRoles(bool IsActive = false)
        {
            return RepoRole.Query().Filter(R => (R.IsActive == IsActive || R.IsActive == true)).Get().OrderBy(T => T.RoleName).ToList();
        }

        //public List<UserLogin> GetUsers1(int PMId, List<int> TLId, List<int> SDId)
        //{
        //    return repoUserMaster.Query().Filter(R => (R.RoleId == PMId || SDId.Contains(R.RoleId.Value) || TLId.Contains(R.RoleId.Value)) && R.IsActive == true).Get().OrderBy(T => T.UserName).ToList();
        //}
        public List<UserLogin> GetUsers1(int PMId)
        {            
            return repoUserMaster.Query().Filter(R => (R.RoleId == PMId 
            || RoleValidator.TL_Technical_DesignationIds.Contains(R.DesignationId.Value)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(R.DesignationId.Value)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(R.DesignationId.Value)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(R.DesignationId.Value)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(R.DesignationId.Value)
            || RoleValidator.TL_Sales_DesignationIds.Contains(R.DesignationId.Value)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(R.DesignationId.Value)
            || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(R.DesignationId.Value)
            || RoleValidator.TL_HR_DesignationIds.Contains(R.DesignationId.Value)
            ) && R.IsActive == true).Get().OrderBy(T => T.UserName).ToList();
        }

        public List<string> GetUserEmailIdsByRolesAndDepartments(int[] roleIds, int[] departmentIds)
        {
            roleIds = roleIds != null ? roleIds : new int[] { };
            departmentIds = departmentIds != null ? departmentIds : new int[] { };

            return repoUserMaster.Query()
                    .Filter(x => x.EmailOffice != null && x.EmailOffice != "" && (x.RoleId.HasValue && roleIds.Contains(x.RoleId.Value)) ||
                        (x.DeptId.HasValue && departmentIds.Contains(x.DeptId.Value)))
                    .GetQuerable()
                    .Select(x => x.EmailOffice)
                    .Distinct()
                    .ToList();
        }

        public List<UserLogin> GetUsers(int RoleId, int Uid, int PMUid)
        {
            return repoUserMaster.Query()
                .Filter(T => (T.IsActive.HasValue ? T.IsActive.Value : true))
                .Get()
                .OrderBy(t => t.Name)
                .ThenByDescending(T => T.ModifyDate)
                .Where(x => (x.PMUid.Equals(RoleId == (int)Enums.UserRoles.PM ? Uid : PMUid) || x.Uid.Equals(RoleId == (int)Enums.UserRoles.PM ? Uid : PMUid)) && x.RoleId != (int)Enums.UserRoles.HRBP)
                .ToList();
        }

        public DailyThought GetDailyThought()
        {
            return repoDailyThought.Query().Get().FirstOrDefault();
        }

        public List<UserLogin> GetUsersListForHierarchyByAllRoleOld(int id, int roleId)
        {
            List<UserLogin> list = null;
            try
            {
                if (roleId == (int)Enums.UserRoles.PM)
                {
                    return repoUserMaster.Query().Filter(T => T.IsActive == true && T.PMUid == id
                    && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.Name).ToList();
                }
                else if (roleId == (int)Enums.UserRoles.HRBP)
                {
                    return repoUserMaster.Query()
                        .Filter(T => T.IsActive == true
                        && (T.RoleId != (int)Enums.UserRoles.HRBP
                        || T.RoleId == (int)Enums.UserRoles.PM || T.TLId == id)).Get().OrderBy(T => T.Name).ToList();
                }
                else if (RoleValidator.TL_RoleIds.Contains(roleId))
                {
                    int[] sdList = repoUserMaster.Query().Filter(T => T.TLId == id).Get().Select(T => T.Uid).ToArray();
                    return repoUserMaster.Query().Filter(T => (T.IsActive == true)
                    && (T.TLId == id || sdList.Contains((int)T.TLId))
                    && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(A => A.Name).ToList();
                }
                else if (RoleValidator.DV_RoleIds.Contains(roleId))
                {
                    return repoUserMaster.Query()
                        .Filter(T => (T.IsActive == true)
                        && T.TLId == id && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(A => A.Name).ToList();
                }
                else
                {
                    return repoUserMaster.Query().Filter(T => T.IsActive == true && T.PMUid == id && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.Name).ToList();
                }

                //switch (roleId)
                //{
                //    case (int)Enums.UserRoles.PM:
                //        return repoUserMaster.Query().Filter(T => T.IsActive == true && T.PMUid == id
                //        && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.Name).ToList();
                //    case (int)Enums.UserRoles.HRBP:
                //        return repoUserMaster.Query()
                //            .Filter(T => T.IsActive == true
                //            && (T.RoleId != (int)Enums.UserRoles.HRBP
                //            || T.RoleId == (int)Enums.UserRoles.PM || T.TLId == id)).Get().OrderBy(T => T.Name).ToList();
                //    case (int)Enums.UserRoles.DVManagerial:
                //    case (int)Enums.UserRoles.DVPManagerial:
                //    case (int)Enums.UserRoles.QAManagerial:
                //    case (int)Enums.UserRoles.QAPManagerial:
                //    case (int)Enums.UserRoles.UIUXManagerial:
                //    case (int)Enums.UserRoles.GamingManagerial:
                //    case (int)Enums.UserRoles.UIUXDesigner:
                //        int[] sdList = repoUserMaster.Query().Filter(T => T.TLId == id).Get().Select(T => T.Uid).ToArray();
                //        return repoUserMaster.Query().Filter(T => (T.IsActive == true)
                //        && (T.TLId == id || sdList.Contains((int)T.TLId))
                //        && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(A => A.Name).ToList();
                //    case (int)Enums.UserRoles.DV:
                //        return repoUserMaster.Query()
                //            .Filter(T => (T.IsActive == true)
                //            && T.TLId == id && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(A => A.Name).ToList();
                //    default:
                //        return repoUserMaster.Query().Filter(T => T.IsActive == true && T.PMUid == id && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.Name).ToList();
                //}
            }
            catch
            {

            }
            return list;
        }
        public List<UserLogin> GetUsersListForHierarchyByAllRole(int id, int roleId, int _DesignationId)
        {
            List<UserLogin> list = null;
            try
            {
                if (roleId == (int)Enums.UserRoles.PM)
                {
                    return repoUserMaster.Query().Filter(T => T.IsActive == true && T.PMUid == id
                    && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.Name).ToList();
                }
                else if (roleId == (int)Enums.UserRoles.HRBP)
                {
                    return repoUserMaster.Query()
                        .Filter(T => T.IsActive == true
                        && (T.RoleId != (int)Enums.UserRoles.HRBP
                        || T.RoleId == (int)Enums.UserRoles.PM || T.TLId == id)).Get().OrderBy(T => T.Name).ToList();
                }
                else if (RoleValidator.TL_Technical_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_Sales_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_HR_DesignationIds.Contains(_DesignationId))
                {
                    int[] sdList = repoUserMaster.Query().Filter(T => T.TLId == id).Get().Select(T => T.Uid).ToArray();
                    return repoUserMaster.Query().Filter(T => (T.IsActive == true)
                    && (T.TLId == id || sdList.Contains((int)T.TLId))
                    && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(A => A.Name).ToList();
                }               
                else
                {
                    return repoUserMaster.Query().Filter(T => T.IsActive == true && T.PMUid == id && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.Name).ToList();
                }

                //switch (roleId)
                //{
                //    case (int)Enums.UserRoles.PM:
                //        return repoUserMaster.Query().Filter(T => T.IsActive == true && T.PMUid == id
                //        && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.Name).ToList();
                //    case (int)Enums.UserRoles.HRBP:
                //        return repoUserMaster.Query()
                //            .Filter(T => T.IsActive == true
                //            && (T.RoleId != (int)Enums.UserRoles.HRBP
                //            || T.RoleId == (int)Enums.UserRoles.PM || T.TLId == id)).Get().OrderBy(T => T.Name).ToList();
                //    case (int)Enums.UserRoles.DVManagerial:
                //    case (int)Enums.UserRoles.DVPManagerial:
                //    case (int)Enums.UserRoles.QAManagerial:
                //    case (int)Enums.UserRoles.QAPManagerial:
                //    case (int)Enums.UserRoles.UIUXManagerial:
                //    case (int)Enums.UserRoles.GamingManagerial:
                //    case (int)Enums.UserRoles.UIUXDesigner:
                //        int[] sdList = repoUserMaster.Query().Filter(T => T.TLId == id).Get().Select(T => T.Uid).ToArray();
                //        return repoUserMaster.Query().Filter(T => (T.IsActive == true)
                //        && (T.TLId == id || sdList.Contains((int)T.TLId))
                //        && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(A => A.Name).ToList();
                //    case (int)Enums.UserRoles.DV:
                //        return repoUserMaster.Query()
                //            .Filter(T => (T.IsActive == true)
                //            && T.TLId == id && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(A => A.Name).ToList();
                //    default:
                //        return repoUserMaster.Query().Filter(T => T.IsActive == true && T.PMUid == id && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.Name).ToList();
                //}
            }
            catch
            {

            }
            return list;
        }

        private DailyThought GetDailyThoughtById(int id)
        {
            return repoDailyThought.FindById(id);
        }

        public DailyThought SaveDailyThought(DailyThoughtDto model)
        {
            DailyThought entity = null;
            if (model.Id > 0)
            {
                entity = GetDailyThoughtById(model.Id);
                if (entity != null)
                {
                    entity.Thought1 = model.Thought1;
                    entity.Thought2 = model.Thought2;
                    repoDailyThought.SaveChanges();
                }
            }
            else
            {
                entity.Thought1 = model.Thought1;
                entity.Thought2 = model.Thought2;
                repoDailyThought.Insert(entity);
            }
            return entity;
        }

        public UserLogin GetUsersById(int id)
        {
            return repoUserMaster.Query().Filter(T => T.Uid == id).Get().FirstOrDefault();
        }
        public List<UserLogin> GetUserReportByPaging(out int total, PagingService<UserLogin> pagingService)
        {
            return repoUserMaster.Query()
                .Filter(pagingService.Filter).
                OrderBy(pagingService.Sort).
                GetPage(pagingService.Start, pagingService.Length, out total).
                ToList();
        }


        public List<UserLogin> GetUsersListByUserIds(List<int> Uids, bool IsActive)
        {
            return repoUserMaster.Query().Filter(U => Uids.Contains(U.Uid) && U.IsActive == IsActive).Get().ToList();
        }

        public List<UserLogin> GetUsersDetailsByDeptId(int[] ids)
        {
            return repoUserMaster.Query().Filter(t => ids.Contains(t.DeptId ?? 0) && t.IsActive == true).Get().ToList();
        }

        public UserLogin GetForgotMailByEmailId(string mailId)
        {
            return repoUserMaster.Query().Filter(R => R.EmailOffice == mailId).Get().FirstOrDefault();
        }

        public List<UserLogin> GetUsersWithApidata()
        {
            return repoUserMaster.Query().Filter(T => (T.IsActive.HasValue ? T.IsActive.Value : true) && T.RoleId == (int)Enums.UserRoles.PM && T.CRMUserId.HasValue && T.CRMUserId.Value != 0 && T.ApiPassword != null).Get().ToList();
        }

        public List<UserLogin> GetResignedUsersByPM(int PMUid)
        {
            var result = (dynamic)null;
            try
            {
                result = repoUserMaster.Query().Get().Where(T => T.IsResigned == true && (T.PMUid == PMUid || (T.Uid == PMUid)))
                           .OrderBy(o => o.Name).ToList();
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }
            return result;
        }

        public List<string> GetUserEmailIdsByDepartment(int departmentId)
        {
            return repoUserMaster.Query()
                    .Filter(x => x.EmailOffice != null && x.EmailOffice != "" &&
                        (x.DeptId.HasValue && x.DeptId == departmentId))
                    .GetQuerable()
                    .Select(x => x.EmailOffice.Trim().ToLower())
                    .Distinct()
                    .ToList();
        }

        #region "Dispose"
        public void Dispose()
        {
            if (repoUserMaster != null)
            {
                repoUserMaster.Dispose();
                repoUserMaster = null;
            }
            if (_repoUserTech != null)
            {
                _repoUserTech.Dispose();
                _repoUserTech = null;
            }
        }
        #endregion
        public string GetNameById(int uid)
        {
            string Name = repoUserMaster.Query().Filter(u => u.Uid == uid).Get().FirstOrDefault()?.Name;
            return string.IsNullOrWhiteSpace(Name) ? "" : Name;

        }
        public string GetPmEmailId(int uid)
        {
            string emailId = repoUserMaster.Query().Filter(u => u.Uid == uid).Get().FirstOrDefault()?.EmailOffice;
            return string.IsNullOrWhiteSpace(emailId) ? "" : emailId;

        }
        public List<TLEmployeeDto> GetUserTLTogether(int pmUserId)
        {
            var users = repoUserMaster.Query().Filter(u => u.IsActive == true).Get().ToList();

            var query = from user in users
                        join tl in users
                        on user.TLId equals tl.Uid into gj
                        from subTL in gj.DefaultIfEmpty()
                        where (user.PMUid == pmUserId || user.TLId == pmUserId || user.Uid == pmUserId)
                        select new TLEmployeeDto
                        {
                            Uid = user.Uid,
                            EmployeeEmail = user?.EmailOffice?.Trim().ToLower(),
                            EmployeeName = user.Name,
                            MobileNumber = user.MobileNumber,
                            TLId = user.TLId,
                            TLEmail = subTL?.EmailOffice,
                            TLName = subTL?.Name,
                            DepartmentName = user.Department.Name
                        };
            return query.ToList();
        }

        public List<UserLogin> GetSelfAndUsersListByAllRole(int ID, int _RoleId,int _DesignationId)
        {
            if (_RoleId == (int)Enums.UserRoles.PM)
            {
                return repoUserMaster.Query().Filter(T => T.IsActive == true && (T.PMUid == ID || T.TLId == ID || T.Uid == ID) && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.Name).ToList();
            }
            else if (_RoleId == (int)Enums.UserRoles.UKPM)
            {
                return repoUserMaster.Query().Filter(T => T.IsActive == true && (T.PMUid == ID || T.TLId == ID || T.Uid == ID) && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.Name).ToList();
            }
            else if (RoleValidator.HR_RoleIds.Contains(_RoleId))
            {
                return repoUserMaster.Query().Filter(T => T.IsActive == true).Get().OrderBy(T => T.Name).ToList();
            }
            else if (RoleValidator.TL_Technical_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_Sales_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_HR_DesignationIds.Contains(_DesignationId))
            {
                int[] sdList = repoUserMaster.Query().Filter(T => T.TLId == ID).Get().Select(T => T.Uid).ToArray();
                return repoUserMaster.Query().Filter(T => (T.IsActive == true) && (T.TLId == ID || T.Uid == ID || sdList.Contains((int)T.TLId)) && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(A => A.Name).ToList();
            }
            //else if (RoleValidator.DV_RoleIds.Contains(_RoleId))
            //{
            //    return repoUserMaster.Query().Filter(T => (T.IsActive == true) && (T.TLId == ID || T.Uid == ID) && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(A => A.Name).ToList();
            //}
            else if (_RoleId == (int)Enums.UserRoles.UKBDM)
            {
                int pmuid = repoUserMaster.Query().Filter(T => T.Uid == ID).Get().Single().PMUid.Value;
                return repoUserMaster.Query().Filter(T => (T.IsActive == true) && T.PMUid == pmuid && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(A => A.Name).ToList();
            }
            else
                return repoUserMaster.Query().Filter(T => T.IsActive == true && T.PMUid == ID && T.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.Name).ToList();
        }
        public List<string> GetUsersEmailByRole(int pmUid, int roleId)
        {
            List<string> list = null;
            try
            {
                return repoUserMaster.Query().Filter(T => T.IsActive == true && T.PMUid == pmUid
                && T.RoleId == roleId && !String.IsNullOrEmpty(T.EmailOffice)).Get().Select(x => x.EmailOffice).ToList();
            }
            catch
            {
            }
            return list;
        }


        public List<UserLogin> GetAllUsers()
        {
            return repoUserMaster.Query().Filter(u => u.IsActive.HasValue && u.IsActive == true).Get().ToList();
        }

        public List<UserLogin> GetAllDotsquaresDevelopers()
        {
            return repoUserMaster.Query().Filter(u => u.IsActive.HasValue && u.IsActive == true &&
            (RoleValidator.TL_Technical_DesignationIds.Contains(u.DesignationId.Value)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(u.DesignationId.Value)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(u.DesignationId.Value)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(u.DesignationId.Value)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(u.DesignationId.Value)
            || RoleValidator.TL_Sales_DesignationIds.Contains(u.DesignationId.Value)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(u.DesignationId.Value)
            || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(u.DesignationId.Value)
            || RoleValidator.TL_HR_DesignationIds.Contains(u.DesignationId.Value)
            || u.RoleId == ((int)Enums.UserRoles.PM) || u.RoleId == ((int)Enums.UserRoles.UKPM) || RoleValidator.BA_RoleIds.Contains(u.RoleId.Value))).Get().ToList();
        }

        public void UpdtUserEncryptData(UserLogin userLogin)
        {
            if (userLogin != null)
            {
                repoUserMaster.ChangeEntityState<UserLogin>(userLogin, ObjectState.Modified);
                repoUserMaster.SaveChanges();
            }
        }

        public List<UserLogin> GetRoleByPM(int pmId, List<int> roleId)
        {
            return repoUserMaster.Query()
                               .Filter(T => T.IsActive == true && (T.PMUid == pmId || T.Uid == pmId) && roleId.Contains(T.RoleId ?? 0))
                               .OrderBy(o => o.OrderBy(x => x.Name))
                               .Get().ToList();
        }
        public UserLogin GetLoginDeatilByUserNameOREmail(string username)
        {
            return repoUserMaster.Query().Filter(x => x.UserName.ToLower() == username.ToLower() || x.EmailOffice.ToLower() == username.ToLower()).Get().FirstOrDefault();
        }


        public List<RoleCategory> GetRolesCategory()
        {
            return RepoRoleCate.Query().Filter(R => R.IsActive == true).Get().OrderBy(T => T.Name).ToList();
        }
        public List<Designation> GetDesignationList(int? roleId)
        {

            if (roleId != null)
            {
                return RepoDesi.Query().Filter(R => R.IsActive == true && R.RoleId == roleId).Get().OrderBy(T => T.DisplayOrder).ToList();
            }
            else
            {
                return RepoDesi.Query().Filter(R => R.IsActive == true).Get().OrderBy(T => T.DisplayOrder).ToList();
            }

        }

        public List<Role> GetRolesByRoleCategoyId(int? RoleCateGoryId)
        {

            return RepoRole.Query().Filter(R => (R.RoleCategoryId == RoleCateGoryId && R.IsActive == true)).Get().OrderBy(T => T.RoleName).ToList();
        }

        public List<Designation> GetDesignationByRoleId(int RoleId)
        {

            return RepoDesi.Query().Filter(R => (R.RoleId == RoleId && R.IsActive == true)).Get().OrderBy(T => T.DisplayOrder).ToList();
        }

        public void SaveUserTech(User_Tech entity)
        {
            if (entity.Id > 0)
            {
                _repoUserTech.Update(entity);
            }
            else
            {
                _repoUserTech.Insert(entity);
            }
        }
        public void UserTech_Deleted(List<User_Tech> usertech)
        {
            if (usertech.Count > 0)
            {
                foreach(var entity in usertech)
                {
                    _repoUserTech.Delete(entity.Id);
                }                
            }
        }
        public List<User_Tech> GetUserTechByUid(int uid)
        {
           return _repoUserTech.Query().Filter(x => x.Uid == uid).Get().ToList();
        }
        public User_Tech GetUserTechByUidandTechId(int uid, int Techid)
        {
            return _repoUserTech.Query().Filter(x => x.Uid == uid && x.TechId == Techid).Get().FirstOrDefault();
        }
        public DomainExperts GetDomainExpertsByUid(int uid, int domainId)
        {
            return _repoDomainExperts.Query().Filter(x => x.Uid == uid && x.DomainId == domainId).Get().FirstOrDefault();
        }
        public List<DomainExperts> GetDomainExpertsByUid(int uid)
        {
            return _repoDomainExperts.Query().Filter(x => x.Uid == uid).Get().ToList();
        }
        public void SaveDomainExperts(DomainExperts entity, string action)
        {
            if (action == "Updated")
            {
                _repoDomainExperts.Update(entity);
            }
            else
            {
                _repoDomainExperts.Insert(entity);
            }
        }
        public void DomainExperts_Deleted(List<DomainExperts> domainExpert)
        {
            if (domainExpert.Count > 0)
            {
                foreach (var entity in domainExpert)
                {
                    //_repoDomainExperts.Delete(entity);
                    using (SqlConnection con = new SqlConnection(_configuration["ConnectionStrings:dsmanagementConnection"]))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("delete from DomainExperts where DomainId=@DomainId and uid=@uid", con))
                        {
                            cmd.Parameters.AddWithValue("@DomainId", entity.DomainId);
                            cmd.Parameters.AddWithValue("@uid", entity.Uid);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {                                
                            }
                        }
                        con.Close();
                    }
                }
            }            
        }

        public List<UserLogin> GetProjectListByCurrentUser(List<int?> EmployeeIds)
        {
            return repoUserMaster.Query().Filter(x => EmployeeIds.Contains(x.Uid)).Get().ToList();

        }

        public List<UserLogin> GetProjectListByCurrentUserId(List<int> EmployeeIds)
        {
            return repoUserMaster.Query().Filter(x => EmployeeIds.Contains(x.Uid)).Get().ToList();

        }

        public List<UserLogin> GetReminderUserList(List<int> EmployeeIds)
        {
            return repoUserMaster.Query().Filter(x => EmployeeIds.Contains(x.Uid)).Get().ToList();

        }
    }
}

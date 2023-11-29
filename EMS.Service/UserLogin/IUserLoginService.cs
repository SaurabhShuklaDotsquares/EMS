using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using EMS.Dto;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Service
{
    public interface IUserLoginService : IDisposable
    {
        UserLogin GetLoginDeatils(string email, string password);
        UserLogin GetLoginDeatilByEmail(string email);
        UserLogin GetUserInfoByID(int Uid);
        List<UserLogin> GetUserInfoByID(int[] Uid);
        List<UserLogin> GetUsersListByAllRole(int ID, int _RoleId);
        List<UserLogin> GetUsersListByAllDesignation(int ID, int _RoleId, int _Designation);
        List<UserLogin> GetUserByUKBDMUid(int PMUid);
        List<UserLogin> GetUserByRole(int RoleId, bool IsActive = false);
        List<UserLogin> GetUserByDesignation(int[] Designation, bool IsActive = false);
        UserLogin GetUserByUserName(string name);
        List<UserLogin> GetTLSDUsers(int PMUid);
        void Save(UserLogin entity);
        void UpdateStatus(UserLogin entity);
      
        void ChangePassword(UserLogin userLogin);
        List<UserLogin> GetUsers(bool IsActive = false);
        List<UserLogin> GetPMAndPMOUsers(bool IsActive = false);
        List<UserLogin> GetPMUsers(bool IsActive = false);
        List<UserLogin> GetPMAndPMOHRUsers(bool IsActive = false);
        List<UserLogin> GetPMAndPMOHRDirectorUsers(bool IsActive = false);
        //List<UserLogin> GetWorkAlternators(int PMUid, int Uid);
        List<UserLogin> GetWorkAlternators(int PMUid, int Uid, int RoleId);
        
        List<UserLogin> GetUsersByPM(int PMUid);
        List<UserLogin> GetUsersByDepartment(int pmid,int DepartmentId);
        List<UserLogin> GetTLUsers(int TLId);

        List<UserLogin> GetUsersByRoles(int[] roleIds, int? PMUid);

        string CheckUser(string email, string username);
        bool CheckUserAttendanceId(string email,int attendanceId);
        UserLogin GetUserByAttendanceId(int attendanceId);
        bool CheckHRMId(int hrmId);
        bool CheckAttendanceId(int attendanceId);
        bool CheckEmpCode(string empCode);
        bool CheckAdditionalAccess(int uid);

        ICollection<UserLogin> GetUsersList(out int total, PagingService<UserLogin> pagingService);
        List<Role> GetRolesWithoutPM(bool IsActive = false);
        List<Role> GetRoles(bool IsActive = false);

        UserLogin GetUser(int hrmId, string empCode);

        //List<UserLogin> GetUsers1(int PMId, List<int> TLId, List<int> SDId);
        List<UserLogin> GetUsers1(int PMId);

        List<string> GetUserEmailIdsByRolesAndDepartments(int[] roleIds, int[] departmentIds);

        List<UserLogin> GetUsers(int RoleId, int Uid, int PMUid);

        DailyThought GetDailyThought();
        DailyThought SaveDailyThought(DailyThoughtDto model);

        //List<UserLogin> GetUsersListForHierarchyByAllRole(int id, int roleId);
        List<UserLogin> GetUsersListForHierarchyByAllRole(int id, int roleId, int _DesignationId);
        UserLogin GetUsersById(int id);
        //List<UserLogin> GetUsersByRole(int id, int roleId);
        List<UserLogin> GetUsersByRole(int id, int roleId, int _DesignationId);
        bool UserTechDeleted(UserLogin userloginUpdated);
        bool UserDomainDeleted(UserLogin userloginUpdated);
        List<UserLogin> GetUserReportByPaging(out int total, PagingService<UserLogin> pagingService);

        List<UserLogin> GetUsersListByUserIds(List<int> Uids, bool IsActive = true);

        List<UserLogin> GetUsersDetailsByDeptId(int[] ids);
        UserLogin GetForgotMailByEmailId(string mailId);

        List<UserLogin> GetUsersWithApidata();

        string GetNameById(int uid);

        List<UserLogin> GetResignedUsersByPM(int PMUid);
        string GetPmEmailId(int uid);
        List<UserLogin> GetAllUsersList();
        List<UserLogin> GetAllActiveUsersList();
        List<ExperienceType> GetAllExperienceTypeList();
        List<string> GetUserEmailIdsByDepartment(int departmentId);
        List<TLEmployeeDto> GetUserTLTogether(int pmUserId);
        List<UserLogin> GetSelfAndUsersListByAllRole(int ID, int _RoleId,int _DesignationId);
        List<UserLogin> GetAllUsers();
        List<UserLogin> GetAllDotsquaresDevelopers();
        void UpdtUserEncryptData(UserLogin userLogin);


        List<UserLogin> GetRoleByPM(int pmId, List<int> roleId);
        List<string> GetUsersEmailByRole(int pmUid, int roleId);
        bool DeleteDocumentFile(int documentId);

        List<string> GetPfDocumentFile(int userid);

        UserLogin GetLoginDeatilByUserNameOREmail(string username);
        List<RoleCategory> GetRolesCategory();
        List<Designation> GetDesignationList(int? roleId);
        List<Role> GetRolesByRoleCategoyId(int? RoleCateGoryId);
        List<Designation> GetDesignationByRoleId(int RoleId);
        //IEnumerable<Designation> GetDesignationList(int? roleId);
        void SaveUserTech(User_Tech entity);
        List<User_Tech> GetUserTechByUid(int uid);
        void UserTech_Deleted(List<User_Tech> usertech);
        User_Tech GetUserTechByUidandTechId(int uid, int Techid);
        DomainExperts GetDomainExpertsByUid(int uid, int domainId);
        void SaveDomainExperts(DomainExperts entity, string action);
        List<DomainExperts> GetDomainExpertsByUid(int uid);
        void DomainExperts_Deleted(List<DomainExperts> usertech);
        List<UserLogin> GetProjectListByCurrentUser(List<int?> EmployeeIds);
        List<UserLogin> GetProjectListByCurrentUserId(List<int> EmployeeIds);
        List<UserLogin> GetReminderUserList(List<int> EmployeeIds);
    }
}

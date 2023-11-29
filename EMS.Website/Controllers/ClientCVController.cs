using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SelectPdf;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.IO;
using System;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using DataTables.AspNet.Core;
using EMS.Web.Code.LIBS;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using PagedList.Core;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using Microsoft.EntityFrameworkCore.Internal;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Castle.Core.Internal;
using System.Text;
using NUglify.Helpers;
using NPOI.POIFS.FileSystem;
using System.Data.SqlClient;

namespace EMS.Website.Controllers
{
    [CustomAuthorization()]
    public class ClientCVController : BaseController
    {
        private readonly IUserLoginService _userLoginService;
        private readonly ICVBuilderService _cVBuilderService;
        private readonly IProjectClosureService _projectClosureService;
        private readonly IProjectService _projectService;
        private readonly IHostingEnvironment _env;
        private readonly IDepartmentService _departmentService;
        private readonly IDomainTypeService _domainTypeService;
        private readonly ITechnologyParentService _technologyParentService;
        private readonly ITechnologyService _technologyService;
        private readonly ITechnologyParentMappingService _technologyParentMappingService;
        private readonly ICurrencyService _currencyService;
        private readonly IEstimateService _estimateService;
        public ClientCVController(IUserLoginService userLoginService, ICVBuilderService cVBuilderService, IProjectClosureService projectClosureService, IProjectService projectService, IHostingEnvironment env, IDepartmentService departmentService, IDomainTypeService domainTypeService, ITechnologyParentService technologyParentService, ITechnologyService technologyService, ITechnologyParentMappingService technologyParentMappingService, ICurrencyService currencyService, IEstimateService estimateService)
        {
            _cVBuilderService = cVBuilderService;
            _userLoginService = userLoginService;
            _projectClosureService = projectClosureService;
            _projectService = projectService;
            _env = env;
            _departmentService = departmentService;
            _domainTypeService = domainTypeService;
            _technologyParentService = technologyParentService;
            _technologyService = technologyService;
            _technologyParentMappingService = technologyParentMappingService;
            _currencyService = currencyService;
            _estimateService = estimateService;
        }
        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult Index()
        {
            var model = new CVBuilderIndexDto();

            List<UserLogin> userList = new List<UserLogin>();
            if (CurrentUser.RoleId == (int)Enums.UserRoles.UKPM)
            {
                userList = _userLoginService.GetAllDotsquaresDevelopers();
            }
            else
            {
                userList = _userLoginService.GetUsersByPM(PMUserId);
            }

            model.UserList = userList.Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString() })
                                    .ToList();
            if (_cVBuilderService.cvBuilderFindByUserId(CurrentUser.Uid) != null && _cVBuilderService.cvBuilderFindByUserId(CurrentUser.Uid).Id != 0)
            {
                model.IsCVExists = false;
            }
            else
            {
                model.IsCVExists = true;
            }
            ViewBag.Users = GetEmployees(false);
            ViewBag.PM = _userLoginService.GetPMAndPMOHRDirectorUsers(true).Select(p => new SelectListItem { Text = p.Name, Value = p.Uid.ToString() }).ToList();
            ViewBag.DepartmentList = _departmentService.GetDepartments().Select(x => new SelectListItem { Text = x.Name, Value = x.DeptId.ToString() }).ToList();
            ViewBag.ExperienceTypeList = _userLoginService.GetAllExperienceTypeList().Select(x => new SelectListItem { Text = x.Experience, Value = x.Id.ToString() }).ToList();
            ViewBag.DomainTypeList = _domainTypeService.GetDomainList().OrderBy(x => x.DomainName).Select(n => new SelectListItem { Text = n.DomainName, Value = n.DomainId.ToString() }).ToList();
            ViewBag.TechnologyList = _technologyService.GetTechnologyList().OrderBy(x => x.Title).Select(n => new SelectListItem { Text = n.Title, Value = n.TechId.ToString() }).ToList();
            ViewBag.SpecTypeList = WebExtensions.GetList<Enums.TechnologySpecializationType>().Where(x => x.Id != 4).Select(n => new SelectListItem { Text = n.Text, Value = n.Value.ToString() }).ToList();
            List<SelectListItem> statusItems = new List<SelectListItem>();
            statusItems.Add(new SelectListItem { Text = "Free", Value = "1" });
            statusItems.Add(new SelectListItem { Text = "All", Value = "2" });
            ViewBag.EmpStatusTypeList = statusItems.ToList();
            return View(model);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult Index(IDataTablesRequest request, CVBuilderSearchFilter searchFilter)
        {
            //var pagingServices = new PagingService<Cvbuilder>(request.Start, request.Length);

            CVSearchRequest cVSearchRequest = new CVSearchRequest();

            if (searchFilter.pm > 0)
            {
                cVSearchRequest.pm = searchFilter.pm;
                cVSearchRequest.IsPm = 1;
            }
            if (searchFilter.Domains != null && searchFilter.Domains.Length > 0)
            {
                cVSearchRequest.Domains = "," + string.Join(", ", searchFilter.Domains.Select(x => x)) + ",";
            }
            if (searchFilter.ExperienceType != null && searchFilter.ExperienceType.Length > 0)
            {
                cVSearchRequest.ExperienceType = "," + string.Join(", ", searchFilter.ExperienceType.Select(x => x)) + ",";
            }
            if (searchFilter.TrainingCheck)
            {
                cVSearchRequest.TrainingCheck = searchFilter.TrainingCheck;
            }
            if (searchFilter.PMReviewCheck)
            {
                cVSearchRequest.PMReviewCheck = searchFilter.PMReviewCheck;
            }
            if (RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId) || SiteKey.AshishTeamPMUId == CurrentUser.Uid || IsPMEvent || (int)Enums.UserRoles.Director == CurrentUser.RoleId)
            {
                if (searchFilter.EmpStatusTypeCheck)
                {
                    List<int> lstProjecId = new List<int>();
                    if (RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId)|| (int)Enums.UserRoles.Director == CurrentUser.RoleId)
                    {
                        if (searchFilter.pm > 0)
                        {
                            var ProjectList = _projectService.GetProjectListByPmuid(searchFilter.pm.Value).Select(x => x.ProjectId).ToList();
                            lstProjecId = _projectService.BucketProjectsByPM(searchFilter.pm.Value, ProjectList);
                        }
                        else
                        {
                            var ProjectList = _projectService.GetProjectList().Select(x => x.ProjectId).ToList();
                            lstProjecId = _projectService.BucketProjects(ProjectList);
                        }
                    }
                    else
                    {
                        var ProjectList = _projectService.GetProjectListByPmuid(PMUserId).Select(x => x.ProjectId).ToList();
                        lstProjecId = _projectService.BucketProjectsByPM(PMUserId, ProjectList);
                    }
                    lstProjecId.Add(2215);
                    cVSearchRequest.BucketProjectList = "," + string.Join(", ", lstProjecId.Select(x => x)) + ",";
                    cVSearchRequest.EmpStatusTypeCheck = searchFilter.EmpStatusTypeCheck;
                }
            }
            string spaction = string.Empty;
            if (!(RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId)) && !((int)Enums.UserRoles.Director == CurrentUser.RoleId))
            {
                if (SiteKey.AshishTeamPMUId == CurrentUser.Uid || IsPMEvent)
                {
                    cVSearchRequest.pm = CurrentUser.Uid;
                    cVSearchRequest.UserId = CurrentUser.Uid;
                    cVSearchRequest.IsPm = 1;
                }
                else
                {
                    spaction = "isUser";
                    cVSearchRequest.pm = CurrentUser.PMUid;
                    cVSearchRequest.UserId = CurrentUser.Uid;
                    cVSearchRequest.IsPm = 0;
                }
            }
            if (searchFilter.Technologies != null && searchFilter.Technologies.Length > 0)
            {
                cVSearchRequest.Technologies = "," + string.Join(", ", searchFilter.Technologies.Select(x => x)) + ",";
                if (searchFilter.TechnologyrdAnd)
                {
                    cVSearchRequest.TechnologyrdAnd = searchFilter.TechnologyrdAnd;
                }
            }
            if (searchFilter.SpecType > 0)
            {
                cVSearchRequest.SpecType = searchFilter.SpecType.Value;
            }


            int totalCount = 0;
            var isPMUser = IsPMEvent;

            var response = _cVBuilderService.GetCVBuilderDatasp(out totalCount, cVSearchRequest, spaction);

            if (searchFilter.Domains != null && searchFilter.Domains.Any())
            {
                response = response.FindAll(x => x.DomainId.Any() && searchFilter.Domains.Any(t => x.DomainId.ToArray().Contains(t)));
            }

            var keybytes = Encryption.GetRiJndael_KeyBytes(SiteKey.Encryption_Key);
            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                rowIndex = (index + 1) + (request.Start),
                Id = r.Id,
                IsEdit = r.UserId == CurrentUser.Uid || IsPM ? 1 : 0,
                Name = r.Name,
                Email = r.EmailOffice,
                Phone = r.MobileNumber,
                ExperienceType = r.Experience,
                Industry = r.DomainName,
                Technology = !string.IsNullOrEmpty(r.TechTitle) ? (r.TechTitle.Replace("&lt;", "<").Replace("&gt;", ">")) : string.Empty,
                IsApproved = r.IsTraining ? 1 : 0,
                PMApproved = r.IsApproved ? 1 : 0,
                IsPM = RoleValidator.AllHR_DesignationIds.Contains(CurrentUser.DesignationId) || (int)Enums.UserRoles.Director == CurrentUser.RoleId || CurrentUser.Uid == SiteKey.AshishTeamPMUId ? 1 : 0,
                EstimateRate = CalculationResult(r.UserId, r.ExperienceId, r.RoleId),
                EncryptId = Convert.ToBase64String(Encryption.EncryptStringToBytes(r.Id.ToString(), keybytes, keybytes)).Replace("/", "@@")
            }));
        }

        [HttpGet]
        public ActionResult Add(string id, string EncryptUid)
        {
            var keybytes = Encryption.GetRiJndael_KeyBytes(SiteKey.Encryption_Key);
            int Uid_User = 0;
            if (!string.IsNullOrWhiteSpace(EncryptUid))
            {
                EncryptUid = EncryptUid.Replace(" ", "+").Replace("@@", "/");

                //*********** GetRiJndael Decrypt**************Start/// 
                byte[] TempEncrypted = Encoding.UTF8.GetBytes(EncryptUid);
                byte[] TempmyRijndaelKey2 = Encoding.UTF8.GetBytes(Convert.ToBase64String(keybytes));
                byte[] myRijndaelKey = Convert.FromBase64String(Encoding.ASCII.GetString(TempmyRijndaelKey2));
                try
                {
                    byte[] encrypted = Convert.FromBase64String(Encoding.ASCII.GetString(TempEncrypted));
                    EncryptUid = Encryption.DecryptStringFromBytes(encrypted, myRijndaelKey, myRijndaelKey);
                    Uid_User = Convert.ToInt32(EncryptUid);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("error404", "error");
                }
            }
            if (Uid_User > 0)
            {
                var entity = _cVBuilderService.cvBuilderFindByUserId(Uid_User);
                if (entity != null && entity.Id > 0)
                {                    
                    id = Convert.ToBase64String(Encryption.EncryptStringToBytes(entity.Id.ToString(), keybytes, keybytes)).Replace("/", "@@");
                }
            }
            int? ids = 0;
            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Replace(" ", "+").Replace("@@", "/");

                //*********** GetRiJndael Decrypt**************Start/// 
                byte[] TempEncrypted = Encoding.UTF8.GetBytes(id);
                byte[] TempmyRijndaelKey2 = Encoding.UTF8.GetBytes(Convert.ToBase64String(keybytes));
                byte[] myRijndaelKey = Convert.FromBase64String(Encoding.ASCII.GetString(TempmyRijndaelKey2));
                try
                {
                    byte[] encrypted = Convert.FromBase64String(Encoding.ASCII.GetString(TempEncrypted));
                    id = Encryption.DecryptStringFromBytes(encrypted, myRijndaelKey, myRijndaelKey);
                    ids = Convert.ToInt32(id);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("error404", "error");
                }
            }
            CVBuilderDto model = new CVBuilderDto();
            model.Uid_User = RoleValidator.BA_RoleIds.Contains(CurrentUser.RoleId) ? CurrentUser.Uid : 0;
            model.TechnologyList = _technologyService.GetTechnologyList().OrderBy(x => x.Title)
                                   .Select(x => new UserTechnologyDto { TechId = x.TechId, TechName = x.Title }).ToList();
            model.DomainExpert = _domainTypeService.GetDomainList().OrderBy(x => x.DomainName)
                                   .Select(x => new DomainExpertDto { DomainId = x.DomainId, DomainName = x.DomainName }).ToList();
            model.SpecTypeList = WebExtensions.GetList<Enums.TechnologySpecializationType>();
            ViewBag.levels = WebExtensions.GetList<Enums.TechnologySpecializationType>().Where(x => x.Id != 4).OrderByDescending(o => o.Id);

            List<UserLogin> userList = new List<UserLogin>();
            if (CurrentUser.RoleId == (int)Enums.UserRoles.UKPM)
            {
                userList = _userLoginService.GetAllDotsquaresDevelopers();
            }
            else
            {
                userList = _userLoginService.GetUsersByPM(PMUserId);
            }
            //model.UserList = userList.Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString() }).OrderBy(s => s.Text).ToList();
            model.UserList = userList.Select(x => new SelectListItem { Text = x.Name, Value = Convert.ToBase64String(Encryption.EncryptStringToBytes(x.Uid.ToString(), keybytes, keybytes)).Replace("/", "@@")}).OrderBy(s => s.Text).ToList();
            DateTime now = DateTime.Today;
            List<SelectListItem> YearList = new List<SelectListItem>();
            List<SelectListItem> _fYearList = new List<SelectListItem>();
            for (int i = 0; i < 70; i++)
            {
                string _year = now.Year.ToString();
                YearList.Add(new SelectListItem { Text = _year, Value = _year });
                _fYearList.Add(new SelectListItem { Text = _year, Value = _year });
                now = now.AddYears(-1);
            }
            _fYearList.Insert(0, new SelectListItem() { Text = "Select", Value = "Select" });
            YearList.Insert(0, new SelectListItem() { Text = "Present", Value = "Present" });
            ViewBag.Years = YearList.ToList();
            ViewBag.fYears = _fYearList.ToList();

            ViewBag.Months = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames.TakeWhile(m => m != String.Empty)
                        .Select((m, i) => new SelectListItem
                        {
                            Text = m.Substring(0, 3),
                            Value = Convert.ToString(Convert.ToInt32(i) + 1)
                        }).ToList();
            ViewBag.ExperienceTypeList = _userLoginService.GetAllExperienceTypeList().Select(x => new SelectListItem { Text = x.Experience, Value = x.Id.ToString() }).ToList();
            if (ids > 0)
            {
                var entity = _cVBuilderService.cvBuilderFindById(ids.Value);
                if (entity != null)
                {
                    model.Id = entity.Id;
                    model.Title = entity.Title;
                    model.Uid_User = entity.UserId;
                    model.ProfileSummary = entity.ProfileSummary;
                    model.TechnicalSkills = entity.TechnicalSkills;
                    model.WorkExperience = entity.WorkExperience;
                    model.RolesAcross = entity.RolesAcross;
                    model.Linkedin = entity.LinkedinId;
                    model.Languages = entity.Languages;
                    model.ExperienceType = entity.ExperienceId;
                    model.OtherIndustry = entity.OtherIndustry;
                    model.OtherTechnology = entity.OtherTechnology;
                    model.OtherTechnologyParent = entity.OtherTechnologyParent;
                    model.IsAgree = entity.IsAgree.Value;
                    if (entity.CvbuilderCoreCompetencies != null)
                    {
                        model.dataList = entity.CvbuilderCoreCompetencies.OrderBy(x => x.Id)
                           .Select(s => new CVBuilderData
                           {
                               Id = s.Id,
                               Title = s.Title,
                               KRAOrderno = s.LevelId.ToString()
                           }).ToList();
                    }
                    if (entity.CvbuilderCertifications != null)
                    {
                        model.Certifications = entity.CvbuilderCertifications.OrderBy(x => x.Id)
                           .Select(s => new CVBuilderCertificationsData
                           {
                               Id = s.Id,
                               Title = s.CertificationName,
                               CertificationsNumber = s.CertificationNumber,
                               CertificationsURL = s.CertificationsImage
                           }).ToList();
                    }
                    if (entity.CvBuilderEducation != null)
                    {
                        model.Education = entity.CvBuilderEducation.OrderBy(x => x.Id)
                           .Select(s => new EducationData
                           {
                               Title = s.Title,
                               University = s.University
                           }).ToList();
                    }
                    if (entity.CvbuilderPreviousExperience != null)
                    {
                        model.PreviousExperience = entity.CvbuilderPreviousExperience.OrderBy(x => x.Id)
                           .Select(s => new PreviousExperienceData
                           {
                               OrganizationName = s.OrganizationName,
                               Designation = s.Designation,
                               FromMonth = s.FromDate != "Select" ? s.FromDate.Split(" ")[0] : "1",
                               FromDate = s.FromDate != "Select" ? s.FromDate.Split(" ")[1] : s.FromDate,
                               ToMonth = s.ToDate != "Present" ? s.ToDate.Split(" ")[0] : "1",
                               ToDate = s.ToDate != "Present" ? s.ToDate.Split(" ")[1] : s.ToDate,
                           }).ToList();
                    }
                    if (entity.CvbuilderIndustry != null)
                    {
                        model.Industries = _domainTypeService.GetDomainList().
                           Select(n => new SelectListItem
                           {
                               Text = n.DomainName,
                               Value = n.DomainId.ToString(),
                               Selected = entity.CvbuilderIndustry.Any(x => x.IndustryId == n.DomainId)
                           }).ToList();
                    }
                    else
                    {
                        model.Industries = _domainTypeService.GetDomainList().Select(n => new SelectListItem { Text = n.DomainName, Value = n.DomainId.ToString() }).ToList();
                    }
                }
                else
                {
                    if (Uid_User > 0)
                    {
                        model.Uid_User = Uid_User;
                    }
                }
            }
            else
            {
                model.Industries = _domainTypeService.GetDomainList().Select(n => new SelectListItem { Text = n.DomainName, Value = n.DomainId.ToString() }).ToList();
                if (Uid_User > 0)
                {
                    model.Uid_User = Uid_User;
                }
            }
            int userId = 0;
            if (model.Uid_User > 0)
            {
                userId = model.Uid_User;
            }
            else
            {
                userId = CurrentUser.Uid;
            }
            UserLogin UserModel = new UserLogin();
            UserModel = _userLoginService.GetUserInfoByID(userId);
            if (UserModel.User_Tech.Any())
            {
                model.TechnologyList.ForEach(t =>
                {
                    var usrTech = UserModel.User_Tech.FirstOrDefault(ut => ut.TechId == t.TechId);
                    if (usrTech != null)
                    {
                        t.Selected = true;
                        t.SpecTypeId = usrTech.SpecTypeId;
                    }
                });
            }
            if (UserModel.DomainExperts.Any())
            {
                model.DomainExpert.ForEach(t =>
                {
                    var userDomain = UserModel.DomainExperts.FirstOrDefault(ut => ut.DomainId == t.DomainId);
                    if (userDomain != null)
                    {
                        t.Selected = true;
                        t.DomainId = userDomain.DomainId;
                    }
                });
            }
            else
            {
                model.DomainExpert.ForEach(t =>
                {
                    var userDomain = UserModel.DomainExperts.FirstOrDefault(ut => ut.DomainId == t.DomainId);
                    if (userDomain != null)
                    {
                        t.Selected = false;
                        t.DomainId = userDomain.DomainId;
                    }
                });
            }
            if (UserModel != null)
            {
                model.ProfilePicture = UserModel.ProfilePicture;
            }
            model.UserName = CurrentUser.Name;
            model.Designation = CurrentUser.DesignationName;
            model.Email = CurrentUser.EmailOffice;
            model.Phone = CurrentUser.MobileNumber;
            if (IsPM)
            {
                if (model.Uid_User > 0)
                {
                    var users = _userLoginService.GetUsersById(model.Uid_User);
                    model.UserName = users.Name;
                    model.Designation = users.Designation.Name;
                    model.Email = users.EmailOffice;
                    model.Phone = users.MobileNumber;
                    model.ProfilePicture = users.ProfilePicture;
                }
                else
                {
                    model.Uid_User = CurrentUser.Uid;
                }
            }
            if (model.Uid_User > 0)
            {
                model.EncryptUid = Convert.ToBase64String(Encryption.EncryptStringToBytes(model.Uid_User.ToString(), keybytes, keybytes)).Replace("/", "@@");
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult SaveRecords(CVBuilderDto cvDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    cvDto.dataList = JsonConvert.DeserializeObject<List<CVBuilderData>>(cvDto.dataListJson);
                    cvDto.Education = JsonConvert.DeserializeObject<List<EducationData>>(cvDto.EducationJson);
                    cvDto.Certifications = JsonConvert.DeserializeObject<List<CVBuilderCertificationsData>>(cvDto.CertificationsJson);
                    cvDto.PreviousExperience = JsonConvert.DeserializeObject<List<PreviousExperienceData>>(cvDto.PreviousExperienceJson);
                    cvDto.Technology = JsonConvert.DeserializeObject<List<CVTechData>>(cvDto.TechnologyJson);
                    //cvDto.Industry = JsonConvert.DeserializeObject<List<int>>(cvDto.IndustryJson);
                    cvDto.DomainExpert = JsonConvert.DeserializeObject<List<DomainExpertDto>>(cvDto.UserDomainJson);
                    string selectedProfileName;
                    string uniqueFileName = UploadedFile(cvDto.ProfileImage, cvDto.ProfilePicture, out selectedProfileName);
                    if (cvDto != null)
                    {
                        if (cvDto.Uid_User == 0)
                        {
                            cvDto.Uid_User = CurrentUser.Uid;
                        }
                        _cVBuilderService.Save(cvDto, ContextProvider.HostEnvironment.WebRootPath);
                        if (uniqueFileName != null)
                        {
                            UserLogin UserModel = new UserLogin();
                            UserModel = _userLoginService.GetUserInfoByID(cvDto.Uid_User);
                            if (UserModel != null)
                            {
                                UserModel.ProfilePicture = uniqueFileName ?? UserModel.ProfilePicture;
                                _userLoginService.Save(UserModel);
                            }
                        }
                        if (cvDto.Technology != null && cvDto.Technology.Count() > 0)
                        {
                            foreach (var item in cvDto.Technology)
                            {
                                User_Tech obj = new User_Tech();
                                obj = _userLoginService.GetUserTechByUidandTechId(cvDto.Uid_User, item.TechId);
                                if (obj != null)
                                {
                                    obj.TechId = item.TechId;
                                    obj.SpecTypeId = item.SpecTypeId;
                                    obj.Uid = cvDto.Uid_User;
                                }
                                else
                                {
                                    obj = new User_Tech();
                                    obj.TechId = item.TechId;
                                    obj.SpecTypeId = item.SpecTypeId;
                                    obj.Uid = cvDto.Uid_User;
                                }
                                _userLoginService.SaveUserTech(obj);
                            }
                            var userTechData = _userLoginService.GetUserTechByUid(cvDto.Uid_User);
                            if (userTechData != null)
                            {
                                List<User_Tech> lstuserTech = new List<User_Tech>();
                                foreach (var item in userTechData)
                                {
                                    var isExists = cvDto.Technology.Where(x => x.TechId == item.TechId).FirstOrDefault();
                                    if (isExists == null)
                                    {
                                        lstuserTech.Add(new User_Tech { Id = item.Id, Uid = item.Uid, TechId = item.TechId, SpecTypeId = item.SpecTypeId });
                                    }
                                }
                                if (lstuserTech != null && lstuserTech.Count > 0)
                                {
                                    _userLoginService.UserTech_Deleted(lstuserTech);
                                }
                            }
                        }


                        if (cvDto.DomainExpert != null && cvDto.DomainExpert.Count() > 0)
                        {
                            foreach (var item in cvDto.DomainExpert)
                            {
                                DomainExperts obj = new DomainExperts();
                                obj = _userLoginService.GetDomainExpertsByUid(cvDto.Uid_User, item.DomainId);
                                if (obj != null)
                                {
                                    obj.DomainId = item.DomainId;
                                    obj.Uid = cvDto.Uid_User;
                                    _userLoginService.SaveDomainExperts(obj, "Updated");
                                }
                                else
                                {
                                    obj = new DomainExperts();
                                    obj.DomainId = item.DomainId;
                                    obj.Uid = cvDto.Uid_User;
                                    _userLoginService.SaveDomainExperts(obj, "");
                                }                                
                            }
                            var DomainExpertData = _userLoginService.GetDomainExpertsByUid(cvDto.Uid_User);
                            if (DomainExpertData != null)
                            {
                                List<DomainExperts> lstDomainExpert = new List<DomainExperts>();
                                foreach (var item in DomainExpertData)
                                {
                                    var isExists = cvDto.DomainExpert.Where(x => x.DomainId == item.DomainId).FirstOrDefault();
                                    if (isExists == null)
                                    {
                                        lstDomainExpert.Add(new DomainExperts { DomainId = item.DomainId, Uid = item.Uid });
                                    }
                                }
                                if (lstDomainExpert != null && lstDomainExpert.Count > 0)
                                {
                                    _userLoginService.DomainExperts_Deleted(lstDomainExpert);
                                   
                                }
                            }                            
                        }
                    }
                    return Json("Record saved successfully.");
                }
            }
            catch (Exception ex)
            {
                string message = ex.GetBaseException().Message;
                ModelState.AddModelError("Error!", message);
            }
            return CreateModelStateErrors();
        }
        private string UploadedFile(IFormFile ProfileImage, string ProfilePicture, out string selectedFileName)
        {
            string uniqueFileName = selectedFileName = null;
            if (ProfileImage != null)
            {
                var extension = System.IO.Path.GetExtension(ProfileImage.FileName);
                if (extension.ToLower() == ".jpeg" || extension.ToLower() == ".jpg" || extension.ToLower() == ".png")
                {
                    string uploadsFolder = Path.Combine(ContextProvider.HostEnvironment.WebRootPath, $"Images//profile");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    //prev. file
                    string prevpath = Path.Combine(uploadsFolder, ProfilePicture ?? "");
                    if (System.IO.File.Exists(prevpath))
                    {
                        System.IO.File.Delete(prevpath);
                    }
                    //new file
                    selectedFileName = ProfileImage.FileName;
                    uniqueFileName = ProfileImage.FileName.ToUnique();
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        ProfileImage.CopyTo(fileStream);
                    }
                }
            }
            return uniqueFileName;
        }
        [HttpGet]
        public ActionResult Print(string id, int tpId)
        {
            int? ids = 0;
            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Replace(" ", "+").Replace("@@", "/");

                //*********** GetRiJndael Decrypt**************Start/// 
                var keybytes = Encryption.GetRiJndael_KeyBytes(SiteKey.Encryption_Key);

                byte[] TempEncrypted = Encoding.UTF8.GetBytes(id);
                byte[] TempmyRijndaelKey2 = Encoding.UTF8.GetBytes(Convert.ToBase64String(keybytes));
                byte[] myRijndaelKey = Convert.FromBase64String(Encoding.ASCII.GetString(TempmyRijndaelKey2));
                try
                {
                    byte[] encrypted = Convert.FromBase64String(Encoding.ASCII.GetString(TempEncrypted));
                    id = Encryption.DecryptStringFromBytes(encrypted, myRijndaelKey, myRijndaelKey);
                    ids = Convert.ToInt32(id);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("error404", "error");
                }
            }
            int templateId = tpId;
            PrintCVDto printCVDto = new PrintCVDto();
            if (ids > 0)
            {
                var entity = _cVBuilderService.cvBuilderFindById(ids.Value);
                if (entity != null)
                {
                    if (templateId == 2)
                    {
                        printCVDto = CVTemplate2(entity);
                    }
                    else
                    {
                        printCVDto = CVTemplate1(entity);
                    }
                }                
            }            

            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();


            // set converter options
            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            converter.Options.WebPageWidth = 1024;
            converter.Options.WebPageHeight = 0;
            converter.Options.MarginTop = 20;
            converter.Options.MarginBottom = 0;
            converter.Options.WebPageFixedSize = false;
            // footer settings
            converter.Options.DisplayFooter = true;
            converter.Footer.DisplayOnFirstPage = true;
            converter.Footer.DisplayOnOddPages = true;
            converter.Footer.DisplayOnEvenPages = true;
            //converter.Footer.Height = 50;
            PdfHtmlSection footerHtml = new PdfHtmlSection(
             "<div><table style='margin-left: auto;margin-right: auto;'><tr><td><img src='" + SiteKey.DomainName + "Content/images/dots-logon.png' alt='' style='height: 15px; width: 15px;'></td>" +
             "<td style='font-size:12px;font-family: Arial, Helvetica, sans-serif;'> © Copyright 2023 Dotsquares Technologies (I) Pvt. Ltd. All Rights Reserved.</td></tr></table>" +
             "</div>",
             string.Empty);
            converter.Footer.Add(footerHtml);


            // create a new pdf document converting an url
            PdfDocument doc = converter.ConvertHtmlString(printCVDto.htmlString);

            // save pdf document
            byte[] pdf = doc.Save();
            // close pdf document
            doc.Close();

            // return resulted pdf document
            FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            fileResult.FileDownloadName = printCVDto.Username + "_CV.pdf";
            return fileResult;

        }

        private List<SelectListItem> GetEmployees(bool selectDefault = true)
        {
            var EmployeeList = _userLoginService.GetUsersListByAllDesignation(CurrentUser.Uid, CurrentUser.RoleId, CurrentUser.DesignationId);
            var selectEmpList = EmployeeList.Select(x => new SelectListItem { Text = x.Name.ToString(), Value = x.Uid.ToString(), Selected = selectDefault ? (x.Uid == EmployeeList.FirstOrDefault().Uid ? true : false) : false }).ToList();
            if (selectDefault)
                selectEmpList.Insert(0, new SelectListItem() { Text = "-Select-", Value = "" });
            return selectEmpList;
        }
        [HttpPost]
        public ActionResult GetEmployeesByPM(int pmid)
        {
            var users = (pmid > 0 ? _userLoginService.GetUsersByPM(pmid) : _userLoginService.GetUsers(true)).Select(u => new { text = u.EmpCode != null ? u.Name.ToString() + " [" + u.EmpCode + "]" : u.Name, value = u.Uid });
            return Json(users);
        }
        [HttpPost]
        public ActionResult GetEmployeesByDepartment(int pmid, int departmentId)
        {
            if (pmid == 0)
            {
                if ((int)Enums.UserRoles.PM == CurrentUser.RoleId)
                {
                    pmid = CurrentUser.Uid;
                }
            }
            if (departmentId > 0)
            {
                var users = (departmentId > 0 ? _userLoginService.GetUsersByDepartment(pmid, departmentId) : _userLoginService.GetUsers(true)).Select(u => new { text = u.EmpCode != null ? u.Name.ToString() + " [" + u.EmpCode + "]" : u.Name, value = u.Uid });
                return Json(users);
            }
            else
            {
                var users = (pmid > 0 ? _userLoginService.GetUsersByPM(pmid) : _userLoginService.GetUsers(true)).Select(u => new { text = u.EmpCode != null ? u.Name.ToString() + " [" + u.EmpCode + "]" : u.Name, value = u.Uid });
                return Json(users);
            }
        }
        [CustomActionAuthorization]
        public ActionResult View(string id,int tpId)
        {
            int? ids = 0;
            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Replace(" ", "+").Replace("@@", "/");

                //*********** GetRiJndael Decrypt**************Start/// 
                var keybytes = Encryption.GetRiJndael_KeyBytes(SiteKey.Encryption_Key);

                byte[] TempEncrypted = Encoding.UTF8.GetBytes(id);
                byte[] TempmyRijndaelKey2 = Encoding.UTF8.GetBytes(Convert.ToBase64String(keybytes));
                byte[] myRijndaelKey = Convert.FromBase64String(Encoding.ASCII.GetString(TempmyRijndaelKey2));
                try
                {
                    byte[] encrypted = Convert.FromBase64String(Encoding.ASCII.GetString(TempEncrypted));
                    id = Encryption.DecryptStringFromBytes(encrypted, myRijndaelKey, myRijndaelKey);
                    ids = Convert.ToInt32(id);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("error404", "error");
                }
            }
            int templateId = tpId;
            CVViewDto model = new CVViewDto();
            if (ids > 0)
            {
                var entity = _cVBuilderService.cvBuilderFindById(ids.Value);
                if (entity != null)
                {
                    if (templateId == 2) 
                    {
                        model = ViewCVTemplate2(entity);
                        return PartialView("_DisplayTemplate2", model);
                    }
                    else
                    {
                        model = ViewCVTemplate1(entity);
                        return PartialView("_DisplayClientCVData", model);
                    }
                }
            }
            return PartialView("_DisplayClientCVData", model);
        }
        [HttpPost]

        [HttpPost]
        public ActionResult GetYearsMonthsList()
        {
            MonthYear monthYear = new MonthYear();
            DateTime now = DateTime.Today;
            List<SelectListItem> YearList = new List<SelectListItem>();
            List<SelectListItem> _fYearList = new List<SelectListItem>();
            for (int i = 0; i < 70; i++)
            {
                string _year = now.Year.ToString();
                YearList.Add(new SelectListItem { Text = _year, Value = _year });
                _fYearList.Add(new SelectListItem { Text = _year, Value = _year });
                now = now.AddYears(-1);
            }
            _fYearList.Insert(0, new SelectListItem() { Text = "Select", Value = "Select" });
            YearList.Insert(0, new SelectListItem() { Text = "Present", Value = "Present" });
            monthYear.YearsList = YearList;
            monthYear.FYearsList = _fYearList;
            monthYear.MonthsList = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames.TakeWhile(m => m != String.Empty)
                        .Select((m, i) => new SelectListItem
                        {
                            Text = m.Substring(0, 3),
                            Value = Convert.ToString(Convert.ToInt32(i) + 1)
                        }).ToList();
            return Json(monthYear);
        }

        [HttpPost]
        public FileResult Download([FromBody] CVBuilderSearchFilter searchFilter)
        {
            string fileSavePath = Path.Combine(_env.WebRootPath, "Upload", "CV");
            CVSearchRequest cVSearchRequest = new CVSearchRequest();

            if (searchFilter.pm > 0)
            {
                cVSearchRequest.pm = searchFilter.pm;
                cVSearchRequest.IsPm = 1;
            }
            if (searchFilter.Domains != null && searchFilter.Domains.Length > 0)
            {
                cVSearchRequest.Domains = "," + string.Join(", ", searchFilter.Domains.Select(x => x)) + ",";
            }
            if (searchFilter.ExperienceType != null && searchFilter.ExperienceType.Length > 0)
            {
                cVSearchRequest.ExperienceType = "," + string.Join(", ", searchFilter.ExperienceType.Select(x => x)) + ",";
            }
            if (searchFilter.TrainingCheck)
            {
                cVSearchRequest.TrainingCheck = searchFilter.TrainingCheck;
            }
            if (searchFilter.PMReviewCheck)
            {
                cVSearchRequest.PMReviewCheck = searchFilter.PMReviewCheck;
            }
            if (RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId) || SiteKey.AshishTeamPMUId == CurrentUser.Uid || IsPMEvent || (int)Enums.UserRoles.Director == CurrentUser.RoleId)
            {
                if (searchFilter.EmpStatusTypeCheck)
                {
                    List<int> lstProjecId = new List<int>();
                    if (RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId) || (int)Enums.UserRoles.Director == CurrentUser.RoleId)
                    {
                        if (searchFilter.pm > 0)
                        {
                            var ProjectList = _projectService.GetProjectListByPmuid(searchFilter.pm.Value).Select(x => x.ProjectId).ToList();
                            lstProjecId = _projectService.BucketProjectsByPM(searchFilter.pm.Value, ProjectList);
                        }
                        else
                        {
                            var ProjectList = _projectService.GetProjectList().Select(x => x.ProjectId).ToList();
                            lstProjecId = _projectService.BucketProjects(ProjectList);
                        }
                    }
                    else
                    {
                        var ProjectList = _projectService.GetProjectListByPmuid(PMUserId).Select(x => x.ProjectId).ToList();
                        lstProjecId = _projectService.BucketProjectsByPM(PMUserId, ProjectList);
                    }
                    lstProjecId.Add(2215);
                    cVSearchRequest.BucketProjectList = "," + string.Join(", ", lstProjecId.Select(x => x)) + ",";
                    cVSearchRequest.EmpStatusTypeCheck = searchFilter.EmpStatusTypeCheck;
                }
            }
            string spaction = string.Empty;
            if (!(RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId)) && !((int)Enums.UserRoles.Director == CurrentUser.RoleId))
            {
                if (SiteKey.AshishTeamPMUId == CurrentUser.Uid || IsPMEvent)
                {
                    cVSearchRequest.pm = CurrentUser.Uid;
                    cVSearchRequest.UserId = CurrentUser.Uid;
                    cVSearchRequest.IsPm = 1;
                }
                else
                {
                    spaction = "isUser";
                    cVSearchRequest.pm = CurrentUser.PMUid;
                    cVSearchRequest.UserId = CurrentUser.Uid;
                    cVSearchRequest.IsPm = 0;
                }
            }
            if (searchFilter.Technologies != null && searchFilter.Technologies.Length > 0)
            {
                cVSearchRequest.Technologies = "," + string.Join(", ", searchFilter.Technologies.Select(x => x)) + ",";
                if (searchFilter.TechnologyrdAnd)
                {
                    cVSearchRequest.TechnologyrdAnd = searchFilter.TechnologyrdAnd;
                }
            }
            if (searchFilter.SpecType > 0)
            {
                cVSearchRequest.SpecType = searchFilter.SpecType.Value;
            }


            int totalCount = 0;
            var isPMUser = IsPMEvent;

            var response = _cVBuilderService.GetCVBuilderDatasp(out totalCount, cVSearchRequest, spaction);

            if (searchFilter.Domains != null && searchFilter.Domains.Any())
            {
                response = response.FindAll(x => x.DomainId.Any() && searchFilter.Domains.Any(t => x.DomainId.ToArray().Contains(t)));
            }

            if (response != null && response.Count() > 0)
            {
                foreach (var res in response)
                {
                    PrintCVDto printCVDto = new PrintCVDto();
                    var entity = _cVBuilderService.cvBuilderFindById(res.Id);
                    if (entity != null)
                    {
                        if (searchFilter.TemplateId == 2)
                        {

                            printCVDto = CVTemplate2(entity);
                        }
                        else
                        {
                            printCVDto = CVTemplate1(entity);
                        }
                    }  
                    // instantiate a html to pdf converter object
                    HtmlToPdf converter = new HtmlToPdf();

                    // set converter options
                    converter.Options.PdfPageSize = PdfPageSize.A4;
                    converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
                    converter.Options.WebPageWidth = 1024;
                    converter.Options.WebPageHeight = 0;
                    converter.Options.MarginTop = 20;
                    converter.Options.MarginBottom = 0;
                    converter.Options.WebPageFixedSize = false;
                    // footer settings
                    converter.Options.DisplayFooter = true;
                    converter.Footer.DisplayOnFirstPage = true;
                    converter.Footer.DisplayOnOddPages = true;
                    converter.Footer.DisplayOnEvenPages = true;
                    //converter.Footer.Height = 50;
                    PdfHtmlSection footerHtml = new PdfHtmlSection(
                     "<div><table style='margin-left: auto;margin-right: auto;'><tr><td><img src='" + SiteKey.DomainName + "Content/images/dots-logon.png' alt='' style='height: 15px; width: 15px;'></td>" +
                     "<td style='font-size:12px;font-family: Arial, Helvetica, sans-serif;'> © Copyright 2023 Dotsquares Technologies (I) Pvt. Ltd. All Rights Reserved.</td></tr></table>" +
                     "</div>",
                     string.Empty);
                    converter.Footer.Add(footerHtml);


                    // create a new pdf document converting an url
                    PdfDocument doc = converter.ConvertHtmlString(printCVDto.htmlString);

                    // save pdf document
                    doc.Save(fileSavePath + "\\" + printCVDto.Username + "_CV.pdf");
                    // close pdf document
                    doc.Close();
                }
            }

            var zipName = $"CV-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";


            using (MemoryStream ms = new MemoryStream())
            {
                //required: using System.IO.Compression;
                using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    //QUery the Products table and get all image content
                    response.ForEach(file =>
                    {
                        var _fileName = file.Name + "_CV.pdf";
                        string path = Path.Combine(fileSavePath, _fileName);
                        var entry = zip.CreateEntry(_fileName);
                        using (var fileStream = new MemoryStream(System.IO.File.ReadAllBytes(path)))
                        using (var entryStream = entry.Open())
                        {
                            fileStream.CopyTo(entryStream);
                        }
                    });
                }
                // Delete all files in a directory    
                string[] files = Directory.GetFiles(fileSavePath);
                foreach (string file in files)
                {
                    System.IO.File.Delete(file);
                }
                return File(ms.ToArray(), "application/zip", zipName);
            }
        }
        

        [HttpGet]
        public ActionResult UpdateApproved(string id)
        {
            int? ids = 0;
            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Replace(" ", "+").Replace("@@", "/");

                //*********** GetRiJndael Decrypt**************Start/// 
                var keybytes = Encryption.GetRiJndael_KeyBytes(SiteKey.Encryption_Key);

                byte[] TempEncrypted = Encoding.UTF8.GetBytes(id);
                byte[] TempmyRijndaelKey2 = Encoding.UTF8.GetBytes(Convert.ToBase64String(keybytes));
                byte[] myRijndaelKey = Convert.FromBase64String(Encoding.ASCII.GetString(TempmyRijndaelKey2));
                try
                {
                    byte[] encrypted = Convert.FromBase64String(Encoding.ASCII.GetString(TempEncrypted));
                    id = Encryption.DecryptStringFromBytes(encrypted, myRijndaelKey, myRijndaelKey);
                    ids = Convert.ToInt32(id);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("error404", "error");
                }
            }
            var entity = _cVBuilderService.cvBuilderFindById(ids.Value);
            if (entity != null)
            {
                entity.IsTraining = entity.IsTraining.Value ? false : true;
                _cVBuilderService.UpdateApprovedStatus(entity);
            }
            return Json("Change Status successfully.");
        }
        [HttpGet]
        public ActionResult ApprovedPM(string id)
        {
            int? ids = 0;
            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Replace(" ", "+").Replace("@@", "/");

                //*********** GetRiJndael Decrypt**************Start/// 
                var keybytes = Encryption.GetRiJndael_KeyBytes(SiteKey.Encryption_Key);

                byte[] TempEncrypted = Encoding.UTF8.GetBytes(id);
                byte[] TempmyRijndaelKey2 = Encoding.UTF8.GetBytes(Convert.ToBase64String(keybytes));
                byte[] myRijndaelKey = Convert.FromBase64String(Encoding.ASCII.GetString(TempmyRijndaelKey2));
                try
                {
                    byte[] encrypted = Convert.FromBase64String(Encoding.ASCII.GetString(TempEncrypted));
                    id = Encryption.DecryptStringFromBytes(encrypted, myRijndaelKey, myRijndaelKey);
                    ids = Convert.ToInt32(id);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("error404", "error");
                }
            }
            var entity = _cVBuilderService.cvBuilderFindById(ids.Value);
            if (entity != null)
            {
                entity.IsApproved = entity.IsApproved.Value ? false : true;
                _cVBuilderService.UpdateApprovedStatus(entity);
            }
            return Json("Change Status successfully.");
        }
        public string GetSpecDescription(byte? value, string title)
        {
            string span = string.Empty;
            if (value == 1)
            {
                span = "<span title=\"Expert\" class=\"round-b badge-success\"></span>";
            }
            else if (value == 2)
            {
                span = "<span title=\"Intermediate\" class=\"round-b badge-primary\"></span>";
            }
            else if (value == 3)
            {
                span = "<span title=\"Beginner\" class=\"round-b badge-warning\"></span>";
            }

            return span + " " + title;
        }
        private string CalculationResult(int UserId, int experience, int RoleId)
        {
            List<int> user_Teches = _userLoginService.GetUserTechByUid(UserId).Where(x => x.SpecTypeId != 4).Select(x1 => x1.TechId).ToList();
            var usdCurrency = _currencyService.GetCurrencyByName("USD");
            var audCurrency = _currencyService.GetCurrencyByName("AUD");
            var aedCurrency = _currencyService.GetCurrencyByName("AED");

            var totalEstimateHour = "1 Hours";
            decimal price = 0;
            List<decimal> values = new List<decimal>();
            if (RoleId == (int)Enums.UserRoles.DV)
            {
                foreach (var tech in user_Teches)
                {
                    var technologyId = _technologyService.GetTechnologyById(tech).EstimateTechnologyId;
                    if (technologyId != null)
                    {
                        price = EstimatePrice(RoleId, experience, technologyId.Value);
                    }
                    if (price > 0)
                    {
                        values.Add(price);
                    }
                }
            }
            else
            {
                price = EstimatePrice(RoleId, experience, null);
                if (price > 0)
                {
                    values.Add(price);
                }
            }
            string Rate = string.Empty;
            if (values.Count > 0)
            {
                decimal maxPrice = values.Max();
                decimal minPrice = values.Min();
                var pound = (1 * 1) * maxPrice;
                var usd = (pound * Convert.ToDecimal(usdCurrency.ExchangeRate));
                var aud = (pound * Convert.ToDecimal(audCurrency.ExchangeRate));
                var aed = Math.Truncate(Math.Truncate((pound * Convert.ToDecimal(aedCurrency.ExchangeRate))) / 10) * 10;
                Rate = "<span><b>£</b> " + pound.ToString("0.00") + "</span><br/><span><b>USD</b> " + usd.ToString("0.00") + "</span><br/><span><b>AUD</b> " + aud.ToString("0.00") + "</span><br/><span><b>AED</b> " + aed.ToString("0.00") + "</span>";
            }
            else
            {
                Rate = "<span><b>£</b> 0.00 </span><br/><span><b>USD</b> 0.00 </span><br/><span><b>AUD</b> 0.00 </span><br/><span><b>AED</b> 0.00 </span>";
            }
            return Rate;
        }
        private decimal EstimatePrice(int roleId, int estimateRoleExpId, long? technologyParentId)
        {
            decimal price = 0;            
            var data = _cVBuilderService.GetEstimateRoleTechnoloyPrice(roleId, estimateRoleExpId, technologyParentId);
            if (data != null)
            {
                price = data.Price.Value;
            }

            return price;
        }
       
        private PrintCVDto CVTemplate1(Cvbuilder entity)
        {
            CVBuilderDto model = new CVBuilderDto();
            PrintCVDto printCVDto = new PrintCVDto();
            string DateOfBirth = string.Empty;
            string UserProfile = string.Empty;
            string coreHtml = string.Empty;
            string certificationsHtml = string.Empty;
            string previousExperienceHtml = string.Empty;
            string educationHtml = string.Empty;
            if (entity != null)
            {
                model.Id = entity.Id;
                model.Title = entity.Title;
                model.Uid_User = entity.UserId;
                model.UserName = entity.User.Name;
                model.ProfileSummary = entity.ProfileSummary;
                model.TechnicalSkills = entity.TechnicalSkills;
                model.WorkExperience = entity.WorkExperience;
                model.RolesAcross = entity.RolesAcross;
                model.Linkedin = entity.LinkedinId;
                model.Languages = entity.Languages;
                var users = _userLoginService.GetUserInfoByID(entity.UserId);
                model.UserName = users.Name;
                model.Designation = users.Designation.Name;
                model.Email = users.EmailOffice;
                model.Phone = users.MobileNumber;
                model.Address = users.Address;
                DateOfBirth = users.DOB != null ? users.DOB.Value.ToString("dd-MMM-yyyy") : string.Empty;
                if (!string.IsNullOrEmpty(users.ProfilePicture))
                {
                    UserProfile = "images/profile/" + users.ProfilePicture;
                    string imagePath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath, $"Images\\profile\\" + users.ProfilePicture);
                    if (!System.IO.File.Exists(imagePath))
                    {
                        UserProfile = "PdfTemplates/images/DefaultImage.png";
                    }
                }
                else
                {
                    UserProfile = "PdfTemplates/images/DefaultImage.png";
                }

                if (entity.CvbuilderCoreCompetencies != null)
                {
                    foreach (var core in entity.CvbuilderCoreCompetencies)
                    {
                        coreHtml += "<tr>";
                        coreHtml += "<td style=\"color:#0b0c0e; font-size:16px\">" + core.Title + "</td>";
                        if (core.LevelId == 1)
                        {
                            coreHtml += "<td  style=\"vertical-align:bottom; padding-left:10px; text-align:right;\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/images/bar-99.png\" alt=\"\" /></td>";
                        }
                        else if (core.LevelId == 2)
                        {
                            coreHtml += "<td  style=\"vertical-align:bottom; padding-left:10px; text-align:right;\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/images/bar-70.png\" alt=\"\" /></td>";
                        }
                        else
                        {
                            coreHtml += "<td  style=\"vertical-align:bottom; padding-left:10px; text-align:right;\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/images/bar-30.png\" alt=\"\" /></td>";
                        }
                        //coreHtml += "<td  style=\"vertical-align:bottom; padding-left:10px; text-align:right;\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/images/bar.png\" alt=\"\" /></td>";
                        coreHtml += "</tr><tr><td style=\"color:#0b0c0e; font-size:16px\">&nbsp;</td><td>&nbsp;</td></tr>";
                    }
                }
                if (entity.CvbuilderCertifications != null)
                {
                    var remainder = entity.CvbuilderCertifications.Count % 3;
                    var total = entity.CvbuilderCertifications.Count / 3;
                    int pi = 0;
                    for (int i = 0; i < (total > 0 ? total : remainder); i++)
                    {

                        for (int j = 0; j < 3; j++)
                        {
                            bool isSpace = false;
                            int spaceCenter = 0;
                            certificationsHtml += "<tr>";
                            foreach (var crt in entity.CvbuilderCertifications.Skip(pi).Take(3))
                            {
                                if (spaceCenter > 0)
                                {
                                    certificationsHtml += "<td width=\"2%\" style=\"display:inline-block\" align=\"center\"></td>";
                                }

                                certificationsHtml += "<td style=\"display:inline-block; vertical-align:top; width:32%;\">";
                                certificationsHtml += "<table width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\"><tbody><tr>";
                                certificationsHtml += "<td style=\"background-color:#f6f6f6; border-left:5px solid #1e9dcb; padding:15px 5px;\" width=\"50\" align=\"center\">";
                                certificationsHtml += "<img src=\"" + SiteKey.DomainName + "PdfTemplates/images/certificate1.png\" style=\"max-width:40px;position:relative;z-index:1;\" alt=\"\"></td><td style=\"background-color:#f6f6f6; padding:15px 2px;\">";
                                certificationsHtml += "<div style=\"font-size:15px; color:#1e9dcb; margin-bottom:8px;position:relative;z-index:1;\">" + crt.CertificationName + "</div>";
                                if (!string.IsNullOrWhiteSpace(crt.CertificationNumber))
                                {
                                    certificationsHtml += "<div style=\"font-size:12px;font-weight:700; color:#0b0c0e\">(" + crt.CertificationNumber + ")</div></td></tr></tbody></table></td>";
                                }
                                else
                                {
                                    certificationsHtml += "<div style=\"font-size:12px;font-weight:700; color:#0b0c0e\"></div></td></tr></tbody></table></td>";
                                }
                                pi++;
                                spaceCenter++;
                                isSpace = true;
                            }
                            certificationsHtml += "</tr>";
                            if (isSpace)
                            {
                                certificationsHtml += "<tr><td>&nbsp;</td></tr>";
                            }
                        }

                    }
                }
                if (entity.CvBuilderEducation != null)
                {

                    foreach (var edu in entity.CvBuilderEducation)
                    {
                        educationHtml += "<tr>";
                        educationHtml += "<td style=\"width:35px\" valign=\"top\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/images/bullot.png\" alt=\"\" /></td>";
                        educationHtml += "<td valign=\"top\" style=\"color:#0b0c0e; font-size:16px; line-height:20px\"><strong>" + edu.Title + "<br /></strong> " + edu.University + "</td>";
                        educationHtml += "</tr>";
                        educationHtml += "<tr><td style=\"width:35px\" valign=\"top\">&nbsp;</td><td valign=\"top\" style=\"color:#0b0c0e; font-size:16px; line-height:20px\">&nbsp;</td></tr>";
                    }

                }
                if (entity.CvbuilderPreviousExperience != null)
                {
                    var remainder = entity.CvbuilderPreviousExperience.Count % 3;
                    var total = entity.CvbuilderPreviousExperience.Count / 3;
                    int pi = 0;
                    for (int i = 0; i < (total > 0 ? total : remainder); i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            bool isSpace = false;
                            previousExperienceHtml += "<tr>";
                            int icon = 0;
                            foreach (var preExp in entity.CvbuilderPreviousExperience.Skip(pi).Take(3))
                            {
                                string OrgName = string.Empty;
                                var isDotsquares = preExp.OrganizationName.ToLower().Contains("dotsquares");
                                if (isDotsquares)
                                {
                                    OrgName = preExp.OrganizationName;
                                }
                                else
                                {
                                    OrgName = "Confidential";
                                }
                                if (icon > 0)
                                {
                                    previousExperienceHtml += "<td style=\"width:4%; display:inline-block;\" align=\"center\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/images/arrow.png\" alt=\"\" /></td>";
                                }
                                previousExperienceHtml += "<td align=\"left\" style=\"background-color:#f6f6f6; border-left:5px solid #1e9dcb; padding:15px;width:27%; display:inline-block;\">";
                                previousExperienceHtml += "<div style=\"font-size:16px; color:#0b0c0e; line-height:25px\"><strong>" + preExp.FromDate + " to " + preExp.ToDate + "</strong><br />" + OrgName + "<br /><strong style='font-size: 12px;font-style: italic;'>" + preExp.Designation + "</strong></div></td>";

                                icon++;
                                pi++;
                                isSpace = true;
                            }
                            previousExperienceHtml += "</tr>";
                            if (isSpace)
                            {
                                previousExperienceHtml += "<tr><td>&nbsp;</td></tr>";
                            }
                        }

                    }

                }
            }

            StreamReader filePtr;
            string fileData = "CVBuilderHtmlNew.html";
            var webRoot = _env.WebRootPath;
            var pathToFile = _env.WebRootPath
                       + Path.DirectorySeparatorChar.ToString()
                       + "PdfTemplates"
                       + Path.DirectorySeparatorChar.ToString()
                       + "CVBuilderHtmlNew.html";
            filePtr = System.IO.File.OpenText(pathToFile);
            fileData = filePtr.ReadToEnd();
            filePtr.Close();
            filePtr = null;
            string bullotImg = SiteKey.DomainName + "PdfTemplates/images/bullot.png";
            string _ProfileSummary = string.Empty;
            string _TechnicalSkills = string.Empty;
            string _WorkExperience = string.Empty;
            if (!string.IsNullOrEmpty(model.ProfileSummary))
            {
                _ProfileSummary = model.ProfileSummary.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                        .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 30px;padding-bottom: 10px;'>");
            }
            if (!string.IsNullOrEmpty(model.TechnicalSkills))
            {
                _TechnicalSkills = model.TechnicalSkills.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                        .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 30px;padding-bottom: 10px;'>");
            }
            if (!string.IsNullOrEmpty(model.WorkExperience))
            {
                _WorkExperience = model.WorkExperience.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                       .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 30px;padding-bottom: 10px;'>");
            }
            string _RolesAcross = string.Empty;
            if (!string.IsNullOrEmpty(model.RolesAcross))
            {
                string _RolesAcrossUL = model.RolesAcross.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                            .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 30px;padding-bottom: 10px;'>");

                _RolesAcross = "<tr>" +
                    "<td style='color:#1e9dcb; font-size:23px; font-weight:700;  background:url(" + SiteKey.DomainName + "PdfTemplates/images/line2.png); background-repeat:repeat-x;  visibility: visible;'>" +
                    "<span style='background-color: #fff; display: inline-block; padding-right: 10px; position: relative; z-index: 1;'>" +
                    "<img src='" + SiteKey.DomainName + "PdfTemplates/images/icon8.png' align='left' style='margin-right:10px; margin-top:-3px' /> Roles Across The Career</span>" +
                    "</td></tr>" +
                    "<tr><td style='height:10px'></td></tr>" +
                    "<tr><td><table width='100%' border='0' cellspacing='0' cellpadding='0'><tr>" + _RolesAcrossUL + "</tr></table></td></tr>" +
                    "<tr><td style='height:25px'></td></tr>";

            }
            string certificationsData = string.Empty;
            if (!string.IsNullOrEmpty(certificationsHtml))
            {
                certificationsData = "<tr><td style='color:#1e9dcb; font-size:23px; font-weight:700;  background:url(" + SiteKey.DomainName + "PdfTemplates/images/line2.png); background-repeat:repeat-x;  visibility: visible;'>" +
                    "<span style='background-color: #fff; display: inline-block; padding-right: 10px; position: relative; z-index: 1; '>" +
                    "<img src='" + SiteKey.DomainName + "PdfTemplates/images/certificate.png' align='left' style='margin-right:10px; position:relative; top:3px' /> Certifications" +
                    "</span></td></tr><tr><td style='height:25px'></td></tr><tr><td><table width='100%' border='0' cellspacing='0' cellpadding='0'>" +
                    "" + certificationsHtml + "</table></td></tr><tr><td style='height:25px'></td></tr>";
            }
            string EmailDots = string.Empty;
            if (!string.IsNullOrEmpty(model.Email))
            {
                MailAddress address = new MailAddress(model.Email);
                string hostname = address.Host;
                if (hostname.ToLower() == "dotsquares.com")
                {
                    EmailDots = "<table width='100%' border='0' cellspacing='0' cellpadding='0' style='position: relative; z-index: 1;'>" +
                        "<tr><td width='35' valign='top'><img src='" + SiteKey.DomainName + "PdfTemplates/images/icon1.jpg' style='max-width: inherit;' alt='' /></td>" +
                        "<td width='15' valign='top'>&nbsp;</td><td valign='top'><div style='color:#939393; font-size: 16px; margin-bottom: 5px'>Email</div>" +
                        "<div style='color:#0b0c0e; font-size:16px;'>" + model.Email + "</div></td></tr></table>";
                }
            }
            string htmlString = fileData.Replace("@Name@", model.UserName)
                        .Replace("@Designation@", model.Designation)
                        .Replace("@DomainName@", SiteKey.DomainName)
                        .Replace("@Title@", model.Title)
                        .Replace("@Email@", EmailDots)
                        .Replace("@Languages@", model.Languages)
                        .Replace("@Linkedin@", model.Linkedin)
                        .Replace("@Phone@", model.Phone)
                        .Replace("@Location@", model.Address)
                        .Replace("@ProfileSummary@", _ProfileSummary)
                        .Replace("@TechnicalSkills@", _TechnicalSkills)
                        .Replace("@WorkExperience@", _WorkExperience)
                        .Replace("@RolesAcross@", _RolesAcross)
                        .Replace("@CoreCompetencies@", coreHtml)
                        .Replace("@Certifications@", certificationsData)
                        .Replace("@DOB@", DateOfBirth)
                        .Replace("@PreviousExperience@", previousExperienceHtml)
                        .Replace("@Education@", educationHtml)
                        .Replace("@UserProfile@", SiteKey.DomainName + UserProfile);

            printCVDto.htmlString = htmlString;
            printCVDto.Username = model.UserName;
            return printCVDto;
        }
        private PrintCVDto CVTemplate2(Cvbuilder entity)
        {
            CVBuilderDto model = new CVBuilderDto();
            PrintCVDto printCVDto = new PrintCVDto();
            string DateOfBirth = string.Empty;
            string UserProfile = string.Empty;
            string coreHtml = string.Empty;
            string certificationsHtml = string.Empty;
            string previousExperienceHtml = string.Empty;
            string CAREERTIMELINEHTML = string.Empty;
            string educationHtml = string.Empty;
            string technologyHTML = string.Empty;
            if (entity != null)
            {
                model.Id = entity.Id;
                model.Title = entity.Title;
                model.Uid_User = entity.UserId;
                model.UserName = entity.User.Name;
                model.ProfileSummary = entity.ProfileSummary;
                model.TechnicalSkills = entity.TechnicalSkills;
                model.WorkExperience = entity.WorkExperience;
                model.RolesAcross = entity.RolesAcross;
                model.Linkedin = entity.LinkedinId;
                model.Languages = entity.Languages;
                var users = _userLoginService.GetUserInfoByID(entity.UserId);
                model.UserName = users.Name;
                model.Designation = users.Designation.Name;
                model.Email = users.EmailOffice;
                model.Phone = users.MobileNumber;
                model.Address = users.Address;
                DateOfBirth = users.DOB != null ? users.DOB.Value.ToString("dd-MMM-yyyy") : string.Empty;
                if (!string.IsNullOrEmpty(users.ProfilePicture))
                {
                    UserProfile = "images/profile/" + users.ProfilePicture;
                    string imagePath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath, $"Images\\profile\\" + users.ProfilePicture);
                    if (!System.IO.File.Exists(imagePath))
                    {
                        UserProfile = "PdfTemplates/images/DefaultImage.png";
                    }
                }
                else
                {
                    UserProfile = "PdfTemplates/images/DefaultImage.png";
                }

                if (entity.CvbuilderCoreCompetencies != null)
                {
                    foreach (var core in entity.CvbuilderCoreCompetencies)
                    {
                        coreHtml += "<tr>";
                        if (core.LevelId == 1)
                        {
                            coreHtml += "<td style=\"  width: 80px; text-align: left;vertical-align: top;padding: 6px 0;\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/Template2/images/rating-1.jpg\" alt=\"icon\" /></td>";
                        }
                        else if (core.LevelId == 2)
                        {
                            coreHtml += "<td style=\"  width: 80px; text-align: left;vertical-align: top;padding: 6px 0;\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/Template2/images/rating-2.jpg\" alt=\"icon\" /></td>";
                        }
                        else
                        {
                            coreHtml += "<td style=\"  width: 80px; text-align: left;vertical-align: top;padding: 6px 0;\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/Template2/images/rating-3.jpg\" alt=\"icon\" /></td>";
                        }
                        coreHtml += "<td style=\"font-size: 16px; line-height: 25px; color: #000;padding-bottom: 5px; \">" + core.Title + "</td>";
                        coreHtml += "</tr>";
                    }
                }
                //if (entity.CvbuilderCertifications != null)
                //{
                //    var remainder = entity.CvbuilderCertifications.Count % 3;
                //    var total = entity.CvbuilderCertifications.Count / 3;
                //    int pi = 0;
                //    for (int i = 0; i < (total > 0 ? total : remainder); i++)
                //    {

                //        for (int j = 0; j < 3; j++)
                //        {
                //            bool isSpace = false;
                //            int spaceCenter = 0;
                //            certificationsHtml += "<tr>";
                //            foreach (var crt in entity.CvbuilderCertifications.Skip(pi).Take(3))
                //            {
                //                if (spaceCenter > 0)
                //                {
                //                    certificationsHtml += "<td width=\"2%\" style=\"display:inline-block\" align=\"center\"></td>";
                //                }

                //                certificationsHtml += "<td style=\"display:inline-block; vertical-align:top; width:32%;\">";
                //                certificationsHtml += "<table width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\"><tbody><tr>";
                //                certificationsHtml += "<td style=\"background-color:#f6f6f6; border-left:5px solid #1e9dcb; padding:15px 5px;\" width=\"50\" align=\"center\">";
                //                certificationsHtml += "<img src=\"" + SiteKey.DomainName + "PdfTemplates/images/certificate1.png\" style=\"max-width:40px;position:relative;z-index:1;\" alt=\"\"></td><td style=\"background-color:#f6f6f6; padding:15px 2px;\">";
                //                certificationsHtml += "<div style=\"font-size:15px; color:#1e9dcb; margin-bottom:8px;position:relative;z-index:1;\">" + crt.CertificationName + "</div>";
                //                if (!string.IsNullOrWhiteSpace(crt.CertificationNumber))
                //                {
                //                    certificationsHtml += "<div style=\"font-size:12px;font-weight:700; color:#0b0c0e\">(" + crt.CertificationNumber + ")</div></td></tr></tbody></table></td>";
                //                }
                //                else
                //                {
                //                    certificationsHtml += "<div style=\"font-size:12px;font-weight:700; color:#0b0c0e\"></div></td></tr></tbody></table></td>";
                //                }
                //                pi++;
                //                spaceCenter++;
                //                isSpace = true;
                //            }
                //            certificationsHtml += "</tr>";
                //            if (isSpace)
                //            {
                //                certificationsHtml += "<tr><td>&nbsp;</td></tr>";
                //            }
                //        }

                //    }
                //}
                if (entity.CvbuilderCertifications != null)
                {
                    bool icon = true;
                    certificationsHtml += "<td style=\"padding-top: 24px;\">";
                    foreach (var crt in entity.CvbuilderCertifications)
                    {
                        if (!string.IsNullOrEmpty(crt.CertificationsImage))
                        {
                            icon = false;
                            certificationsHtml += "<img style='margin-right: 10px; margin-bottom: 5px;height: 72px;width: 76px;' src='"+SiteKey.DomainName+"images/CertificationImage/" + crt.CertificationsImage + "' alt=''/>";
                        }
                    }
                    if (icon)
                    {
                        certificationsHtml = "";
                        certificationsHtml += "<td style=\"padding-top: 101px;\">";
                    }
                    certificationsHtml += "</td>";
                }
                else
                {
                    certificationsHtml += "<td style=\"padding-top: 101px;\"></td>";
                }
                if (entity.CvBuilderEducation != null)
                {

                    foreach (var edu in entity.CvBuilderEducation)
                    {
                        educationHtml += "<tr>";
                        educationHtml += "<td style=\"color: #fff; background-color: #7e7e7e; font-size: 16px; line-height: 20px; text-align: right; padding: 10px 15px; \">" + edu.Title + "<br />" + edu.University + "</td>";
                        educationHtml += "</tr>";
                    }

                }
                if (entity.CvbuilderPreviousExperience != null)
                {
                    var remainder = entity.CvbuilderPreviousExperience.Count % 3;
                    var total = entity.CvbuilderPreviousExperience.Count / 3;
                    int pi = 0;
                    for (int i = 0; i < (total > 0 ? total : remainder); i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            bool isSpace = false;
                            CAREERTIMELINEHTML += "<tr>";
                            int icon = 0;
                            foreach (var preExp in entity.CvbuilderPreviousExperience.Skip(pi).Take(3))
                            {
                                string OrgName = string.Empty;
                                var isDotsquares = preExp.OrganizationName.ToLower().Contains("dotsquares");
                                if (isDotsquares)
                                {
                                    OrgName = preExp.OrganizationName;
                                }
                                else
                                {
                                    OrgName = "Confidential";
                                }
                                if (icon > 0)
                                {
                                    CAREERTIMELINEHTML += "<td width=\"30\" align=\"center\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/Template2/images/arrow.png\" alt=\"\" /></td>";
                                }
                                CAREERTIMELINEHTML += "<td style='background-color:#f6f6f6; border-left:5px solid #006ea7; padding:15px;'>";
                                CAREERTIMELINEHTML += " <div>" +
                                    "<div style=\"font-size:15px; color:#006ea7; margin-bottom:8px\">"+ preExp.FromDate + " to "+ preExp.ToDate + "</div>" +
                                    "<div style=\"font-size:18px; color:#0b0c0e\">"+ OrgName + "</div></div></td>";
                                
                                icon++;
                                pi++;
                                isSpace = true;
                            }
                            CAREERTIMELINEHTML += "</tr>";
                            if (isSpace)
                            {
                                CAREERTIMELINEHTML += "<tr><td>&nbsp;</td></tr>";
                            }
                        }

                    }
                    
                }
                if (entity.CvbuilderPreviousExperience != null)
                {
                    var remainder = entity.CvbuilderPreviousExperience.Count % 3;
                    var total = entity.CvbuilderPreviousExperience.Count / 3;
                    int pi = 0;
                    for (int i = 0; i < (total > 0 ? total : remainder); i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            bool isSpace = false;                            
                            int icon = 0;
                            foreach (var preExp in entity.CvbuilderPreviousExperience.OrderByDescending(x=>x.Id).Skip(pi).Take(3))
                            {
                                if (icon > 0)
                                {
                                    previousExperienceHtml += "<tr><td style=\"height:10px; display: block;\">&nbsp;</td></tr>";
                                }
                                previousExperienceHtml += "<tr><td style=\"padding:10px 15px; background-color: #006ea7; color: #fff; font-size: 16px; line-height: 20px;\">"+preExp.Designation+"</td></tr>" +
                                 "<tr><td style=\"height:10px; display: block;\">&nbsp;</td></tr>" +
                                 "<tr><td><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\"><tr><td style=\"color: #000; font-size: 16px;line-height: 25px; padding: 0 0 3px 0; width: 70%;\">" +
                                 "<strong>"+preExp.OrganizationName+" </strong></td><td style=\"color: #000; font-size: 16px;line-height: 25px; padding: 0 0 3px 0; width: 30%;\"><strong> "+preExp.FromDate+" – "+preExp.ToDate+" </strong>" +
                                 "</td></tr></table></td></tr>";
                                icon++;
                                pi++;                             
                            }                            
                        }
                        
                    }

                }
                
                if (entity.User != null)
                {
                    if (entity.User.User_Tech != null)
                    {
                        var isCheck = entity.User.User_Tech.Where(x => x.SpecTypeId != 4 && x.SpecTypeId != 3).ToList();
                        if (isCheck != null && isCheck.Count > 0)
                        {
                            technologyHTML += "<tr><td style='padding: 5px 15px 15px;'><table width='100%' cellpadding='0' cellspacing='0'><tr><td style='padding-bottom: 30px;'>" +
                                    "<table width='80%' cellpadding='0' cellspacing='0' style='margin-left: auto; border-bottom: 0;'><tr>" +
                                    "<td style='text-transform: uppercase; color: #000; font-size: 18px; padding: 0 0 3px 0; text-align: right;'>" +
                                    "<strong> EXPERTISE </strong> <img style='position: relative; top: 2px;' src='" + SiteKey.DomainName + "PdfTemplates/Template2/images/expertise.png' alt='user' />" +
                                    "</td></tr><tr><td style='border-bottom: 1px solid #006ea7; text-align: right;'>" +
                                    "<img style='position: relative; top:3px;' src='" + SiteKey.DomainName + "PdfTemplates/Template2/images/bar-img-01.jpg' alt='user' />" +
                                    "</td></tr></table></td></tr>" +
                                    "<tr><td><table width='100%' cellpadding='0' cellspacing='0' style='border: 1px solid #000;'><thead><tr>" +
                                    "<th style='border-bottom: 1px solid #000; text-align: left; padding: 10px; font-size: 16px; line-height: 25px;'>S.No</th>" +
                                    "<th style='border: 1px solid #000; border-top: 0; text-align: left; padding: 10px; font-size: 16px; line-height: 25px;'>Technology Name</th>" +
                                    "<th style='border-bottom: 1px solid #000; text-align: left; padding: 10px; font-size: 16px; line-height: 25px;'>Experience</th>" +
                                    "</tr></thead><tbody>";


                            int index = 1;
                            foreach (var tech in entity.User.User_Tech.Where(x => x.SpecTypeId != 4 && x.SpecTypeId != 3).OrderBy(o=>o.SpecTypeId).Take(15))
                            {
                                string specType = ((Enums.TechnologySpecializationType)tech.SpecTypeId).GetDescription();
                                technologyHTML += "<tr><td style=\"font-size: 16px; line-height: 25px; color: #000; border-bottom: 1px solid #000;padding: 10px;\">" + index + "</td>" +
                                    "<td style=\"font-size: 16px; line-height: 25px; color: #000; border: 1px solid #000; border-top: 0; padding: 10px;\">" + tech.Technology.Title + "</td>" +
                                    "<td style=\"font-size: 16px; line-height: 25px; color: #000; border-bottom: 1px solid #000; padding: 10px;\">" + specType + "</td></tr>";
                                index++;
                            }
                            technologyHTML += "</tbody></table></td></tr></table></td></tr>";
                        }
                    }
                }
            }
            
            StreamReader filePtr;
            string fileData = "CVBuilderHtmlNew.html";            
            var pathToFile = _env.WebRootPath
                       + Path.DirectorySeparatorChar.ToString()
                       + "PdfTemplates"
                       + Path.DirectorySeparatorChar.ToString()
                       + "Template2"
                       + Path.DirectorySeparatorChar.ToString()
                       + "CVBuilderTemplate2.html";
            filePtr = System.IO.File.OpenText(pathToFile);
            fileData = filePtr.ReadToEnd();
            filePtr.Close();
            filePtr = null;
            string bullotImg = SiteKey.DomainName + "PdfTemplates/Template2/images/dot.jpg";
            string _ProfileSummary = string.Empty;
            string _TechnicalSkills = string.Empty;
            string _WorkExperience = string.Empty;
            if (!string.IsNullOrEmpty(model.ProfileSummary))
            {
                _ProfileSummary = model.ProfileSummary.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                        .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 20px;padding-bottom: 10px;background-position: left 6px;background-size: 7.5px;'>");
            }
            if (!string.IsNullOrEmpty(model.TechnicalSkills))
            {
                _TechnicalSkills = model.TechnicalSkills.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                        .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 20px;padding-bottom: 10px;background-position: left 5px;background-size: 7.5px;'>");
            }
            if (!string.IsNullOrEmpty(model.WorkExperience))
            {
                _WorkExperience = model.WorkExperience.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                       .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 20px;padding-bottom: 10px;background-position: left 5px;background-size: 7.5px;'>");
            }
            string _RolesAcross = string.Empty;
            if (!string.IsNullOrEmpty(model.RolesAcross))
            {
                string _RolesAcrossUL = model.RolesAcross.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                            .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 20px;padding-bottom: 10px;background-position: left 5px;background-size: 7.5px;'>");

                _RolesAcross = "<tr>" +
                    "<td style=\"padding:0 20px;\">" +
                    "<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\">" +
                    "<tr><td style=\"padding-top: 21px;\">" +
                    "<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\"><tr>" +
                    "<td style=\"text-transform: uppercase; color: #000; font-size: 18px; padding: 0 0 3px 0;\">" +
                    "<img style=\"position: relative; top: 2px;\" src=\"" + SiteKey.DomainName + "PdfTemplates/Template2/images/role-career.png\" alt=\"user\" /> <strong>" +
                    "ROLES ACROSS THE CAREER" +
                    "</strong></td></tr><tr><td style=\"border-bottom: 1px solid #000;\">" +
                    "<img style=\"position: relative; top:3px;\" src=\"" + SiteKey.DomainName + "PdfTemplates/Template2/images/bar-img.jpg\" alt=\"user\" />" +
                    "</td></tr></table></td></tr><tr><td style=\"height:20px; display: block;\">&nbsp;</td></tr>" +
                    "<tr><td>" + _RolesAcrossUL + "</td></tr></table></td></tr>";         

            }
            //string certificationsData = string.Empty;
            //if (!string.IsNullOrEmpty(certificationsHtml))
            //{
            //    certificationsData = "<tr><td style='color:#1e9dcb; font-size:23px; font-weight:700;  background:url(" + SiteKey.DomainName + "PdfTemplates/images/line2.png); background-repeat:repeat-x;  visibility: visible;'>" +
            //        "<span style='background-color: #fff; display: inline-block; padding-right: 10px; position: relative; z-index: 1; '>" +
            //        "<img src='" + SiteKey.DomainName + "PdfTemplates/images/certificate.png' align='left' style='margin-right:10px; position:relative; top:3px' /> Certifications" +
            //        "</span></td></tr><tr><td style='height:25px'></td></tr><tr><td><table width='100%' border='0' cellspacing='0' cellpadding='0'>" +
            //        "" + certificationsHtml + "</table></td></tr><tr><td style='height:25px'></td></tr>";
            //}
            string EmailDots = string.Empty;
            if (!string.IsNullOrEmpty(model.Email))
            {
                MailAddress address = new MailAddress(model.Email);
                string hostname = address.Host;
                if (hostname.ToLower() == "dotsquares.com")
                {
                    EmailDots = model.Email;
                }
            }
            string htmlString = fileData.Replace("@Name@", model.UserName)
                        .Replace("@Designation@", model.Designation)
                        .Replace("@DomainName@", SiteKey.DomainName)
                        .Replace("@Title@", model.Title)
                        .Replace("@Email@", EmailDots)
                        .Replace("@Languages@", model.Languages)
                        .Replace("@Linkedin@", model.Linkedin)
                        .Replace("@Phone@", model.Phone)
                        .Replace("@Location@", model.Address)
                        .Replace("@ProfileSummary@", _ProfileSummary)
                        .Replace("@TechnicalSkills@", _TechnicalSkills)
                        .Replace("@WorkExperience@", _WorkExperience)
                        .Replace("@RolesAcross@", _RolesAcross)
                        .Replace("@CoreCompetencies@", coreHtml)
                        .Replace("@Certifications@", certificationsHtml)
                        .Replace("@DOB@", DateOfBirth)
                        .Replace("@PreviousExperience@", previousExperienceHtml)
                        .Replace("@CAREERTIMELINE@", CAREERTIMELINEHTML)
                        .Replace("@Education@", educationHtml)
                        .Replace("@Technology@", technologyHTML)
                        .Replace("@UserProfile@", SiteKey.DomainName + UserProfile);

            printCVDto.htmlString = htmlString;
            printCVDto.Username = model.UserName;
            return printCVDto;
        }
        private CVViewDto ViewCVTemplate1(Cvbuilder entity)
        {
            CVViewDto model = new CVViewDto();
            string coreHtml = string.Empty;
            string certificationsHtml = string.Empty;
            string previousExperienceHtml = string.Empty;
            string educationHtml = string.Empty;
            if (entity != null)
            {
                model.Id = entity.Id;
                model.Title = entity.Title;
                model.UserName = entity.User.Name;
                model.Linkedin = entity.LinkedinId;
                model.Languages = entity.Languages;
                var users = _userLoginService.GetUserInfoByID(entity.UserId);
                model.UserName = users.Name;
                model.Designation = users.Designation.Name;
                model.Email = users.EmailOffice;
                model.Phone = users.MobileNumber;
                model.Address = users.Address;
                model.DateOfBirth = users.DOB != null ? users.DOB.Value.ToString("dd-MMM-yyyy") : string.Empty;
                string bullotImg = SiteKey.DomainName + "PdfTemplates/images/bullot.png";
                if (!string.IsNullOrEmpty(entity.ProfileSummary))
                {
                    model.ProfileSummary = entity.ProfileSummary.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                                .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 30px;padding-bottom: 10px;'>");
                }
                if (!string.IsNullOrEmpty(entity.TechnicalSkills))
                {
                    model.TechnicalSkills = entity.TechnicalSkills.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                    .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 30px;padding-bottom: 10px;'>");
                }
                if (!string.IsNullOrEmpty(entity.WorkExperience))
                {
                    model.WorkExperience = entity.WorkExperience.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                            .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 30px;padding-bottom: 10px;'>");
                }
                if (!string.IsNullOrEmpty(entity.RolesAcross))
                {
                    model.RolesAcross = entity.RolesAcross.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                            .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 30px;padding-bottom: 10px;'>");
                }
                if (entity.CvbuilderCoreCompetencies != null)
                {
                    foreach (var core in entity.CvbuilderCoreCompetencies)
                    {
                        coreHtml += "<tr>";
                        coreHtml += "<td style=\"color:#0b0c0e; font-size:16px\">" + core.Title + "</td>";
                        if (core.LevelId == 1)
                        {
                            coreHtml += "<td  style=\"vertical-align:bottom; padding-left:10px; text-align:right;\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/images/bar-99.png\" alt=\"\" /></td>";
                        }
                        else if (core.LevelId == 2)
                        {
                            coreHtml += "<td  style=\"vertical-align:bottom; padding-left:10px; text-align:right;\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/images/bar-70.png\" alt=\"\" /></td>";
                        }
                        else
                        {
                            coreHtml += "<td  style=\"vertical-align:bottom; padding-left:10px; text-align:right;\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/images/bar-30.png\" alt=\"\" /></td>";
                        }
                        coreHtml += "</tr><tr><td style=\"color:#0b0c0e; font-size:16px\">&nbsp;</td><td>&nbsp;</td></tr>";
                    }
                }
                if (entity.CvbuilderCertifications != null)
                {
                    var remainder = entity.CvbuilderCertifications.Count % 3;
                    var total = entity.CvbuilderCertifications.Count / 3;
                    int pi = 0;
                    for (int i = 0; i < (total > 0 ? total : remainder); i++)
                    {

                        for (int j = 0; j < 3; j++)
                        {
                            bool isSpace = false;
                            int spaceCenter = 0;
                            certificationsHtml += "<tr>";
                            foreach (var crt in entity.CvbuilderCertifications.Skip(pi).Take(3))
                            {
                                if (spaceCenter > 0)
                                {
                                    certificationsHtml += "<td width=\"2%\" style=\"display:inline-block\" align=\"center\"></td>";
                                }

                                certificationsHtml += "<td style=\"display:inline-block; vertical-align:top; width:32%;\">";
                                certificationsHtml += "<table width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\"><tbody><tr>";
                                certificationsHtml += "<td style=\"background-color:#f6f6f6; border-left:5px solid #1e9dcb; padding:15px 5px;\" width=\"50\" align=\"center\">";
                                certificationsHtml += "<img src=\"" + SiteKey.DomainName + "PdfTemplates/images/certificate1.png\" style=\"max-width:40px;\" alt=\"\"></td><td style=\"background-color:#f6f6f6; padding:15px 2px;\">";
                                certificationsHtml += "<div style=\"font-size:15px; color:#1e9dcb; margin-bottom:8px\">" + crt.CertificationName + "</div>";
                                //certificationsHtml += "<div style=\"font-size:12px;font-weight:700; color:#0b0c0e\">(" + crt.CertificationNumber + ")</div></td></tr></tbody></table></td>";
                                if (!string.IsNullOrWhiteSpace(crt.CertificationNumber))
                                {
                                    certificationsHtml += "<div style=\"font-size:12px;font-weight:700; color:#0b0c0e\">(" + crt.CertificationNumber + ")</div></td></tr></tbody></table></td>";
                                }
                                else
                                {
                                    certificationsHtml += "<div style=\"font-size:12px;font-weight:700; color:#0b0c0e\"></div></td></tr></tbody></table></td>";
                                }

                                pi++;
                                spaceCenter++;
                                isSpace = true;
                            }
                            certificationsHtml += "</tr>";
                            if (isSpace)
                            {
                                certificationsHtml += "<tr><td>&nbsp;</td></tr>";
                            }
                        }

                    }
                }
                if (entity.CvBuilderEducation != null)
                {

                    foreach (var edu in entity.CvBuilderEducation)
                    {
                        educationHtml += "<tr>";
                        educationHtml += "<td style=\"width:35px\" valign=\"top\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/images/bullot.png\" alt=\"\" /></td>";
                        educationHtml += "<td valign=\"top\" style=\"color:#0b0c0e; font-size:16px; line-height:20px\"><strong>" + edu.Title + "<br /></strong> " + edu.University + "</td>";
                        educationHtml += "</tr>";
                        educationHtml += "<tr><td style=\"width:35px\" valign=\"top\">&nbsp;</td><td valign=\"top\" style=\"color:#0b0c0e; font-size:16px; line-height:20px\">&nbsp;</td></tr>";
                    }

                }
                if (entity.CvbuilderPreviousExperience != null)
                {
                    var remainder = entity.CvbuilderPreviousExperience.Count % 3;
                    var total = entity.CvbuilderPreviousExperience.Count / 3;
                    int pi = 0;
                    for (int i = 0; i < (total > 0 ? total : remainder); i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            bool isSpace = false;
                            previousExperienceHtml += "<tr>";
                            int icon = 0;
                            foreach (var preExp in entity.CvbuilderPreviousExperience.Skip(pi).Take(3))
                            {
                                string OrgName = string.Empty;
                                var isDotsquares = preExp.OrganizationName.ToLower().Contains("dotsquares");
                                if (isDotsquares)
                                {
                                    OrgName = preExp.OrganizationName;
                                }
                                else
                                {
                                    OrgName = "Confidential";
                                }
                                if (icon > 0)
                                {
                                    previousExperienceHtml += "<td style=\"width:5%; display:inline-block;\" align=\"center\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/images/arrow.png\" alt=\"\" /></td>";
                                }
                                previousExperienceHtml += "<td align=\"left\" style=\"background-color:#f6f6f6; border-left:5px solid #1e9dcb; padding:15px;width:30%; display:inline-block;\">";
                                previousExperienceHtml += "<div style=\"font-size:16px; color:#0b0c0e; line-height:25px\"><strong>" + preExp.FromDate + " to " + preExp.ToDate + "</strong><br />" + OrgName + "<br /><strong style='font-size: 12px;font-style: italic;'>" + preExp.Designation + "</strong></div></td>";

                                icon++;
                                pi++;
                                isSpace = true;
                            }
                            previousExperienceHtml += "</tr>";
                            if (isSpace)
                            {
                                previousExperienceHtml += "<tr><td>&nbsp;</td></tr>";
                            }
                        }

                    }


                }

                model.CoreCompetencies = coreHtml;
                model.Certifications = certificationsHtml;
                model.Education = educationHtml;
                model.PreviousExperience = previousExperienceHtml;
                if (!string.IsNullOrEmpty(users.ProfilePicture))
                {
                    model.ProfilePicture = "images/profile/" + users.ProfilePicture;

                    string imagePath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath, $"Images\\profile\\" + users.ProfilePicture);
                    if (!System.IO.File.Exists(imagePath))
                    {
                        model.ProfilePicture = "PdfTemplates/images/DefaultImage.png";
                    }
                }
                else
                {
                    model.ProfilePicture = "PdfTemplates/images/DefaultImage.png";
                }
            }

            return model;
        }
        private CVViewDto ViewCVTemplate2(Cvbuilder entity)
        {
            CVViewDto model = new CVViewDto();
            string coreHtml = string.Empty;
            string certificationsHtml = string.Empty;
            string previousExperienceHtml = string.Empty;
            string CAREERTIMELINEHTML = string.Empty;
            string educationHtml = string.Empty;
            if (entity != null)
            {
                model.Id = entity.Id;
                model.Title = entity.Title;
                model.UserName = entity.User.Name;
                model.Linkedin = entity.LinkedinId;
                model.Languages = entity.Languages;
                var users = _userLoginService.GetUserInfoByID(entity.UserId);
                model.UserName = users.Name;
                model.Designation = users.Designation.Name;
                model.Email = users.EmailOffice;
                model.Phone = users.MobileNumber;
                model.Address = users.Address;
                model.DateOfBirth = users.DOB != null ? users.DOB.Value.ToString("dd-MMM-yyyy") : string.Empty;
                string bullotImg = SiteKey.DomainName + "PdfTemplates/Template2/images/dot.jpg";
                if (!string.IsNullOrEmpty(entity.ProfileSummary))
                {
                    model.ProfileSummary = entity.ProfileSummary.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                                .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 20px;padding-bottom: 10px;background-position: left 4px;background-size: 7.5px;'>");
                }
                if (!string.IsNullOrEmpty(entity.TechnicalSkills))
                {
                    model.TechnicalSkills = entity.TechnicalSkills.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                    .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 20px;padding-bottom: 10px;background-position: left 5px;background-size: 7.5px;'>");
                }
                if (!string.IsNullOrEmpty(entity.WorkExperience))
                {
                    model.WorkExperience = entity.WorkExperience.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                            .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 20px;padding-bottom: 10px;background-position: left 5px;background-size: 7.5px;'>");
                }
                if (!string.IsNullOrEmpty(entity.RolesAcross))
                {
                    model.RolesAcross = entity.RolesAcross.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                            .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 20px;padding-bottom: 10px;background-position: left 4px;background-size: 7.5px;'>");
                }
                if (entity.CvbuilderCoreCompetencies != null)
                {
                    foreach (var core in entity.CvbuilderCoreCompetencies)
                    {
                        coreHtml += "<tr>";
                        if (core.LevelId == 1)
                        {
                            coreHtml += "<td style=\"  width: 80px; text-align: left;vertical-align: top;padding: 6px 0;\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/Template2/images/rating-1.jpg\" alt=\"icon\" /></td>";
                        }
                        else if (core.LevelId == 2)
                        {
                            coreHtml += "<td style=\"  width: 80px; text-align: left;vertical-align: top;padding: 6px 0;\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/Template2/images/rating-2.jpg\" alt=\"icon\" /></td>";
                        }
                        else
                        {
                            coreHtml += "<td style=\"  width: 80px; text-align: left;vertical-align: top;padding: 6px 0;\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/Template2/images/rating-3.jpg\" alt=\"icon\" /></td>";
                        }
                        coreHtml += "<td style=\"font-size: 16px; line-height: 25px; color: #000;padding-bottom: 5px; \">" + core.Title + "</td>";
                        coreHtml += "</tr>";
                    }
                }
                //if (entity.CvbuilderCertifications != null)
                //{
                //    var remainder = entity.CvbuilderCertifications.Count % 3;
                //    var total = entity.CvbuilderCertifications.Count / 3;
                //    int pi = 0;
                //    for (int i = 0; i < (total > 0 ? total : remainder); i++)
                //    {

                //        for (int j = 0; j < 3; j++)
                //        {
                //            bool isSpace = false;
                //            int spaceCenter = 0;
                //            certificationsHtml += "<tr>";
                //            foreach (var crt in entity.CvbuilderCertifications.Skip(pi).Take(3))
                //            {
                //                if (spaceCenter > 0)
                //                {
                //                    certificationsHtml += "<td width=\"2%\" style=\"display:inline-block\" align=\"center\"></td>";
                //                }

                //                certificationsHtml += "<td style=\"display:inline-block; vertical-align:top; width:32%;\">";
                //                certificationsHtml += "<table width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\"><tbody><tr>";
                //                certificationsHtml += "<td style=\"background-color:#f6f6f6; border-left:5px solid #1e9dcb; padding:15px 5px;\" width=\"50\" align=\"center\">";
                //                certificationsHtml += "<img src=\"" + SiteKey.DomainName + "PdfTemplates/images/certificate1.png\" style=\"max-width:40px;position:relative;z-index:1;\" alt=\"\"></td><td style=\"background-color:#f6f6f6; padding:15px 2px;\">";
                //                certificationsHtml += "<div style=\"font-size:15px; color:#1e9dcb; margin-bottom:8px;position:relative;z-index:1;\">" + crt.CertificationName + "</div>";
                //                if (!string.IsNullOrWhiteSpace(crt.CertificationNumber))
                //                {
                //                    certificationsHtml += "<div style=\"font-size:12px;font-weight:700; color:#0b0c0e\">(" + crt.CertificationNumber + ")</div></td></tr></tbody></table></td>";
                //                }
                //                else
                //                {
                //                    certificationsHtml += "<div style=\"font-size:12px;font-weight:700; color:#0b0c0e\"></div></td></tr></tbody></table></td>";
                //                }
                //                pi++;
                //                spaceCenter++;
                //                isSpace = true;
                //            }
                //            certificationsHtml += "</tr>";
                //            if (isSpace)
                //            {
                //                certificationsHtml += "<tr><td>&nbsp;</td></tr>";
                //            }
                //        }

                //    }
                //}                
                if (entity.CvbuilderCertifications != null)
                {
                    bool icon = true;
                    certificationsHtml += "<td style=\"padding-top: 20px;\">";
                    foreach (var crt in entity.CvbuilderCertifications)
                    {
                        if (!string.IsNullOrEmpty(crt.CertificationsImage))
                        {
                            icon = false;
                            certificationsHtml += "<img style='margin-right: 10px; margin-bottom: 5px;height: 72px;width: 76px;' src='images/CertificationImage/" + crt.CertificationsImage + "' alt=''/>";
                        }
                    }
                    if (icon)
                    {
                        certificationsHtml = "";
                        certificationsHtml += "<td style=\"padding-top: 97px;\">";
                    }
                    certificationsHtml += "</td>";
                }
                else
                {
                    certificationsHtml += "<td style=\"padding-top: 97px;\"></td>";
                }
                if (entity.CvBuilderEducation != null)
                {

                    foreach (var edu in entity.CvBuilderEducation)
                    {
                        educationHtml += "<tr>";
                        educationHtml += "<td style=\"color: #fff; background-color: #7e7e7e; font-size: 16px; line-height: 20px; text-align: right; padding: 10px 15px; \">" + edu.Title + "<br />" + edu.University + "</td>";
                        educationHtml += "</tr>";
                    }

                }
                if (entity.CvbuilderPreviousExperience != null)
                {
                    var remainder = entity.CvbuilderPreviousExperience.Count % 3;
                    var total = entity.CvbuilderPreviousExperience.Count / 3;
                    int pi = 0;
                    for (int i = 0; i < (total > 0 ? total : remainder); i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            bool isSpace = false;
                            CAREERTIMELINEHTML += "<tr>";
                            int icon = 0;
                            foreach (var preExp in entity.CvbuilderPreviousExperience.Skip(pi).Take(3))
                            {
                                string OrgName = string.Empty;
                                var isDotsquares = preExp.OrganizationName.ToLower().Contains("dotsquares");
                                if(isDotsquares)
                                {
                                    OrgName = preExp.OrganizationName;
                                }
                                else
                                {
                                    OrgName = "Confidential";
                                }
                                if (icon > 0)
                                {
                                    CAREERTIMELINEHTML += "<td width=\"30\" align=\"center\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/Template2/images/arrow.png\" alt=\"\" /></td>";
                                }
                                CAREERTIMELINEHTML += "<td style='background-color:#f6f6f6; border-left:5px solid #006ea7; padding:15px;'>";
                                CAREERTIMELINEHTML += " <div>" +
                                    "<div style=\"font-size:15px; color:#006ea7; margin-bottom:8px\">" + preExp.FromDate + " to " + preExp.ToDate + "</div>" +
                                    "<div style=\"font-size:18px; color:#0b0c0e\">" + OrgName + "</div></div></td>";
                                //previousExperienceHtml += "<div style=\"font-size:16px; color:#0b0c0e; line-height:25px\"><strong>" + preExp.FromDate + " to " + preExp.ToDate + "</strong><br />" + preExp.OrganizationName + "<br /><strong style='font-size: 12px;font-style: italic;'>" + preExp.Designation + "</strong></div></td>";

                                icon++;
                                pi++;
                                isSpace = true;
                            }
                            CAREERTIMELINEHTML += "</tr>";
                            if (isSpace)
                            {
                                CAREERTIMELINEHTML += "<tr><td>&nbsp;</td></tr>";
                            }
                        }

                    }

                }
                if (entity.CvbuilderPreviousExperience != null)
                {
                    var remainder = entity.CvbuilderPreviousExperience.Count % 3;
                    var total = entity.CvbuilderPreviousExperience.Count / 3;
                    int pi = 0;
                    for (int i = 0; i < (total > 0 ? total : remainder); i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            bool isSpace = false;
                            int icon = 0;
                            foreach (var preExp in entity.CvbuilderPreviousExperience.OrderByDescending(x => x.Id).Skip(pi).Take(3))
                            {
                                if (icon > 0)
                                {
                                    previousExperienceHtml += "<tr><td style=\"height:10px; display: block;\">&nbsp;</td></tr>";
                                }
                                previousExperienceHtml += "<tr><td style=\"padding:10px 15px; background-color: #006ea7; color: #fff; font-size: 16px; line-height: 20px;\">" + preExp.Designation + "</td></tr>" +
                                 "<tr><td style=\"height:10px; display: block;\">&nbsp;</td></tr>" +
                                 "<tr><td><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\"><tr><td style=\"color: #000; font-size: 16px;line-height: 25px; padding: 0 0 3px 0; width: 70%;\">" +
                                 "<strong>" + preExp.OrganizationName + " </strong></td><td style=\"color: #000; font-size: 16px;line-height: 25px; padding: 0 0 3px 0; width: 30%;\"><strong> " + preExp.FromDate + " – " + preExp.ToDate + " </strong>" +
                                 "</td></tr></table></td></tr>";
                                icon++;
                                pi++;
                            }
                        }

                    }

                }
                string technologyHTML = string.Empty;
                if(entity.User != null)
                {
                    if (entity.User.User_Tech != null)
                    {
                        int index = 1;
                        foreach (var tech in entity.User.User_Tech.Where(x => x.SpecTypeId != 4 && x.SpecTypeId != 3).OrderBy(o=>o.SpecTypeId).Take(15))
                        {
                            string specType = ((Enums.TechnologySpecializationType)tech.SpecTypeId).GetDescription();
                            technologyHTML += "<tr><td style=\"font-size: 16px; line-height: 25px; color: #000; border-bottom: 1px solid #000;padding: 10px;\">" + index + "</td>" +
                                "<td style=\"font-size: 16px; line-height: 25px; color: #000; border: 1px solid #000; border-top: 0; padding: 10px;\">" + tech.Technology.Title + "</td>" +
                                "<td style=\"font-size: 16px; line-height: 25px; color: #000; border-bottom: 1px solid #000; padding: 10px;\">" + specType + "</td></tr>";
                            index++;
                        }
                    }
                }
                model.CoreCompetencies = coreHtml;
                model.Certifications = certificationsHtml;
                model.Education = educationHtml;
                model.PreviousExperience = previousExperienceHtml;
                model.CAREERTIMELINE = CAREERTIMELINEHTML;
                model.technology = technologyHTML;
                if (!string.IsNullOrEmpty(users.ProfilePicture))
                {
                    model.ProfilePicture = "images/profile/" + users.ProfilePicture;

                    string imagePath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath, $"Images\\profile\\" + users.ProfilePicture);
                    if (!System.IO.File.Exists(imagePath))
                    {
                        model.ProfilePicture = "PdfTemplates/images/DefaultImage.png";
                    }
                }
                else
                {
                    model.ProfilePicture = "PdfTemplates/images/DefaultImage.png";
                }
            }

            return model;
        }
    }
}

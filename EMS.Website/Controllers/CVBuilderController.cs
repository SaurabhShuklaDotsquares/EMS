using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using EMS.Web.Controllers;
using EMS.Web.Modals;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyModel;
using Newtonsoft.Json;
using NPOI;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using System.Drawing;

namespace EMS.Website.Controllers
{
    [CustomAuthorization()]
    public class CVBuilderController : BaseController
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
        public CVBuilderController(IUserLoginService userLoginService, ICVBuilderService cVBuilderService, IProjectClosureService projectClosureService, IProjectService projectService, IHostingEnvironment env, IDepartmentService departmentService, IDomainTypeService domainTypeService, ITechnologyParentService technologyParentService, ITechnologyService technologyService, ITechnologyParentMappingService technologyParentMappingService)
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
        }
        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult Index()
        {
            var model = new CVBuilderIndexDto();

            //model.DateFrom = DateTime.Now.ToFormatDateString("dd/MM/yyyy");
            //model.DateTo = DateTime.Now.ToFormatDateString("dd/MM/yyyy");

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

            return View(model);
        }
        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult Index(IDataTablesRequest request, CVBuilderSearchFilter searchFilter)
        {
            var pagingServices = new PagingService<Cvbuilder>(request.Start, request.Length);
            var expr = PredicateBuilder.True<Cvbuilder>();


            if (searchFilter.pm > 0)
            {
                expr = expr.And(x => x.User.PMUid == searchFilter.pm);
            }
            if (searchFilter.department > 0)
            {
                expr = expr.And(x => x.User.DeptId == searchFilter.department);
            }
            if (searchFilter.Uid_User > 0)
            {
                expr = expr.And(x => x.UserId == searchFilter.Uid_User);
            }
            //if (searchFilter.ExperienceType > 0)
            //{
            //    expr = expr.And(x => x.ExperienceId == searchFilter.ExperienceType);
            //}            
            if (searchFilter.Domains != null && searchFilter.Domains.Length > 0)
            {
                expr = expr.And(l => l.CvbuilderIndustry.Any(lt => searchFilter.Domains.Contains(lt.IndustryId.Value)));
            }
            if (searchFilter.Technologies != null && searchFilter.Technologies.Length > 0)
            {
                expr = expr.And(l => l.CvbuilderTechnology.Any(lt => searchFilter.Technologies.Contains(lt.TechnologyId.Value)));
            }
            if (searchFilter.ExperienceType != null && searchFilter.ExperienceType.Length > 0)
            {
                expr = expr.And(x => searchFilter.ExperienceType.Contains(x.ExperienceId));
            }

            DateTime? startDate = searchFilter.DateFrom.ToDateTime("dd/MM/yyyy");
            DateTime? endDate = searchFilter.DateTo.ToDateTime("dd/MM/yyyy");

            if (startDate.HasValue && endDate.HasValue)
            {
                expr = expr.And(L => L.CreatedDate >= startDate && L.CreatedDate <= endDate);
            }
            else if (startDate.HasValue)
            {
                expr = expr.And(L => L.CreatedDate >= startDate);
            }
            else if (endDate.HasValue)
            {
                expr = expr.And(L => L.CreatedDate <= endDate);
            }
            if (!RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId))
            {
                expr = expr.And(x => x.UserId == CurrentUser.Uid || SiteKey.AshishTeamPMUId == CurrentUser.Uid);
            }

            pagingServices.Filter = expr;
            pagingServices.Sort = (o) =>
            {
                return o.OrderByDescending(c => c.CreatedDate);
            };
            var _experience = _userLoginService.GetAllExperienceTypeList();
            int totalCount = 0;
            var isPMUser = IsPMEvent;
            var response = _cVBuilderService.GetCVBuilderByPaging(out totalCount, pagingServices);

            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                rowIndex = (index + 1) + (request.Start),
                Id = r.Id,
                Title = r.Title,
                ProfileSummary = r.ProfileSummary,
                TechnicalSkills = r.TechnicalSkills,
                WorkExperience = r.WorkExperience,
                RolesAcross = r.RolesAcross,
                CreatedDate = r.CreatedDate.ToFormatDateString("MMM dd, yyyy"),
                IsEdit = r.UserId == CurrentUser.Uid ? 1 : 0,
                Name = r.User.Name,
                Email = r.User.EmailOffice,
                Phone = r.User.MobileNumber,
                ExperienceType = _experience.Where(x => x.Id == r.ExperienceId).FirstOrDefault().Experience,
                Industry = string.Join(", ", r.CvbuilderIndustry.Select(x => x.Industry.DomainName)),
                Technology = string.Join(", ", r.CvbuilderTechnology.Select(x => x.Technology.Title)),
            }));
        }
        [HttpGet]
        public ActionResult Add(int? id)
        {
            CVBuilderDto model = new CVBuilderDto();
            model.Uid_User = RoleValidator.BA_RoleIds.Contains(CurrentUser.RoleId) ? CurrentUser.Uid : 0;

            // model.NextStartDate = DateTime.Now.ToFormatDateString("dd/MM/yyyy");

            List<UserLogin> userList = new List<UserLogin>();
            if (CurrentUser.RoleId == (int)Enums.UserRoles.UKPM)
            {
                userList = _userLoginService.GetAllDotsquaresDevelopers();
            }
            else
            {
                userList = _userLoginService.GetUsersByPM(PMUserId);
            }
            model.UserList = userList.Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString() }).OrderBy(s => s.Text).ToList();
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
            if (id > 0)
            {
                var entity = _cVBuilderService.cvBuilderFindById(id.Value);
                if (entity != null)
                {
                    model.Id = entity.Id;
                    model.Title = entity.Title;
                    //model.NextStartDate = entity.CreatedDate.ToFormatDateString("dd/MM/yyyy");
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
                    if (entity.CvbuilderCoreCompetencies != null)
                    {
                        model.dataList = entity.CvbuilderCoreCompetencies.OrderBy(x => x.Id)
                           .Select(s => new CVBuilderData
                           {
                               Title = s.Title
                           }).ToList();
                    }
                    if (entity.CvbuilderCertifications != null)
                    {
                        model.Certifications = entity.CvbuilderCertifications.OrderBy(x => x.Id)
                           .Select(s => new CVBuilderCertificationsData
                           {
                               Title = s.CertificationName,
                               CertificationsNumber = s.CertificationNumber,
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
                    if (entity.CvbuilderTechnology != null)
                    {
                        model.TechnologyParents = _technologyParentService.GetTechnologyParentList().
                           Select(n => new SelectListItem
                           {
                               Text = n.Title,
                               Value = n.Id.ToString(),
                               Selected = n.TechnologyParentMapping.Any(p => entity.CvbuilderTechnology.Any(y => y.TechnologyId == p.Technology.TechId))
                           }).ToList();
                        model.Technologies = _technologyParentMappingService.GetTechnologyParentList()
                                            .Where(x => model.TechnologyParents.Where(y => y.Selected == true)
                                            .Any(n => Convert.ToInt32(n.Value) == x.TechnologyParentId)
                                ).ToList().OrderBy(x => x.TechnologyParentId).Select(n => new SelectListItem
                                {
                                    Text = n.Technology.Title,
                                    Value = n.TechnologyId.ToString(),
                                    Selected = entity.CvbuilderTechnology.Any(x => x.TechnologyId == n.TechnologyId),
                                    Group = new SelectListGroup() { Name = n.TechnologyParentId.ToString() }
                                }).ToList();
                    }
                    else
                    {
                        model.Technologies = _technologyService.GetTechnologyList().Select(n => new SelectListItem { Text = n.Title, Value = n.TechId.ToString() }).ToList();
                        model.TechnologyParents = _technologyParentService.GetTechnologyParentList().Select(n => new SelectListItem { Text = n.Title, Value = n.Id.ToString() }).ToList();
                    }
                }
            }
            else
            {
                model.Industries = _domainTypeService.GetDomainList().Select(n => new SelectListItem { Text = n.DomainName, Value = n.DomainId.ToString() }).ToList();
                model.Technologies = _technologyService.GetTechnologyList().Select(n => new SelectListItem { Text = n.Title, Value = n.TechId.ToString() }).ToList();
                model.TechnologyParents = _technologyParentService.GetTechnologyParentList().Select(n => new SelectListItem { Text = n.Title, Value = n.Id.ToString() }).ToList();
            }

            model.UserName = CurrentUser.Name;
            model.Designation = CurrentUser.DesignationName;
            model.Email = CurrentUser.EmailOffice;
            model.Phone = CurrentUser.MobileNumber;
            return View(model);
        }
        [HttpPost]
        public ActionResult SaveRecords(string jsondata)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CVBuilder_Dto cvDto = new CVBuilder_Dto();
                    cvDto = JsonConvert.DeserializeObject<CVBuilder_Dto>(jsondata);
                    if (cvDto != null)
                    {
                        cvDto.Uid_User = CurrentUser.Uid;
                        _cVBuilderService.SaveOld(cvDto);
                    }
                    ShowSuccessMessage("Success!", "Record saved successfully.", false);
                    return RedirectToAction("index");
                }
            }
            catch (Exception ex)
            {
                string message = ex.GetBaseException().Message;
                ModelState.AddModelError("Error!", message);
            }
            return CreateModelStateErrors();
        }

        [HttpGet]
        public ActionResult Print(int? id)
        {
            CVBuilderDto model = new CVBuilderDto();
            string DateOfBirth = string.Empty;
            string UserProfile = string.Empty;
            string coreHtml = string.Empty;
            string certificationsHtml = string.Empty;
            string previousExperienceHtml = string.Empty;
            string educationHtml = string.Empty;
            if (id > 0)
            {
                var entity = _cVBuilderService.cvBuilderFindById(id.Value);
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
                    DateOfBirth = users.DOB != null ? users.DOB.Value.ToString("dd-MM-yyyy") : string.Empty;
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
                            coreHtml += "<td  style=\"vertical-align:bottom; padding-left:10px; text-align:right;\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/images/bar.png\" alt=\"\" /></td>";
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
                                    certificationsHtml += "<div style=\"font-size:12px;font-weight:700; color:#0b0c0e\">(" + crt.CertificationNumber + ")</div></td></tr></tbody></table></td>";

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
                                    if (icon > 0)
                                    {
                                        previousExperienceHtml += "<td style=\"width:4%; display:inline-block;\" align=\"center\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/images/arrow.png\" alt=\"\" /></td>";
                                    }
                                    previousExperienceHtml += "<td align=\"left\" style=\"background-color:#f6f6f6; border-left:5px solid #1e9dcb; padding:15px;width:27%; display:inline-block;\">";
                                    previousExperienceHtml += "<div style=\"font-size:16px; color:#0b0c0e; line-height:25px\"><strong>" + preExp.FromDate + " to " + preExp.ToDate + "</strong><br />" + preExp.OrganizationName + "<br />" + preExp.Designation + "</div></td>";

                                    icon++;
                                    pi++;
                                    isSpace = true;
                                }
                                previousExperienceHtml += "</tr>";
                                if (isSpace)
                                {
                                    previousExperienceHtml += "<tr><td>&nbsp;</td></tr>";
                                }
                                //previousExperienceHtml += "<tr>";
                                //int icon = 0;
                                //foreach (var preExp in entity.CvbuilderPreviousExperience.Skip(pi).Take(3))
                                //{
                                //    if (icon > 0)
                                //    {
                                //        previousExperienceHtml += "<td width=\"30\" align=\"center\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/images/arrow.png\" alt=\"\" /></td>";
                                //    }
                                //    previousExperienceHtml += "<td align=\"left\" style=\"background-color:#f6f6f6; border-left:5px solid #1e9dcb; padding:15px;\">";
                                //    previousExperienceHtml += "<div style=\"font-size:16px; color:#0b0c0e; line-height:25px\"><strong>" + preExp.FromDate + " to " + preExp.ToDate + "</strong><br />" + preExp.OrganizationName + "<br />" + preExp.Designation + "</div></td>";
                                //    icon++;
                                //    pi++;
                                //}
                                //previousExperienceHtml += "</tr>";
                                //previousExperienceHtml += "<tr><td>&nbsp;</td></tr>";
                            }

                        }


                        //for (int i = 0; i < remainder; i++)
                        //{
                        //    previousExperienceHtml += "<tr>";
                        //    for (int j = 0; j < 3; j++)
                        //    {
                        //        previousExperienceHtml += "<td>";
                        //        previousExperienceHtml += $"{pi}";
                        //        previousExperienceHtml += "</td>";
                        //        pi++;
                        //    }
                        //    previousExperienceHtml += "</tr>";
                        //}

                    }
                }
            }

            StreamReader filePtr;
            string fileData = "CVBuilderHtml.html";
            var webRoot = _env.WebRootPath;
            var pathToFile = _env.WebRootPath
                       + Path.DirectorySeparatorChar.ToString()
                       + "PdfTemplates"
                       + Path.DirectorySeparatorChar.ToString()
                       + "CVBuilderHtml.html";
            filePtr = System.IO.File.OpenText(pathToFile);
            fileData = filePtr.ReadToEnd();
            filePtr.Close();
            filePtr = null;
            string bullotImg = SiteKey.DomainName + "PdfTemplates/images/bullot.png";
            string _ProfileSummary = model.ProfileSummary.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                        .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 30px;padding-bottom: 10px;'>");
            string _TechnicalSkills = model.TechnicalSkills.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                        .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 30px;padding-bottom: 10px;'>");
            string _WorkExperience = model.WorkExperience.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                        .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 30px;padding-bottom: 10px;'>");

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
                certificationsData = "<tr><td style='color:#1e9dcb; font-size:23px; font-weight:700;  background:url("+ SiteKey.DomainName + "PdfTemplates/images/line2.png); background-repeat:repeat-x;  visibility: visible;'>" +
                    "<span style='background-color: #fff; display: inline-block; padding-right: 10px; position: relative; z-index: 1; '>" +
                    "<img src='"+ SiteKey.DomainName + "PdfTemplates/images/certificate.png' align='left' style='margin-right:10px; position:relative; top:3px' /> Certifications" +
                    "</span></td></tr><tr><td style='height:25px'></td></tr><tr><td><table width='100%' border='0' cellspacing='0' cellpadding='0'>" +
                    ""+ certificationsHtml + "</table></td></tr><tr><td style='height:25px'></td></tr>";
            }

            string htmlString = fileData.Replace("@Name@", model.UserName)
                        .Replace("@Designation@", model.Designation)
                        .Replace("@DomainName@", SiteKey.DomainName)
                        .Replace("@Title@", model.Title)
                        .Replace("@Email@", model.Email)
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



            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();


            // set converter options
            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            converter.Options.WebPageWidth = 1024;
            converter.Options.WebPageHeight = 0;
            converter.Options.MarginTop = 20;
            converter.Options.MarginBottom = 0;
            //converter.Options.PdfPageSize = PdfPageSize.Custom;
            //converter.Options.PdfPageCustomSize = new SizeF(Convert.ToInt32(1024), Convert.ToInt32(1370));
            converter.Options.WebPageFixedSize = false;
            // footer settings
            converter.Options.DisplayFooter = true;
            converter.Footer.DisplayOnFirstPage = true;
            converter.Footer.DisplayOnOddPages = true;
            converter.Footer.DisplayOnEvenPages = true;
            //converter.Footer.Height = 50;
            PdfHtmlSection footerHtml = new PdfHtmlSection(
             "<div><table style='margin-left: auto;margin-right: auto;'><tr><td><img src='" + SiteKey.DomainName + "Content/images/dots-logon.png' alt='' style='height: 15px; width: 15px;'></td>" +
             "<td style='font-size:12px;'> © Copyright 2023 Dotsquares Technologies (I) Pvt. Ltd. All Rights Reserved.</td></tr></table>" +
             "</div>",
             string.Empty);
            converter.Footer.Add(footerHtml);


            // create a new pdf document converting an url
            PdfDocument doc = converter.ConvertHtmlString(htmlString);
            string fileSavePath = Path.Combine(_env.WebRootPath, "Upload", "CV");
            string logoPath = Path.Combine(_env.WebRootPath, "PdfTemplates", "images");


            string imgFile = logoPath + "\\DefaultImage.png";

           
                
            //PdfTemplate template = doc.AddTemplate(doc.Pages[0].ClientRectangle);
            
            //PdfImageSection imageSection = new PdfImageSection(0, 0,
            //   doc.Pages[0].ClientRectangle.Width - 300,
            //   doc.Pages[0].ClientRectangle.Height - 150, imgFile);

            //PdfImageElement img = new PdfImageElement(
            //    doc.Pages[0].ClientRectangle.Width - 300,
            //    doc.Pages[0].ClientRectangle.Height - 150, imgFile);
            //imageSection.Transparency = 50;
            //template.Background = true;
            //template.Add(imageSection);

            // save pdf document
            byte[] pdf = doc.Save();
            //doc.Save(fileSavePath + "\\" + model.UserName + "_CV.pdf");
            // close pdf document
            doc.Close();
            //byte[] pdf = System.IO.File.ReadAllBytes(fileSavePath + "\\" + model.UserName + "_CV.pdf");
            // return resulted pdf document
            FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            fileResult.FileDownloadName = model.UserName + "_CV.pdf";
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
        public ActionResult View(int id)
        {
            CVViewDto model = new CVViewDto();
            string coreHtml = string.Empty;
            string certificationsHtml = string.Empty;
            string previousExperienceHtml = string.Empty;
            string educationHtml = string.Empty;
            if (id > 0)
            {
                var entity = _cVBuilderService.cvBuilderFindById(id);
                if (entity != null)
                {
                    model.Id = entity.Id;
                    model.Title = entity.Title;
                    model.UserName = entity.User.Name;
                    //model.ProfileSummary = entity.ProfileSummary;
                    //model.TechnicalSkills = entity.TechnicalSkills;
                    //model.WorkExperience = entity.WorkExperience;
                    //model.RolesAcross = entity.RolesAcross;
                    model.Linkedin = entity.LinkedinId;
                    model.Languages = entity.Languages;
                    var users = _userLoginService.GetUserInfoByID(entity.UserId);
                    model.UserName = users.Name;
                    model.Designation = users.Designation.Name;
                    model.Email = users.EmailOffice;
                    model.Phone = users.MobileNumber;
                    model.Address = users.Address;
                    model.DateOfBirth = users.DOB != null ? users.DOB.Value.ToString("dd-MM-yyyy") : string.Empty;
                    string bullotImg = SiteKey.DomainName + "PdfTemplates/images/bullot.png";
                    model.ProfileSummary = entity.ProfileSummary.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                                .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 30px;padding-bottom: 10px;'>");
                    model.TechnicalSkills = entity.TechnicalSkills.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                        .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 30px;padding-bottom: 10px;'>");
                    model.WorkExperience = entity.WorkExperience.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                                .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 30px;padding-bottom: 10px;'>");
                    model.RolesAcross = entity.RolesAcross.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                                .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 30px;padding-bottom: 10px;'>");

                    if (entity.CvbuilderCoreCompetencies != null)
                    {
                        foreach (var core in entity.CvbuilderCoreCompetencies)
                        {
                            coreHtml += "<tr>";
                            coreHtml += "<td style=\"color:#0b0c0e; font-size:16px\">" + core.Title + "</td>";
                            coreHtml += "<td  style=\"vertical-align:bottom; padding-left:10px; text-align:right;\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/images/bar.png\" alt=\"\" /></td>";
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
                                    certificationsHtml += "<div style=\"font-size:12px;font-weight:700; color:#0b0c0e\">(" + crt.CertificationNumber + ")</div></td></tr></tbody></table></td>";

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
                                    if (icon > 0)
                                    {
                                        previousExperienceHtml += "<td style=\"width:5%; display:inline-block;\" align=\"center\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/images/arrow.png\" alt=\"\" /></td>";
                                    }
                                    previousExperienceHtml += "<td align=\"left\" style=\"background-color:#f6f6f6; border-left:5px solid #1e9dcb; padding:15px;width:30%; display:inline-block;\">";
                                    previousExperienceHtml += "<div style=\"font-size:16px; color:#0b0c0e; line-height:25px\"><strong>" + preExp.FromDate + " to " + preExp.ToDate + "</strong><br />" + preExp.OrganizationName + "<br />" + preExp.Designation + "</div></td>";

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
            }

            return PartialView("_DisplayCVData", model);
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

        //[HttpPost]
        //public FileResult Download([FromBody] CVBuilderSearchFilter searchFilter)
        //{
        //    var pagingServices = new PagingService<Cvbuilder>(0, 5000);
        //    var expr = PredicateBuilder.True<Cvbuilder>();


        //    if (searchFilter.pm > 0)
        //    {
        //        expr = expr.And(x => x.User.PMUid == searchFilter.pm);
        //    }
        //    if (searchFilter.department > 0)
        //    {
        //        expr = expr.And(x => x.User.DeptId == searchFilter.department);
        //    }
        //    if (searchFilter.Uid_User > 0)
        //    {
        //        expr = expr.And(x => x.UserId == searchFilter.Uid_User);
        //    }
        //    if (searchFilter.Domains != null && searchFilter.Domains.Length > 0)
        //    {
        //        expr = expr.And(l => l.CvbuilderIndustry.Any(lt => searchFilter.Domains.Contains(lt.IndustryId.Value)));
        //    }
        //    if (searchFilter.Technologies != null && searchFilter.Technologies.Length > 0)
        //    {
        //        expr = expr.And(l => l.CvbuilderTechnology.Any(lt => searchFilter.Technologies.Contains(lt.TechnologyId.Value)));
        //    }
        //    if (searchFilter.ExperienceType != null && searchFilter.ExperienceType.Length > 0)
        //    {
        //        expr = expr.And(x => searchFilter.ExperienceType.Contains(x.ExperienceId));
        //    }

        //    DateTime? startDate = searchFilter.DateFrom.ToDateTime("dd/MM/yyyy");
        //    DateTime? endDate = searchFilter.DateTo.ToDateTime("dd/MM/yyyy");

        //    if (startDate.HasValue && endDate.HasValue)
        //    {
        //        expr = expr.And(L => L.CreatedDate >= startDate && L.CreatedDate <= endDate);
        //    }
        //    else if (startDate.HasValue)
        //    {
        //        expr = expr.And(L => L.CreatedDate >= startDate);
        //    }
        //    else if (endDate.HasValue)
        //    {
        //        expr = expr.And(L => L.CreatedDate <= endDate);
        //    }
        //    if (!RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId))
        //    {
        //        expr = expr.And(x => x.UserId == CurrentUser.Uid || SiteKey.AshishTeamPMUId == CurrentUser.Uid);
        //    }

        //    pagingServices.Filter = expr;
        //    pagingServices.Sort = (o) =>
        //    {
        //        return o.OrderByDescending(c => c.CreatedDate);
        //    };
        //    int totalCount = 0;
        //    var isPMUser = IsPMEvent;
        //    var response = _cVBuilderService.GetProjectReviewByPaging(out totalCount, pagingServices);


        //    var zipName = $"CV-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";
        //    string fileSavePath = Path.Combine(_env.WebRootPath, "Upload", "CV");

        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        //required: using System.IO.Compression;
        //        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
        //        {
        //            //QUery the Products table and get all image content
        //            response.ForEach(file =>
        //            {
        //                var _fileName = file.User.Name + "_CV.pdf";
        //                string path = Path.Combine(fileSavePath, _fileName);
        //                var entry = zip.CreateEntry(_fileName);
        //                using (var fileStream = new MemoryStream(System.IO.File.ReadAllBytes(path)))
        //                using (var entryStream = entry.Open())
        //                {
        //                    fileStream.CopyTo(entryStream);
        //                }
        //            });
        //        }
        //        return File(ms.ToArray(), "application/zip", zipName);
        //    }
        //}
        [HttpPost]
        public FileResult Download([FromBody] CVBuilderSearchFilter searchFilter)
        {
            var pagingServices = new PagingService<Cvbuilder>(0, 5000);
            var expr = PredicateBuilder.True<Cvbuilder>();
            string fileSavePath = Path.Combine(_env.WebRootPath, "Upload", "CV");

            if (searchFilter.pm > 0)
            {
                expr = expr.And(x => x.User.PMUid == searchFilter.pm);
            }
            if (searchFilter.department > 0)
            {
                expr = expr.And(x => x.User.DeptId == searchFilter.department);
            }
            if (searchFilter.Uid_User > 0)
            {
                expr = expr.And(x => x.UserId == searchFilter.Uid_User);
            }
            if (searchFilter.Domains != null && searchFilter.Domains.Length > 0)
            {
                expr = expr.And(l => l.CvbuilderIndustry.Any(lt => searchFilter.Domains.Contains(lt.IndustryId.Value)));
            }
            if (searchFilter.Technologies != null && searchFilter.Technologies.Length > 0)
            {
                expr = expr.And(l => l.CvbuilderTechnology.Any(lt => searchFilter.Technologies.Contains(lt.TechnologyId.Value)));
            }
            if (searchFilter.ExperienceType != null && searchFilter.ExperienceType.Length > 0)
            {
                expr = expr.And(x => searchFilter.ExperienceType.Contains(x.ExperienceId));
            }

            DateTime? startDate = searchFilter.DateFrom.ToDateTime("dd/MM/yyyy");
            DateTime? endDate = searchFilter.DateTo.ToDateTime("dd/MM/yyyy");

            if (startDate.HasValue && endDate.HasValue)
            {
                expr = expr.And(L => L.CreatedDate >= startDate && L.CreatedDate <= endDate);
            }
            else if (startDate.HasValue)
            {
                expr = expr.And(L => L.CreatedDate >= startDate);
            }
            else if (endDate.HasValue)
            {
                expr = expr.And(L => L.CreatedDate <= endDate);
            }
            if (!RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId))
            {
                expr = expr.And(x => x.UserId == CurrentUser.Uid || SiteKey.AshishTeamPMUId == CurrentUser.Uid);
            }

            pagingServices.Filter = expr;
            pagingServices.Sort = (o) =>
            {
                return o.OrderByDescending(c => c.CreatedDate);
            };
            int totalCount = 0;
            var isPMUser = IsPMEvent;
            var response = _cVBuilderService.GetCVBuilderByPaging(out totalCount, pagingServices);

            if (response != null && response.Count() > 0)
            {
                foreach (var entity in response)
                {
                    CVBuilderDto model = new CVBuilderDto();
                    string DateOfBirth = string.Empty;
                    string UserProfile = string.Empty;
                    string coreHtml = string.Empty;
                    string certificationsHtml = string.Empty;
                    string previousExperienceHtml = string.Empty;
                    string educationHtml = string.Empty;

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
                    DateOfBirth = users.DOB != null ? users.DOB.Value.ToString("dd-MM-yyyy") : string.Empty;
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
                            coreHtml += "<td  style=\"vertical-align:bottom; padding-left:10px; text-align:right;\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/images/bar.png\" alt=\"\" /></td>";
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
                                    certificationsHtml += "<div style=\"font-size:12px;font-weight:700; color:#0b0c0e\">(" + crt.CertificationNumber + ")</div></td></tr></tbody></table></td>";

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
                                    if (icon > 0)
                                    {
                                        previousExperienceHtml += "<td style=\"width:4%; display:inline-block;\" align=\"center\"><img src=\"" + SiteKey.DomainName + "PdfTemplates/images/arrow.png\" alt=\"\" /></td>";
                                    }
                                    previousExperienceHtml += "<td align=\"left\" style=\"background-color:#f6f6f6; border-left:5px solid #1e9dcb; padding:15px;width:27%; display:inline-block;\">";
                                    previousExperienceHtml += "<div style=\"font-size:16px; color:#0b0c0e; line-height:25px\"><strong>" + preExp.FromDate + " to " + preExp.ToDate + "</strong><br />" + preExp.OrganizationName + "<br />" + preExp.Designation + "</div></td>";

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

                    StreamReader filePtr;
                    string fileData = "CVBuilderHtml.html";
                    var webRoot = _env.WebRootPath;
                    var pathToFile = _env.WebRootPath
                               + Path.DirectorySeparatorChar.ToString()
                               + "PdfTemplates"
                               + Path.DirectorySeparatorChar.ToString()
                               + "CVBuilderHtml.html";
                    filePtr = System.IO.File.OpenText(pathToFile);
                    fileData = filePtr.ReadToEnd();
                    filePtr.Close();
                    filePtr = null;
                    string bullotImg = SiteKey.DomainName + "PdfTemplates/images/bullot.png";
                    string _ProfileSummary = model.ProfileSummary.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                                .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 30px;padding-bottom: 10px;'>");
                    string _TechnicalSkills = model.TechnicalSkills.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                                .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 30px;padding-bottom: 10px;'>");
                    string _WorkExperience = model.WorkExperience.Replace("<ul>", "<ul style='list-style: none; padding: 0;margin: 0 0 15px;'>")
                                .Replace("<li>", "<li style='background: url(" + bullotImg + ");background-repeat: no-repeat;padding-left: 30px;padding-bottom: 10px;'>");

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

                    string htmlString = fileData.Replace("@Name@", model.UserName)
                                .Replace("@Designation@", model.Designation)
                                .Replace("@DomainName@", SiteKey.DomainName)
                                .Replace("@Title@", model.Title)
                                .Replace("@Email@", model.Email)
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
                     "<td style='font-size:12px;'> © Copyright 2023 Dotsquares Technologies (I) Pvt. Ltd. All Rights Reserved.</td></tr></table>" +
                     "</div>",
                     string.Empty);
                    converter.Footer.Add(footerHtml);


                    // create a new pdf document converting an url
                    PdfDocument doc = converter.ConvertHtmlString(htmlString);

                    // save pdf document
                    doc.Save(fileSavePath + "\\" + model.UserName + "_CV.pdf");
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
                        var _fileName = file.User.Name + "_CV.pdf";
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
    }
    public class MonthYear
    {
        public List<SelectListItem> MonthsList { get; set; }
        public List<SelectListItem> YearsList { get; set; }
        public List<SelectListItem> FYearsList { get; set; }
    }
}
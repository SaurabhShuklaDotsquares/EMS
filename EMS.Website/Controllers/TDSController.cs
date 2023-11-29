using EMS.Core;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EMS.Data.Model;
using DataTables.AspNet.Core;
using EMS.Data;
using System.IO.Compression;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;
using EMS.Web.Models.Others;
using System.Text;
using EMS.Web.Code.LIBS;
using static EMS.Core.Enums;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;
using EMS.Web.LIBS;
using EMS.Website.Code.LIBS;
using EMS.Web.Models;
using NUglify;
using static EMS.Core.Encryption;

namespace EMS.Website.Controllers
{
    [Route("empInvestment")]
    public class TDSController : BaseController
    {
        private readonly IAppraiseService appraiseService;
        private readonly IUserLoginService userLoginService;
        private readonly ITDSService TDSService;
        private IHostingEnvironment _env;
        private static List<TdsempDeduction> TdsempDeductionlist;

        public TDSController(ITDSService TDSService, IAppraiseService appraiseService, IUserLoginService userLoginService, IHostingEnvironment _env)
        {
            this.TDSService = TDSService;
            this.appraiseService = appraiseService;
            this.userLoginService = userLoginService;
            this._env = _env;

        }

        [Route("Index")]
        [HttpGet]
        public ActionResult Index()
        {
            var AssesmentYear = TDSService.GetAssesmentYear(0);
            var currentYearID = AssesmentYear.OrderByDescending(x => x.AssesmentYearId).FirstOrDefault().AssesmentYearId;
            //var currentYearID = AssesmentYear.Where(x => x.YearRange == string.Format("{0}-{1}", (DateTime.Now.Year - 1), DateTime.Now.Year)).FirstOrDefault().AssesmentYearId;
            ViewBag.isCreatedTDS = (TDSService.GetTdsempDeductionByUIDAndYear(CurrentUser.Uid, currentYearID).Count == 0);
            //ViewBag.currentYearRangeId = AssesmentYear.Where(x => x.YearRange == string.Format("{0}-{1}", (DateTime.Now.Year - 1), DateTime.Now.Year)).FirstOrDefault().AssesmentYearId;
            ViewBag.currentYearRangeId = currentYearID;
            ViewBag.AssesmentYearId = AssesmentYear;
            ViewBag.isAcUser = RoleValidator.TL_AccountsAdminSR_DesignationIds.Contains(CurrentUser.DesignationId);
            ViewBag.LockUnlockTypeList = Enum.GetValues(typeof(LockUnlockType)).Cast<LockUnlockType>().Select(v => new SelectListItem
            {
                Text = v.GetDescription(),
                Value = ((int)v).ToString()
            }).ToList();

            ViewBag.LastModifiedEmployeeList = TDSService.GetLastModifiedEmployeeList().Select(x => new SelectListItem
            {
                Text = x.ModifyByEmpNavigation.Name,
                Value = x.ModifyByEmpNavigation.Uid.ToString()
            }).ToList();

            ViewBag.TdsTypeList = Enum.GetValues(typeof(TdsType)).Cast<TdsType>().Select(v => new SelectListItem
            {
                Text = v.GetDescription(),
                Value = ((int)v).ToString()
            }).ToList();

            return View();
        }
        [Route("Index")]
        [HttpPost]
        public IActionResult Index(IDataTablesRequest request, int AssesmentYearId, int Attendencecode, int LockUnlockType = 2, int ModifiedEmployeeId = 0, string UserInfo = "", string StartDate = "", string EndDate = "", int TdsTypeId = 0)
        {
            var pagingServices = new PagingService<TdsempDeduction>(request.Start, request.Length);
            var expr = PredicateBuilder.True<TdsempDeduction>();

            expr = expr.And(e => e.IsActive == true);

            bool isACUser = RoleValidator.TL_AccountsAdminSR_DesignationIds.Contains(CurrentUser.DesignationId);
            int currentUserId = CurrentUser.Uid;

            if (!isACUser)
            {
                expr = expr.And(x => x.Uid == currentUserId);
            }
            if (AssesmentYearId != 0)
            {
                expr = expr.And(x => x.AssesmentYearId == AssesmentYearId);

            }
            if (Attendencecode != 0)
            {
                expr = expr.And(x => x.U.AttendenceId.ToString().Contains(Attendencecode.ToString()));
            }
            if (LockUnlockType != 2)
            {
                expr = expr.And(x => x.IsLockUnlock == Convert.ToBoolean(LockUnlockType));
            }
            if (ModifiedEmployeeId != 0)
            {
                expr = expr.And(x => x.ModifyByEmp == ModifiedEmployeeId);
            }
            if (!string.IsNullOrEmpty(UserInfo))
            {
                expr = expr.And(x => x.U.Name.Contains(UserInfo) || x.U.EmailOffice.Contains(UserInfo) || x.U.PanNumber.Contains(UserInfo));
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                var startdate = StartDate.ToDateTime("dd/MM/yyyy");
                expr = expr.And(x => x.ModifyDateAc.Value.Date >= Convert.ToDateTime(startdate) || x.ModifyDateEmp.Value.Date >= Convert.ToDateTime(startdate.ToString()) || x.ModifyDateEmp.Value.Date == Convert.ToDateTime(startdate.ToString()) || x.ModifyDateAc.Value.Date == Convert.ToDateTime(startdate.ToString()));
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                var enddate = EndDate.ToDateTime("dd/MM/yyyy");
                expr = expr.And(x => x.ModifyDateAc.Value.Date <= Convert.ToDateTime(enddate.ToString()) || x.ModifyDateEmp.Value.Date <= Convert.ToDateTime(enddate.ToString()) || x.ModifyDateEmp.Value.Date == Convert.ToDateTime(enddate.ToString()) || x.ModifyDateAc.Value.Date == Convert.ToDateTime(enddate.ToString()));
            }

            if (TdsTypeId != 0)
            {
                if (TdsTypeId == (int)TdsType.HRADedution13A)
                {
                    expr = expr.And(x => x.Is13Ahradedution == true);
                }
                else if (TdsTypeId == (int)TdsType.HouseLoan24B)
                {
                    expr = expr.And(x => x.Is24Bhouseloan == true);
                }
            }

            if (request.Search.Value.HasValue())
            {
                string searchValue = request.Search.Value.Trim().ToLower();
                expr = expr.And(x => x.U != null && x.U.Name.ToLower().Contains(searchValue) || (x.U.AttendenceId.ToString() ?? "").ToLower().Contains(searchValue) || (x.U.EmailOffice ?? "").ToLower().Contains(searchValue));
            }

            pagingServices.Filter = expr;
            var orderColums = request.Columns.Where(x => x.Sort != null);
            pagingServices.Sort = (o) =>
            {
                foreach (var item in request.SortedColumns())
                {
                    switch (item.Name)
                    {
                        case "Name":
                            return o.OrderByColumn(item, c => c.U.Name);
                        case "AttendenceId":
                            return o.OrderByColumn(item, c => c.U.AttendenceId);
                        case "Email":
                            return o.OrderByColumn(item, c => c.U.EmailOffice);
                        case "UpdatedByEmp":
                            return o.OrderByColumn(item, c => c.ModifyDateEmp);
                        case "UpdatedByEmployer":
                            return o.OrderByColumn(item, c => c.ModifyByEmpNavigation.Name);
                        case "UpdatedBy":
                            return o.OrderByColumn(item, c => c.ModifyDateAc);
                        case "IsLockUnlock":
                            return o.OrderByColumn(item, c => c.IsLockUnlock);
                        default:
                            return o.OrderByColumn(item, c => c.ModifyDateEmp).OrderByDescending(c => c.AssesmentYearId);
                    }
                }
                return o.OrderByDescending(c => c.EmpDeductionId);
            };

            int totalCount = 0;

            var response = TDSService.GetTdsempDeductionByPaging(out totalCount, pagingServices);
            //TempData["data"] = response;
            //TempData.Keep();
            var keybytes = GetRiJndael_KeyBytes(SiteKey.Encryption_Key);
            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                rowIndex = (index + 1) + (request.Start),
                Id = r.EmpDeductionId,
                Name = r.U?.Name,
                AttendenceCode = r.U.AttendenceId,
                MailId = r.U.EmailOffice,
                LastUpdatedByEmp = r.ModifyDateEmp == null ? "" : r.ModifyDateEmp.Value.ToFormatDateString("MMM dd, yyyy hh:mm:ss"),
                LastUpdatedByEmployer = r.ModifyDateAc == null ? "" : r.ModifyDateAc.Value.ToFormatDateString("MMM dd, yyyy hh:mm:ss"),
                UpdatedByEmp = r.ModifyByEmpNavigation == null ? "" : r.ModifyByEmpNavigation.Name,
                UpdatedByAC = r.ModifyByAcNavigation == null ? "" : r.ModifyByAcNavigation.Name,
                Type = r.TypeId,
                AddedDate = r.CreatedDate.ToFormatDateString("MMM dd, yyyy"),
                UID = r.Uid,
                UIDEn = Convert.ToBase64String(EncryptStringToBytes(r.Uid.ToString(), keybytes, keybytes)),
                AssesmentYearId = r.AssesmentYearId,
                AssesmentYear = r.AssesmentYear.YearRange,
                AssesmentYearIdEn = Convert.ToBase64String(EncryptStringToBytes(r.AssesmentYearId.ToString(), keybytes, keybytes)),
                IsLockUnlock = r.IsLockUnlock,
                IsACUser = isACUser,
                //DeclarationSheetDate = r.TdsdeductionDoc.Where(x => x.Uid == r.Uid).Count() > 0 ? ( r.TdsdeductionDoc.Where(x => x.Uid == r.Uid) .FirstOrDefault().ModifyDate==null ? r.TdsdeductionDoc.Where(x => x.Uid == r.Uid).FirstOrDefault().CreatedDate.ToFormatDateString("MMM dd, yyyy hh:mm:ss") : r.TdsdeductionDoc.Where(x => x.Uid == r.Uid).FirstOrDefault().ModifyDate.Value.ToFormatDateString("MMM dd, yyyy hh:mm:ss")) : "",
                DeclarationSheetDate = TDSService.GetTdsDocListByUidAndAssesmentYearId(r.Uid, r.AssesmentYearId) != null ? (TDSService.GetTdsDocListByUidAndAssesmentYearId(r.Uid, r.AssesmentYearId).ModifyDate == null ? TDSService.GetTdsDocListByUidAndAssesmentYearId(r.Uid, r.AssesmentYearId).CreatedDate.ToFormatDateString("MMM dd, yyyy hh:mm:ss") : TDSService.GetTdsDocListByUidAndAssesmentYearId(r.Uid, r.AssesmentYearId).ModifyDate.Value.ToFormatDateString("MMM dd, yyyy hh:mm:ss")) : ""
            }));
        }

        [Route("InvestmentDetails")]
        public IActionResult CreateTDS(string UID, string AssesmentYearId)
        {
            if (string.IsNullOrWhiteSpace(UID))
            {
                return RedirectToAction("error404", "error");
            }

            UID = UID.Replace(" ", "+");

            //*********** GetRiJndael Decrypt**************Start/// 
            var keybytes = GetRiJndael_KeyBytes(SiteKey.Encryption_Key);

            byte[] TempEncrypted = Encoding.UTF8.GetBytes(UID);
            byte[] TempmyRijndaelKey2 = Encoding.UTF8.GetBytes(Convert.ToBase64String(keybytes));
            byte[] myRijndaelKey = Convert.FromBase64String(Encoding.ASCII.GetString(TempmyRijndaelKey2));
            try
            {
                byte[] encrypted = Convert.FromBase64String(Encoding.ASCII.GetString(TempEncrypted));
                UID = DecryptStringFromBytes(encrypted, myRijndaelKey, myRijndaelKey);
            }
            catch (Exception ex)
            {
                return RedirectToAction("error404", "error");
            }
            //*********** GetRiJndael Decrypt**************End///
            if (!string.IsNullOrWhiteSpace(AssesmentYearId))
            {
                AssesmentYearId = AssesmentYearId.Replace(" ", "+");
                byte[] EncryptedAssesmentYearId = Encoding.UTF8.GetBytes(AssesmentYearId);
                try
                {
                    byte[] encryptedAYear = Convert.FromBase64String(Encoding.ASCII.GetString(EncryptedAssesmentYearId));
                    AssesmentYearId = DecryptStringFromBytes(encryptedAYear, myRijndaelKey, myRijndaelKey);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("error404", "error");
                }
            }
            if (string.IsNullOrWhiteSpace(AssesmentYearId))
            {
                var currentyear = TDSService.GetAssesmentYearActive().AssesmentYearId;
                var isExists = TDSService.EmpDeductionByUIDAndYear(Convert.ToInt32(UID), currentyear);
                if (isExists != null)
                {
                    AssesmentYearId = isExists.AssesmentYearId.ToString();
                }
            }
            CreateTDSDto createTDSDto = new CreateTDSDto();
            List<TDSDeductionModel> tdsEmpList = new List<TDSDeductionModel>();
            ///List<TdsempDeduction> tdsEmpList = new List<TdsempDeduction>();

            if (!(string.IsNullOrEmpty(UID) && string.IsNullOrEmpty(AssesmentYearId)))
            {
                //tdsEmpList = TDSService.GetTdsempDeductionByUIDAndYear(Convert.ToInt32(UID), Convert.ToInt32(AssesmentYearId)).ToList();
                tdsEmpList = TDSService.GetTdsEmpDedDocByUIDAndYear(Convert.ToInt32(UID), Convert.ToInt32(AssesmentYearId)).ToList();
                //createTDSDto.Uid =Convert.ToInt32(UID);
                createTDSDto.Uid = Convert.ToBase64String(EncryptStringToBytes(UID, keybytes, keybytes));
                createTDSDto.AssesmentYearId = Convert.ToInt32(AssesmentYearId);
                ViewBag.DownloadZipCount = TDSService.GetTdsDocListByUidAssesmentYear(Convert.ToInt32(UID), Convert.ToInt32(AssesmentYearId)).Count();
            }
            createTDSDto.TdsdeductionType = TDSService.GetTdsDeductionType(null).ToList();
            createTDSDto.TDSType = TDSService.GetTDSType(null).ToList();
            ViewBag.IsEmployee = (CurrentUser.RoleId != (int)Enums.UserRoles.AC);
            ViewBag.DeductionTypeList = TDSService.GetTdsDeductionType(null).ToList();
            ViewBag.tdsEmpDocList = tdsEmpList;
            ViewBag.UID = UID;

            ViewBag.AssesmentYearId = AssesmentYearId;
            ViewBag.isAcUser = RoleValidator.TL_AccountsAdminSR_DesignationIds.Contains(CurrentUser.DesignationId);
            var data = (!string.IsNullOrEmpty(UID) && !string.IsNullOrEmpty(AssesmentYearId)) ? TDSService.GetTdsDocListByUidAndAssesmentYearId(Convert.ToInt32(UID), Convert.ToInt32(AssesmentYearId)) : null;
            ViewBag.DeclarationSheet = data;
            ViewBag.FileName = data != null ? data.FileName : null;

            var yearId = TDSService.GetAssesmentYear(0).OrderByDescending(x => x.AssesmentYearId).FirstOrDefault().AssesmentYearId;
            //var yearId = TDSService.GetAssesmentYear(0).Where(x => x.YearRange == string.Format("{0}-{1}", (DateTime.Now.Year - 1), DateTime.Now.Year)).FirstOrDefault().AssesmentYearId;
            if (yearId != 0)
            {
                ViewBag.AssesmentYear = TDSService.GetAssesmentYear(yearId).FirstOrDefault().YearRange;
            }

            return View("_CreateTDS", createTDSDto);
        }


        [Route("InvestmentDetails")]
        [HttpPost]
        public IActionResult CreateTDS(List<IFormFile> postedFiles, IFormCollection frm)
        {
            try
            {
                #region Check file type is valid alow only xlx,xlsx,csv
                if (frm.Files.Count > 0)
                {
                    string[] declarationsheetTypes = { ".xls", ".xlsx", ".csv" };
                    string[] multiplefileTypes = { ".png", ".jpg", ".jpeg", ".doc", ".docx", ".xls", ".xlsx", ".pdf", ".csv", ".txt" };
                    foreach (var file in frm.Files)
                    {
                        string extension = Path.GetExtension(file.FileName).ToLower();
                        if (file.Name == "filetxt")
                        {
                            if (!declarationsheetTypes.Contains(extension))
                            {
                                return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "File type is not valid", IsSuccess = false, Data = file.Name });
                            }
                        }
                        else
                        {
                            if (!multiplefileTypes.Contains(extension))
                            {
                                return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "File type is not valid", IsSuccess = false, Data = file.Name });
                            }
                        }
                    }
                }
                #endregion

                #region Save TDS employee deduction information
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/TDS");
                //string path = Path.Combine(@"D:\Local\EMSWebCore\EMS.Website", "wwwroot/Upload/TDS");
                var isAcUser = CurrentUser.RoleId == (int)Enums.UserRoles.AC;
                int UID = 0, YearID = 0, TdsEmpDeductionId = 0;
                bool IsExist = false, haverow = false;
                string AssesmentYear = string.Empty;
                string useridenc = frm["hdnUID"].ToString();
                string userid = useridenc;

                foreach (var item in frm.Keys.Where(k => k.StartsWith("DeductinTypeId_")))
                {
                    var idInfo = item.Split('_');
                    var TypeID = Convert.ToInt32(idInfo[1]);
                    var Len = Convert.ToInt32(idInfo[2]);

                    var DeductinTypeId = string.Format("DeductinTypeId_{0}_{1}", TypeID, Len);
                    var ClaimedByEmployee = string.Format("ClaimedByEmployee_{0}_{1}", TypeID, Len);
                    var GivenByEmployer = string.Format("GivenByEmployer_{0}_{1}", TypeID, Len);
                    var EmployeeRemark = string.Format("EmployeeRemark_{0}_{1}", TypeID, Len);
                    var EmployerRemark = string.Format("EmployerRemark_{0}_{1}", TypeID, Len);
                    var fileIds = string.Format("file_{0}_{1}", TypeID, Len);
                    var EmployeePanNumber = string.Format("EmployeePanNumber_{0}_{1}", TypeID, Len);


                    useridenc = useridenc.Replace(" ", "+");
                    //*********** GetRiJndael Decrypt**************Start/// 
                    var keybytes = GetRiJndael_KeyBytes(SiteKey.Encryption_Key);

                    byte[] TempEncrypted = Encoding.UTF8.GetBytes(useridenc);
                    byte[] TempmyRijndaelKey2 = Encoding.UTF8.GetBytes(Convert.ToBase64String(keybytes));
                    try
                    {
                        byte[] encrypted = Convert.FromBase64String(Encoding.ASCII.GetString(TempEncrypted));

                        byte[] myRijndaelKey = Convert.FromBase64String(Encoding.ASCII.GetString(TempmyRijndaelKey2));

                        userid = DecryptStringFromBytes(encrypted, myRijndaelKey, myRijndaelKey);
                        UID = Convert.ToInt32(userid);
                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("error404", "error");
                    }
                    try
                    {
                        YearID = TDSService.GetAssesmentYear(0).OrderByDescending(x => x.AssesmentYearId).FirstOrDefault().AssesmentYearId;
                        if (!string.IsNullOrEmpty(frm[DeductinTypeId]))
                        {
                            haverow = true;

                            TdsempDeduction tdsempDeduction = new TdsempDeduction();
                            tdsempDeduction.DeductionTypeId = Convert.ToInt32(frm[DeductinTypeId]); ;
                            tdsempDeduction.TypeId = TypeID;
                            tdsempDeduction.Uid = !(Convert.ToInt32(userid) == 0) ? Convert.ToInt32(userid) : CurrentUser.Uid;
                            tdsempDeduction.StatusId = 1;
                            //tdsempDeduction.AssesmentYearId = TDSService.GetAssesmentYear(0).Where(x => x.YearRange == string.Format("{0}-{1}", (DateTime.Now.Year - 1), DateTime.Now.Year)).FirstOrDefault().AssesmentYearId; ;
                            tdsempDeduction.AssesmentYearId = TDSService.GetAssesmentYear(0).OrderByDescending(x => x.AssesmentYearId).FirstOrDefault().AssesmentYearId;

                            tdsempDeduction.EmployerRemark = frm[EmployerRemark];
                            tdsempDeduction.GivenByEmployer = string.IsNullOrEmpty(frm[GivenByEmployer]) ? 0 : Convert.ToDecimal(frm[GivenByEmployer]);

                            tdsempDeduction.ClaimedByEmployee = string.IsNullOrEmpty(frm[ClaimedByEmployee]) ? 0 : Convert.ToDecimal(frm[ClaimedByEmployee]);
                            tdsempDeduction.EmployeeRemark = frm[EmployeeRemark];

                            if (!string.IsNullOrEmpty(frm[EmployeePanNumber]))
                            {
                                tdsempDeduction.PanNumberByEmployee = frm[EmployeePanNumber];
                            }

                            if (TypeID == (int)TdsType.HRADedution13A || TypeID == (int)TdsType.HouseLoan24B)
                            {
                                if (tdsempDeduction.DeductionTypeId == (int)DeductionType.AgreementTotalRentabove_1lacs || tdsempDeduction.DeductionTypeId == (int)DeductionType.LandlordPanAttestedCopyTotalRentabove_1lacs)
                                {
                                    tdsempDeduction.Is13Ahradedution = Convert.ToInt32(tdsempDeduction.ClaimedByEmployee) > 100000 ? true : false;
                                }
                                if (tdsempDeduction.DeductionTypeId == (int)DeductionType.LoanInterestCertificate)
                                {
                                    tdsempDeduction.Is24Bhouseloan = Convert.ToInt32(tdsempDeduction.ClaimedByEmployee) > 100000 ? true : false;
                                }
                            }


                            tdsempDeduction.CreatedDate = DateTime.Now;
                            tdsempDeduction.CreatedBy = CurrentUser.Uid;
                            if (isAcUser)
                            {
                                tdsempDeduction.ModifyDateAc = DateTime.Now;
                                tdsempDeduction.ModifyByAc = CurrentUser.Uid;
                            }
                            else
                            {
                                tdsempDeduction.ModifyDateEmp = DateTime.Now;
                                tdsempDeduction.ModifyByEmp = CurrentUser.Uid;
                            }
                            if (string.IsNullOrEmpty(frm["hdnUID"].ToString()))
                            {
                                tdsempDeduction.CreatedDate = DateTime.Now;
                            }
                            tdsempDeduction.IsActive = true;

                            var TdsEmp = TDSService.SaveTDSEmp(tdsempDeduction, isAcUser, out IsExist);
                            UID = TdsEmp.Uid;
                            TdsEmpDeductionId = TdsEmp.EmpDeductionId;
                            YearID = TdsEmp.AssesmentYearId;
                            var Assesment = TDSService.GetAssesmentYear(YearID);
                            AssesmentYear = Assesment != null ? Assesment.FirstOrDefault().YearRange : string.Empty;
                            List<IFormFile> files = Request.Form.Files.Where(x => x.Name == fileIds).ToList();

                            //if (files.Count > 0)
                            //{
                            //    TDSService.DeleteTdsdeductionDoc(TdsEmp.EmpDeductionId);
                            //}

                            foreach (var file in files)
                            {
                                if (!Directory.Exists(path))
                                    Directory.CreateDirectory(path);

                                FileInfo fileInfo = new FileInfo(file.FileName);
                                string fileName1 = CurrentUser.Uid.ToString() + "_" + file.FileName;
                                string fileNameWithPath = Path.Combine(path, fileName1);
                                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                                {
                                    file.CopyTo(stream);
                                }
                                TdsdeductionDoc TdsdeductionDoc = new TdsdeductionDoc();
                                TdsdeductionDoc.EmpDeductionId = TdsEmp.EmpDeductionId;
                                TdsdeductionDoc.FileName = fileName1;
                                TdsdeductionDoc.Files = fileName1;
                                TdsdeductionDoc.CreatedDate = DateTime.Now;
                                TdsdeductionDoc.IsActive = true;
                                var TdsEmpDoc = TDSService.SaveTdsdeductionDoc(TdsdeductionDoc);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                #endregion

                #region Declaration Sheet Upload Code
                var filetxt = Request.Form.Files["filetxt"];
                if (filetxt != null && TdsEmpDeductionId != 0 && UID != 0)
                {
                    var tdsDoc = TDSService.GetTdsDocListByUidAndAssesmentYearId(UID, YearID);
                    if (tdsDoc == null)
                    {
                        FileInfo fileInfo = new FileInfo(filetxt.FileName);
                        string fileName1 = "DeclarationSheet_" + CurrentUser.Uid.ToString() + "_" + filetxt.FileName;
                        string fileNameWithPath = Path.Combine(path, fileName1);
                        using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                        {
                            filetxt.CopyTo(stream);
                        }
                        TdsdeductionDoc TdsdeductionDoc = new TdsdeductionDoc();
                        TdsdeductionDoc.EmpDeductionId = TdsEmpDeductionId;
                        TdsdeductionDoc.FileName = fileName1;
                        TdsdeductionDoc.Files = fileName1;
                        TdsdeductionDoc.CreatedDate = tdsDoc == null ? DateTime.Now : tdsDoc.CreatedDate;
                        TdsdeductionDoc.IsActive = true;
                        TdsdeductionDoc.Uid = UID;
                        var TdsEmpDoc = TDSService.SaveTdsdeductionDoc(TdsdeductionDoc);
                    }
                    else
                    {
                        FileInfo fileInfo = new FileInfo(filetxt.FileName);
                        string fileName1 = "DeclarationSheet_" + CurrentUser.Uid.ToString() + "_" + filetxt.FileName;
                        string fileNameWithPath = Path.Combine(path, fileName1);
                        string deleteImagePath = Path.Combine(path, tdsDoc.FileName);

                        if (!string.IsNullOrEmpty(deleteImagePath))
                        {
                            System.IO.File.Delete(deleteImagePath);
                        }

                        using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                        {
                            filetxt.CopyTo(stream);
                        }

                        tdsDoc.EmpDeductionId = TdsEmpDeductionId;
                        tdsDoc.FileName = fileName1;
                        tdsDoc.Files = fileName1;
                        tdsDoc.CreatedDate = tdsDoc.CreatedDate;
                        tdsDoc.ModifyDate = DateTime.Now;
                        tdsDoc.IsActive = true;
                        tdsDoc.Uid = UID;
                        var TdsEmpDoc = TDSService.UpdateTdsdeductionDoc(tdsDoc);
                    }
                }
                #endregion

                if (haverow)
                {
                    #region Send mail for accounts
                    string MailMessageForAccounts = !(IsExist) ? "Investment details has been update successfully" : "Investment details has been save successfully";
                    var userInfo = userLoginService.GetUserInfoByID(UID);
                    FlexiMail objSendMail = new FlexiMail();

                    var v0 = "";  //user first name
                    var v1 = "";  //portal link
                    var v2 = "";  //user-id
                    var v3 = "";  //password
                    var v4 = "";

                    if (UID != 0 && UID == CurrentUser.Uid)
                    {
                        if (userInfo != null)
                        {
                            v0 = userInfo.Name;
                            v1 = SiteKey.DomainName;
                            v2 = userInfo.EmailOffice;
                            v4 = Convert.ToString(userInfo.AttendenceId);

                            var ValueArray = new string[] { v0, v1, v2, v4 };
                            objSendMail.ValueArray = ValueArray;

                            StringBuilder templatedata = new StringBuilder();
                            templatedata.Append("<html>");
                            templatedata.Append("<head>");
                            templatedata.Append("</head>");
                            templatedata.Append("<body>");
                            templatedata.Append("<div style='width: 500px; background - color:white; margin: auto; '>");

                            templatedata.Append("<div style='text-align:center; font-weight:bold;'>");
                            templatedata.Append("TDS Submission Report");
                            templatedata.Append("</div>");

                            templatedata.Append("<hr>");

                            templatedata.Append("<div style='font-weight:bold;'>");
                            templatedata.Append("Hi Accounts Team,");
                            templatedata.Append("</div>");

                            templatedata.Append("<br>");

                            templatedata.Append("<div style='text-align:justify;'> ");
                            templatedata.Append("Please check Investment records as, " + MailMessageForAccounts + " by " + userInfo.Name);
                            templatedata.Append("</div>");

                            templatedata.Append("<br>");

                            templatedata.Append("<table border='1' style='border-collapse: collapse; '>");
                            templatedata.Append("<tr>");
                            templatedata.Append("<td style='text-align:left; width:50%;'>");
                            templatedata.Append("Name");
                            templatedata.Append("</td>");
                            templatedata.Append("<td style='text-align:left; width:50%;'>");
                            templatedata.Append("Email");
                            templatedata.Append("</td>");
                            templatedata.Append("<td style='text-align:right; width:100%; padding:10px;'>");
                            templatedata.Append("AttendenceId");
                            templatedata.Append("</td>");
                            templatedata.Append("</tr>");
                            templatedata.Append("<tr>");
                            templatedata.Append("<td style='padding: 5px;'>");
                            templatedata.Append(userInfo.Name);
                            templatedata.Append("</td>");
                            templatedata.Append("<td style='padding: 5px;'>");
                            templatedata.Append(userInfo.EmailOffice);
                            templatedata.Append("</td>");
                            templatedata.Append("<td style='padding: 5px; text-align:right;'>");
                            templatedata.Append(userInfo.AttendenceId);
                            templatedata.Append("</td>");
                            templatedata.Append("</tr>");
                            templatedata.Append("</table>");

                            templatedata.Append("<br>");

                            templatedata.Append("<div style='font-weight:bold;'>");
                            templatedata.Append("Thanks & Regards:");
                            templatedata.Append("</div>");

                            templatedata.Append("<div style='color:darkblue;'>");
                            templatedata.Append("Team EMS");
                            templatedata.Append("</div>");

                            templatedata.Append("</div>");

                            templatedata.Append("</body>");
                            templatedata.Append("</html>");

                            objSendMail.From = userInfo.EmailOffice; //"info@dotsquares.com";
                            objSendMail.To = SiteKey.AccountsEmailId;
                            //objSendMail.CC = SiteKey.HREmailId;
                            //objSendMail.BCC = "devendra.sunaria@dotsquares.com";
                            objSendMail.Subject = "Investment details(" + AssesmentYear + ") submitted by " + userInfo.Name + "(" + userInfo.AttendenceId + ")";
                            objSendMail.MailBodyManualSupply = true;
                            objSendMail.MailBody = Convert.ToString(templatedata);
                            objSendMail.Send();
                        }
                    }
                    #endregion

                    var isLogWrite = !string.IsNullOrEmpty(frm["hdnUID"].ToString()) ? TDSService.WriteLog(UID, YearID) : true;
                    ShowSuccessMessage("Success!", MailMessageForAccounts, false);
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = MailMessageForAccounts, IsSuccess = true });
                }
                else
                {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Fill at least any one Deduction Type from any Investment Category with amount 0, In case no investment.", IsSuccess = false });
                }
            }
            catch (Exception Ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Message = Ex.Message, IsSuccess = false });
            }
        }


        [Route("Download")]

        public FileResult Download(string Uid, int AssesmentYearId)
        {
            Uid = Uid.Replace(" ", "+");
            //*********** GetRiJndael Decrypt**************Start/// 
            var keybytes = GetRiJndael_KeyBytes(SiteKey.Encryption_Key);

            byte[] TempEncrypted = Encoding.UTF8.GetBytes(Uid);
            byte[] TempmyRijndaelKey2 = Encoding.UTF8.GetBytes(Convert.ToBase64String(keybytes));
            try
            {
                byte[] encrypted = Convert.FromBase64String(Encoding.ASCII.GetString(TempEncrypted));

                byte[] myRijndaelKey = Convert.FromBase64String(Encoding.ASCII.GetString(TempmyRijndaelKey2));

                Uid = DecryptStringFromBytes(encrypted, myRijndaelKey, myRijndaelKey);
            }
            catch (Exception ex)
            {

            }
            var webRoot = _env.WebRootPath;

            var zipName = $"Attachment-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";
            string fileSavePath = Path.Combine(webRoot, "Upload", "TDS");

            using (MemoryStream ms = new MemoryStream())
            {
                //required: using System.IO.Compression;
                using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    //QUery the Products table and get all image content

                    TDSService.GetTdsDocListByUidAssesmentYear(Convert.ToInt32(Uid), AssesmentYearId).ToList().ForEach(file =>
                    {
                        var entry = zip.CreateEntry(file.FileName);
                        using (var fileStream = new MemoryStream(System.IO.File.ReadAllBytes(Path.Combine(fileSavePath, file.FileName))))
                        using (var entryStream = entry.Open())
                        {
                            fileStream.CopyTo(entryStream);
                        }
                    });
                }
                return File(ms.ToArray(), "application/zip", zipName);
            }
        }


        [Route("DocDetails")]
        [HttpGet]
        public ActionResult DocDetails(int id)
        {
            return PartialView("_DocDetails", TDSService.GetTdsDocList(id).ToList());

        }
        #region [Delete Doc]


        [Route("DeleteDoc/{id}")]

        [HttpGet]
        public ActionResult DeleteDoc(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete?",
                Size = Enums.ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Record ?" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" },
                ID = id.ToString()
            });
        }


        [Route("DeleteDoc/{id}")]

        [HttpPost]
        public ActionResult DeleteDoc(IFormCollection frm, int id)
        {
            var isDelete = TDSService.DeleteDoc(id);
            if (isDelete)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Record deleted successfully.", IsSuccess = true, Data = Convert.ToString(id) });

            }
            else
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = "Unable to delete Doc" });
            }
        }
        #endregion

        #region Mail
        [Route("InvestmentMail")]
        [HttpGet]
        public ActionResult InvestmentMail(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("error404", "error");
            }
            id = id.ToString().Replace(" ", "+");
            //*********** GetRiJndael Decrypt**************Start/// 
            var keybytes = GetRiJndael_KeyBytes(SiteKey.Encryption_Key);

            byte[] TempEncrypted = Encoding.UTF8.GetBytes(id);
            byte[] TempmyRijndaelKey2 = Encoding.UTF8.GetBytes(Convert.ToBase64String(keybytes));
            try
            {
                byte[] encrypted = Convert.FromBase64String(Encoding.ASCII.GetString(TempEncrypted));

                byte[] myRijndaelKey = Convert.FromBase64String(Encoding.ASCII.GetString(TempmyRijndaelKey2));

                id = DecryptStringFromBytes(encrypted, myRijndaelKey, myRijndaelKey);
            }
            catch (Exception ex)
            {
                return RedirectToAction("error404", "error");
            }
            StringBuilder s = new StringBuilder();
            s.Append("<body style='margin: 0;padding: 0;height: 100 %;' class='main - body'>");
            s.Append("<header style='grid-area: header;'>");
            s.Append("HEADER");
            s.Append("</header>");
            s.Append("<main style = 'grid-area: main;'>");
            s.Append("</main>");
            s.Append("<footer style ='grid-area: footer;'>");
            s.Append("this is footer </footer>");
            s.Append("</body>");
            ViewBag.htmlTemp = s;
            ViewBag.UID = id;
            return PartialView("_InvestmentMail", TDSService.GetTdsDocList(Convert.ToInt32(id)).ToList());

        }
        #endregion

        #region
        [Route("UpdateLockUnlock")]
        [HttpPost]
        public ActionResult UpdateLockUnlock(int Uid, bool LockUnlock, int AssesmentYearId)
        {
            var EmployeeTds = TDSService.GetTdsempDeductionByUIDAndYear(Uid, AssesmentYearId);
            if (EmployeeTds != null && EmployeeTds.Any())
            {
                foreach (var item in EmployeeTds)
                {
                    item.IsLockUnlock = LockUnlock;
                    item.ModifyDateAc = DateTime.Now;
                }
                TDSService.UpdateTDSLockUnlock(EmployeeTds);

                return Ok(Json(true));
            }
            return NotFound(Json(false));
        }
        #endregion

        #region
        [Route("InvestmentMail")]
        [HttpPost]
        public ActionResult InvestmentMail(string MailHtml, int hdnUID)
        {
            try
            {
                var userInfo = userLoginService.GetUserInfoByID(hdnUID);
                FlexiMail objSendMail = new FlexiMail();

                var v0 = "";  //user first name
                var v1 = "";  //portal link
                var v2 = "";  //user-id
                var v3 = "";  //password

                if (userInfo != null)
                {
                    v0 = userInfo.Name;
                    v1 = SiteKey.DomainName;
                    v2 = userInfo.EmailOffice;


                    var ValueArray = new string[] { v0, v1, v2 };
                    objSendMail.ValueArray = ValueArray;

                    StringBuilder templatedata = new StringBuilder();
                    templatedata.Append("<html>");
                    templatedata.Append("<head>");
                    templatedata.Append("</head>");
                    templatedata.Append("<body>");
                    templatedata.Append("<div style='width: 500px; background - color:white; margin: auto; '>");

                    templatedata.Append("<div style='text-align:center; font-weight:bold;'>");
                    templatedata.Append("Your TDS Submission Report");
                    templatedata.Append("</div>");

                    templatedata.Append("<hr>");

                    templatedata.Append("<div style='font-weight:bold;'>");
                    templatedata.Append("Dear " + userInfo.Name + ",");
                    templatedata.Append("</div>");

                    templatedata.Append("<br>");

                    templatedata.Append("<div style='text-align:justify;'> ");
                    templatedata.Append(MailHtml);
                    templatedata.Append("</div>");

                    templatedata.Append("<br>");

                    templatedata.Append("<div style='text-align:justify;'> ");
                    templatedata.Append("If you have any doubt and any query so please contact us.");
                    templatedata.Append("</div>");

                    templatedata.Append("<br>");

                    templatedata.Append("<div style='font-weight:bold;'>");
                    templatedata.Append("Thanks & Regards:");
                    templatedata.Append("</div>");

                    templatedata.Append("<div style='color:darkblue;'>");
                    templatedata.Append("Team Accounts");
                    templatedata.Append("</div>");

                    templatedata.Append("<div style='color:royalblue;'>");
                    templatedata.Append("Dotsquares");
                    templatedata.Append("</div>");

                    templatedata.Append("</div>");

                    templatedata.Append("</body>");
                    templatedata.Append("</html>");
                    //var webRoot = _env.WebRootPath;
                    //var filePath = Path.Combine(webRoot, "EmailTemplate", $"WelcomeEmail.html");
                    //var templatedata = objSendMail.GetHtml(filePath);

                    objSendMail.From = SiteKey.AccountsEmailId; //"info@dotsquares.com";
                    objSendMail.To = userInfo.EmailOffice;
                    //objSendMail.CC = SiteKey.HREmailId;
                    //objSendMail.BCC = "devendra.sunaria@dotsquares.com";
                    objSendMail.Subject = "Your TDS Submission Report";
                    objSendMail.MailBodyManualSupply = true;
                    objSendMail.MailBody = Convert.ToString(templatedata);
                    objSendMail.Send();

                    return Ok(Json(true));
                }
                else
                {
                    return Ok(Json(false));
                }
            }
            catch (Exception Ex)
            {
                return NotFound(Json(Ex.Message));
            }

        }
        #endregion

        [Route("DownloadReport")]
        [HttpGet]
        public ActionResult DownloadReport(IDataTablesRequest requests, int AssesmentYearId, int Attendencecode, int LockUnlockType = 2, int ModifiedEmployeeId = 0, string UserInfo = "", string StartDate = "", string EndDate = "", int TdsTypeId = 0)
        {
            var pagingServices = new PagingService<TdsempDeduction>(0, int.MaxValue);
            var expr = PredicateBuilder.True<TdsempDeduction>();

            expr = expr.And(e => e.IsActive == true);

            bool isACUser = RoleValidator.TL_AccountsAdminSR_DesignationIds.Contains(CurrentUser.DesignationId);
            int currentUserId = CurrentUser.Uid;

            if (!isACUser)
            {
                expr = expr.And(x => x.Uid == currentUserId);
            }
            if (AssesmentYearId != 0)
            {
                expr = expr.And(x => x.AssesmentYearId == AssesmentYearId);

            }
            if (Attendencecode != 0)
            {
                expr = expr.And(x => x.U.AttendenceId.ToString().Contains(Attendencecode.ToString()));
            }
            if (LockUnlockType != 2)
            {
                expr = expr.And(x => x.IsLockUnlock == Convert.ToBoolean(LockUnlockType));
            }
            if (ModifiedEmployeeId != 0)
            {
                expr = expr.And(x => x.ModifyByEmp == ModifiedEmployeeId);
            }
            if (!string.IsNullOrEmpty(UserInfo))
            {
                expr = expr.And(x => x.U.Name.Contains(UserInfo) || x.U.EmailOffice.Contains(UserInfo) || x.U.PanNumber.Contains(UserInfo));
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                var startdate = StartDate.ToDateTime("dd/MM/yyyy");
                expr = expr.And(x => x.ModifyDateAc.Value.Date >= Convert.ToDateTime(startdate) || x.ModifyDateEmp.Value.Date >= Convert.ToDateTime(startdate.ToString()) || x.ModifyDateEmp.Value.Date == Convert.ToDateTime(startdate.ToString()) || x.ModifyDateAc.Value.Date == Convert.ToDateTime(startdate.ToString()));
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                var enddate = EndDate.ToDateTime("dd/MM/yyyy");
                expr = expr.And(x => x.ModifyDateAc.Value.Date <= Convert.ToDateTime(enddate.ToString()) || x.ModifyDateEmp.Value.Date <= Convert.ToDateTime(enddate.ToString()) || x.ModifyDateEmp.Value.Date == Convert.ToDateTime(enddate.ToString()) || x.ModifyDateAc.Value.Date == Convert.ToDateTime(enddate.ToString()));
            }

            if (TdsTypeId != 0)
            {
                if (TdsTypeId == (int)TdsType.HRADedution13A)
                {
                    expr = expr.And(x => x.Is13Ahradedution == true);
                }
                else if (TdsTypeId == (int)TdsType.HouseLoan24B)
                {
                    expr = expr.And(x => x.Is24Bhouseloan == true);
                }
            }

            pagingServices.Filter = expr;
            int totalCount = 0;

            var response = TDSService.GetTdsempDeductionByPaging(out totalCount, pagingServices);

            string Reportname = "TDS_Investment_Report_" + response.FirstOrDefault().U.AttendenceId + "";
            int subsheet = 0;
            List<ExportExcelColumn> excelColumn = new List<ExportExcelColumn>();

            excelColumn.Add(new ExportExcelColumn { ColumnName = "Sr. No.", PropertyName = "SrNo", ColumnWidth = 1000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Employee Name", PropertyName = "Employee Name", ColumnWidth = 10000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Attendence Id", PropertyName = "Attendence Id", ColumnWidth = 1000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Email", PropertyName = "Email", ColumnWidth = 80000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Updated By Employee", PropertyName = "Updated By Employee", ColumnWidth = 5000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Update By Employer", PropertyName = "Update By Employer", ColumnWidth = 5000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Updated By Employer", PropertyName = "Updated By Employer", ColumnWidth = 3500 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Declarationsheet Date", PropertyName = "Declarationsheet Date", ColumnWidth = 3500 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Lock/Unlock", PropertyName = "Lock/Unlock", ColumnWidth = 3500 });

            var memoryStream = ToExportToExcel(response, subsheet, Reportname, excelColumn);

            return File(memoryStream.ToArray(), "application/vnd.ms-excel", Reportname.Trim().Replace(" ", "_") + "_" + DateTime.Now.ToString("hh_mm_ss_tt") + ".xls");

        }

        public static MemoryStream ToExportToExcel(List<TdsempDeduction> lsObj, int isSubsheet, string Reportname, List<ExportExcelColumn> excelColumn)
        {
            MemoryStream response = new MemoryStream();
            if (lsObj != null && lsObj.Count() > 0)
            {
                bool columnFlag = false;
                List<string> props = new List<string>();
                List<string> childprops = new List<string>();
                //Get the column names of Employee Birthday Data
                if (excelColumn != null && excelColumn.Count > 0)
                {
                    columnFlag = true;
                    props = excelColumn.Select(s => s.PropertyName.Trim()).ToList();
                }

                //Get the column names of Employee Birthday Data        
                if (props != null && props.Count > 0)
                {
                    var workbook = new HSSFWorkbook();
                    var headerLabelCellStyle = workbook.CreateCellStyle();
                    headerLabelCellStyle.Alignment = HorizontalAlignment.LEFT;
                    headerLabelCellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    headerLabelCellStyle.BorderBottom = CellBorderType.THIN;
                    headerLabelCellStyle.WrapText = true;

                    var headerLabelFont = workbook.CreateFont();
                    headerLabelFont.Boldweight = (short)FontBoldWeight.BOLD;
                    headerLabelFont.Color = HSSFColor.BLACK.index;
                    headerLabelCellStyle.SetFont(headerLabelFont);

                    var headerCellStyle = workbook.CreateCellStyle();
                    headerCellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    headerLabelFont.FontHeightInPoints = 11;
                    headerCellStyle.SetFont(headerLabelFont);

                    var sheet = workbook.CreateSheet(Reportname);
                    var attendeeLabelCellStyle = workbook.CreateCellStyle();

                    attendeeLabelCellStyle.Alignment = HorizontalAlignment.LEFT;
                    attendeeLabelCellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    attendeeLabelCellStyle.BorderBottom = CellBorderType.THIN;
                    attendeeLabelCellStyle.WrapText = true;
                    attendeeLabelCellStyle.FillForegroundColor = HSSFColor.GREY_50_PERCENT.index;
                    attendeeLabelCellStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
                    var attendeeLabelFont = workbook.CreateFont();
                    attendeeLabelFont.Boldweight = (short)FontBoldWeight.BOLD;
                    attendeeLabelFont.Color = HSSFColor.WHITE.index;
                    attendeeLabelCellStyle.SetFont(attendeeLabelFont);

                    //Create a header row
                    var headerRow = sheet.CreateRow(0);
                    //Set the column names in the header row

                    var count = 0;
                    foreach (var item in props)
                    {
                        headerRow.CreateCell(count, CellType.STRING).SetCellValue(
                         columnFlag ? excelColumn.Where(x => x.PropertyName == item).Select(x => x.ColumnName).FirstOrDefault() : item);
                        count = count + 1;
                    }

                    for (int i = 0; i < headerRow.LastCellNum; i++)
                    {
                        headerRow.GetCell(i).CellStyle = attendeeLabelCellStyle;
                        headerRow.Height = 350;
                    }

                    //(Optional) freeze the header row so it is not scrolled
                    sheet.CreateFreezePane(0, 1, 0, 1);

                    int rowNumber = 1; // Index of Row for data

                    var rowCellStyleattendeeothercolumn = workbook.CreateCellStyle();
                    rowCellStyleattendeeothercolumn.VerticalAlignment = VerticalAlignment.CENTER;

                    var otherattendeeLabelFont = workbook.CreateFont();
                    otherattendeeLabelFont.FontHeightInPoints = 9;
                    rowCellStyleattendeeothercolumn.SetFont(otherattendeeLabelFont);

                    for (int i = 0; i < lsObj.Count(); i++)
                    {
                        var row = sheet.CreateRow(rowNumber);

                        count = 0;
                        row.CreateCell(count++).SetCellValue(i + 1); // Sr. No.
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].U.Name));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].U.AttendenceId));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].U.EmailOffice));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].ModifyDateEmp != null ? lsObj[i].ModifyDateEmp.Value.ToFormatDateString("MMM dd, yyyy hh:mm:ss") : string.Empty));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].ModifyByEmpNavigation.Name));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].ModifyDateAc != null ? lsObj[i].ModifyDateAc.Value.ToFormatDateString("MMM dd, yyyy hh:mm:ss") : string.Empty));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].TdsdeductionDoc.Count() > 0 ? (lsObj[i].TdsdeductionDoc.FirstOrDefault().ModifyDate == null ? lsObj[i].TdsdeductionDoc.FirstOrDefault().CreatedDate.ToFormatDateString("MMM dd, yyyy hh:mm:ss") : lsObj[i].TdsdeductionDoc.FirstOrDefault().ModifyDate.Value.ToFormatDateString("MMM dd, yyyy hh:mm:ss")) : ""));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].IsLockUnlock == true ? "Lock" : "Unlock"));

                        rowNumber++;

                        for (int k = 0; k < headerRow.LastCellNum; k++)
                        {
                            string columnName = headerRow.GetCell(k).ToString();
                            int Width = 2000;
                            if (k == 1)
                            {
                                Width = 6000;
                            }
                            else if (k == 2)
                            {
                                Width = 3500;
                            }
                            else if (k == 3)
                            {
                                Width = 9000;
                            }
                            else if (k > 3)
                            {
                                Width = 6000;
                            }
                            sheet.SetColumnWidth(k, Width);
                        }
                        //for (int k = 4; k < headerRow.LastCellNum; k++)
                        //{
                        //    if (row != null)
                        //    {
                        //        string columnName = row.GetCell(k).ToString();
                        //        sheet.SetColumnWidth(k, 8000);
                        //    }
                        //}

                        row.Height = 350;
                    }
                    workbook.Write(response);
                    //Return the result to the end user
                }
            }
            return response;
        }

        #region Delete Investment Detail
        [Route("DeleteInvestment")]
        [HttpPost]
        public IActionResult DeleteInvestment(int Id)
        {
            string Message = string.Empty;
            try
            {
                if (Id > 0)
                {
                    var tdsDoc = TDSService.DeleteTdsdeductionDoc(Id);
                    var tdsEmpDeduction = TDSService.DeleteTdsEmpDeductiondoc(Id);
                    if (!tdsEmpDeduction && !tdsDoc)
                    {
                        Message = "some error occurred";
                    }
                    else
                    {
                        Message = "Investment deleted successfully.";
                    }
                }
                else
                {
                    Message = "some error occurred";
                }
                return NewtonSoftJsonResult(new RequestOutcome<string> { Data = Message, IsSuccess = true });
            }
            catch (Exception Ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Data = Ex.Message, IsSuccess = false });
            }
        }
        #endregion

    }

}

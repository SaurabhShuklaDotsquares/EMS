using Castle.Core.Internal;
using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Dto.SME;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using EMS.Web.Controllers;
using EMS.Web.Models.Others;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using NPOI.HSSF.Record.Formula.Functions;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Row = NPOI.SS.UserModel.Row;

namespace EMS.Website.Controllers
{
    [CustomAuthorization()]
    public class SMEController : BaseController
    {

        #region "Fields and Constructor"
        private readonly ILoginService userLoginService;
        private readonly ISubjectMasterExpertService subjectmasterService;
        #endregion
        public SMEController(ILoginService _userMasterService, ISubjectMasterExpertService _subjectmasterService)
        {
            this.userLoginService = _userMasterService;
            this.subjectmasterService = _subjectmasterService;

        }

        [HttpGet]
        public IActionResult Index(int id)
        {
            var viewModel = new SmeDto();

            viewModel.Level1Options = subjectmasterService.GetUsers1(CurrentUser.PMUid)
                            .Select(x => new SelectListItem { Value = x.Uid.ToString(), Text = $"{x.Name} ({x.EmailOffice})" })
                            .ToList();

            viewModel.Level2Options = subjectmasterService.GetUsers1(CurrentUser.PMUid)
                          .Select(x => new SelectListItem
                          {
                              Value = x.Uid.ToString(),
                              Text = $"{x.Name} ({x.EmailOffice})"
                          })
                          .ToList();



            viewModel.Level3Options = subjectmasterService.GetUsers1(CurrentUser.PMUid)
                          .Select(x => new SelectListItem { Value = x.Uid.ToString(), Text = $"{x.Name} ({x.EmailOffice})" })
                          .ToList();


            viewModel.Level4Options = subjectmasterService.GetUsers1(CurrentUser.PMUid)
                         .Select(x => new SelectListItem { Value = x.Uid.ToString(), Text = $"{x.Name} ({x.EmailOffice})" })
                         .ToList();

            viewModel.Level5Options = subjectmasterService.GetUsers1(CurrentUser.PMUid)
                       .Select(x => new SelectListItem { Value = x.Uid.ToString(), Text = $"{x.Name} ({x.EmailOffice})" })
                       .ToList();

            viewModel.datalist = subjectmasterService.GetSubjectMasterExpertData();
            if (id != 0)
            {
                var existingSme = subjectmasterService.GetSmeById(id);
                if (existingSme != null)
                {
                    viewModel.Id = existingSme.Id;
                    viewModel.SubjectMatterExpert = existingSme.SubjectMatterExpert;
                    viewModel.Level1 = existingSme.Level1;
                    viewModel.Level2 = existingSme.Level2;
                    viewModel.Level3 = existingSme.Level3;
                    viewModel.Level4 = existingSme.Level4;
                    viewModel.Level5 = existingSme.Level5;                  
                    viewModel.IsActive = (bool)existingSme.IsActive;
                }
                
            }

            else
            {
                viewModel.IsActive = true;
            }

            return View(viewModel);
        }


        [HttpPost]
        public IActionResult SaveSME( SmeDto model)
        {
            try
            {
                if (model == null || !ModelState.IsValid)
                {
                    return View("Index", model);
                }
                var smeEntity = new Sme
                {
                    SubjectMatterExpert = model.SubjectMatterExpert == null ? "" : model.SubjectMatterExpert,
                    Level1 = model.Level1,
                    Level2 = model.Level2,
                    Level3 = model.Level3,
                    Level4 = model.Level4,
                    Level5 = model.Level5,
                    CreatedDate = DateTime.Now,
                    IsActive = model.IsActive,
                };

                if (model.Id != 0)
                {
                    var existingSme = subjectmasterService.GetSmeById(model.Id);
                    if (existingSme != null)
                    {
                        existingSme.SubjectMatterExpert = smeEntity.SubjectMatterExpert == null ? "" : smeEntity.SubjectMatterExpert;
                        existingSme.Level1 = smeEntity.Level1;
                        existingSme.Level2 = smeEntity.Level2;
                        existingSme.Level3 = smeEntity.Level3;
                        existingSme.Level4 = smeEntity.Level4;
                        existingSme.Level5 = smeEntity.Level5;
                        existingSme.ModifiedDate = DateTime.Now;
                        existingSme.IsActive = smeEntity.IsActive;

                        subjectmasterService.SaveEdit(existingSme);
                        ShowSuccessMessage("Success!", "Record has been updated successfully !!", false);
                    }
                    else
                    {
                        ModelState.AddModelError("", "SME not found for update.");
                    }
                }
                else
                {
                    subjectmasterService.SaveEdit(smeEntity);
                    ShowSuccessMessage("Success!", "Record has been saved successfully !!", false);
                }

                return RedirectToAction("ShowSMEData");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving data: " + ex.Message);
                return View("Index", model);
            }
        }







        [HttpGet]
        public IActionResult GetSmeData(string sme)
        {
            Sme existingSme = subjectmasterService.GetSmeDataByExpert(sme.ToLower());
            if (existingSme == null)
            {
                return Json(new { success = false });
            }

            return Json(new { success = true, data = existingSme });
        }


        [HttpGet]
        public IActionResult ShowSMEData()
        {
            ViewBag.SubjectMatterList = subjectmasterService.GetSubjectMatterList().OrderBy(x => x.SubjectMatterExpert).Select(n => new SelectListItem { Text = n.SubjectMatterExpert, Value = n.Id.ToString() }).ToList();


            return View();
        }

        [HttpPost]
        public IActionResult GetAllSmeData(IDataTablesRequest request, int[] SubjectMatter)
        {

            var pagingServices = new PagingService<Sme>(request.Start, request.Length);
            bool IsActiveOnly = true;
            var expr = PredicateBuilder.True<Sme>();

            if (SubjectMatter != null && SubjectMatter.Length > 0)
            {
                var subjectMatterIds = SubjectMatter.Select(x => x.ToString()).ToArray();
                expr = expr.And(e => subjectMatterIds.Contains(e.Id.ToString()));
            }
            //if (IsActiveOnly)
            //{
            //    expr = expr.And(e => (bool)e.IsActive);
            //}

            pagingServices.Filter = expr;
            pagingServices.Sort = (o) =>
            {
                return o.OrderBy(c => c.SubjectMatterExpert);
            };
            int totalCount = 0;
            var response = subjectmasterService.GetAllSmeData(out totalCount, pagingServices);

            // Fetch user names for Level1, Level2, and Level3
            var userIds = response.Select(r => new[] { r.Level1, r.Level2, r.Level3, r.Level4, r.Level5 }).SelectMany(x => x).Distinct().ToArray();
            var userNames = subjectmasterService.GetUsersByIds(userIds);

            var transformedResponse = response.Select((r, index) => new
            {
                rowIndex = (index + 1) + (request.Start),
                ID = r.Id,
                subjectmatter = r.SubjectMatterExpert,
                Level1 = GetUserDisplayName(r.Level1, userNames),
                Level2 = GetUserDisplayName(r.Level2, userNames),
                Level3 = GetUserDisplayName(r.Level3, userNames),
                Level4 = GetUserDisplayName(r.Level4, userNames),
                Level5 = GetUserDisplayName(r.Level5, userNames),
                RunningStatus = r.IsActive.Value ? "Active" : "InActive"
            });

            return DataTablesJsonResult(totalCount, request, transformedResponse);
        }

        private string GetUserDisplayName(int? userId, IEnumerable<UserLogin> users)
        {
            var user = users.FirstOrDefault(u => u.Uid == userId);
            if (user != null)
            {
                return user.Name;
            }
            return userId.ToString();
        }


        [HttpGet]
        public ActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this record?",
                Size = Enums.ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Subject Matter?" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        public IActionResult delete(int id)
        {
            Sme data = subjectmasterService.GetSmeById(id);
            if (data != null)
            {
                subjectmasterService.Delete(data);
            }
            ShowSuccessMessage("Success", "Record has been successfully deleted", false);
            return RedirectToAction("ShowSMEData");
        }

        #region SME Report 
        [HttpGet]
        public ActionResult ExportReportToExcel(string SubjectMatter)
        {
            bool IsActiveOnly = true;
            var expr = PredicateBuilder.True<Sme>();
            string[] arr = SubjectMatter.Split(',');

            if (SubjectMatter != null && SubjectMatter.Length > 0)
            {
                expr = expr.And(e => arr.Contains(e.Id.ToString()));
            }
            //if (IsActiveOnly)
            //{
            //    expr = expr.And(e => (bool)e.IsActive);
            //}

            //pagingServices.Filter = expr;
            //pagingServices.Sort = (o) =>
            //{
            //    return o.OrderByDescending(c => c.ModifiedDate);
            //};

            int totalCount = 0;
            var response = new List<Sme>();

            response = subjectmasterService.GetAllSmeDatas(SubjectMatter);
            
            var userIds = response.Select(r => new[] { r.Level1, r.Level2, r.Level3, r.Level4, r.Level5 }).SelectMany(x => x).Distinct().ToArray();
            var userNames = subjectmasterService.GetUsersByIds(userIds);
            string Reportname = "SME_Report";
            int subsheet = 0;
            List<ExportExcelColumn> excelColumn = new List<ExportExcelColumn>();

            excelColumn.Add(new ExportExcelColumn { ColumnName = "Sr. No.", PropertyName = "SrNo", ColumnWidth = 1000 });
            //excelColumn.Add(new ExportExcelColumn { ColumnName = "Status", PropertyName = "IsActive", ColumnWidth = 5000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Subject", PropertyName = "Subject", ColumnWidth = 20000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Expert 1", PropertyName = "Expert1", ColumnWidth = 20000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Expert 2", PropertyName = "Expert2", ColumnWidth = 20000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Expert 3", PropertyName = "Expert3", ColumnWidth = 20000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Expert 4", PropertyName = "Expert4", ColumnWidth = 20000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Expert 5", PropertyName = "Expert5", ColumnWidth = 20000 });

            var memoryStream = ToExportToExcel(response, subsheet, Reportname, excelColumn);

            return File(memoryStream.ToArray(), "application/vnd.ms-excel", Reportname.Trim().Replace(" ", "_") + "_" + DateTime.Now.ToString("hh_mm_ss_tt") + ".xls");
        }
        public  MemoryStream ToExportToExcel(List<Sme> lsObj, int isSubsheet, string Reportname, List<ExportExcelColumn> excelColumn)
        {
            var userIds = lsObj.Select(r => new[] { r.Level1, r.Level2, r.Level3, r.Level4, r.Level5 }).SelectMany(x => x).Distinct().ToArray();
            var userNames = subjectmasterService.GetUsersByIds(userIds);


            MemoryStream response = new MemoryStream();
            if (lsObj != null && lsObj.Count() > 0)
            {
                bool columnFlag = false;
                List<string> props = new List<string>();
                List<string> childprops = new List<string>();
                //Get the column names of Employee  Data
                if (excelColumn != null && excelColumn.Count > 0)
                {
                    columnFlag = true;
                    props = excelColumn.Select(s => s.PropertyName.Trim()).ToList();
                }

                //Get the column names of Employee Relative Data        
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
                        //if (lsObj[i].IsActive == true)
                        //{
                        //    row.CreateCell(count++).SetCellValue(Convert.ToString("Active"));
                        //}
                        //else
                        //{
                        //    row.CreateCell(count++).SetCellValue(Convert.ToString("In Active"));
                        //}
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].SubjectMatterExpert));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(GetUserDisplayName(lsObj[i].Level1, userNames)));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(GetUserDisplayName(lsObj[i].Level2, userNames)));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(GetUserDisplayName(lsObj[i].Level3, userNames)));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(GetUserDisplayName(lsObj[i].Level4, userNames)));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(GetUserDisplayName(lsObj[i].Level5, userNames)));

                        rowNumber++;
                        Row childRow = null;
                        row.Height = 350;
                    }
                    workbook.Write(response);
                    //Return the result to the end user
                }
            }
            return response;
        }
        #endregion


    }
}


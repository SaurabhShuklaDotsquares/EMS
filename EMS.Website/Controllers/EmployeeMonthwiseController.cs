using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using EMS.Web.Controllers;
using EMS.Web.Modals;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace EMS.Website.Controllers
{    
    [CustomAuthorization()]
    public class EmployeeMonthwiseController : BaseController
    {        
        private readonly IUserLoginService _userService;
        private readonly IEmployeeMonthwiseService _employeeMonthwiseService;

        public EmployeeMonthwiseController(IUserLoginService userService, IEmployeeMonthwiseService employeeMonthwiseService)
        {
            _userService = userService;
            _employeeMonthwiseService= employeeMonthwiseService;
        }

        [CustomActionAuthorization]
        public ActionResult Index()
        {
            DateTime now = DateTime.Today;
            List<SelectListItem> MonthYear = new List<SelectListItem>();            
            for (int i = 0; i < 12; i++)
            {
                string Month = now.ToString("MMM") + " " + now.Year.ToString();
                MonthYear.Add(new SelectListItem { Text = Month, Value = Month });
                now = now.AddMonths(-1);
            }
            MonthYear.Insert(0, new SelectListItem() { Text = "Select Month" });
            ViewBag.MonthYear = MonthYear.ToList();           
            return View();
        }

        public IActionResult GetList(DataTables.AspNet.Core.IDataTablesRequest request, MedicalDataSearchFilter searchFilter)
        {            
            DateTime now = Convert.ToDateTime(searchFilter.dateFrom);
            var startDate = new DateTime(now.Year, now.Month, 1);
            var CurrentMonthLastDate = startDate.AddMonths(1).AddDays(-1);

            var LastMonthLastDate = CurrentMonthLastDate.AddDays(0 - CurrentMonthLastDate.Day);
            var NextMonthFirstDate = new DateTime(CurrentMonthLastDate.Year, CurrentMonthLastDate.Month + 1, 1);

            string JDate = CurrentMonthLastDate.ToString("yyyy-MM-dd");
            string RDate = LastMonthLastDate.ToString("yyyy-MM-dd");
            string NJDate = NextMonthFirstDate.ToString("yyyy-MM-dd");

            //string JDate = "2023-02-28 00:00:00.00";
            //string RDate = "2023-01-31";
            //string NJDate = "2023-03-01";
            int totalCount = 0;
            var response = _employeeMonthwiseService.GetEmployeeListMonthWise(JDate, RDate, NJDate);
            totalCount= response.Count();
            return DataTablesJsonResult(totalCount, request, response.Select((x, index) => new {
                rowId = request.Start + index + 1,
                IsActive = x.IsActive,
                JoinedDate = x.JoinedDate,
                RelievingDate = x.RelievingDate,
                AttendenceId = x.AttendenceId,
                uid = x.uid,
                Name = x.Name,
                JobTitle = x.JobTitle,
                DepartmentName = x.DepartmentName,
                PMName = x.PMName
            }));

        }


        #region EmployeeMonthwise Report 
        [HttpGet]
        public ActionResult ExportReportToExcel(string MonthYear)
        {
            DateTime now = Convert.ToDateTime(MonthYear);
            var startDate = new DateTime(now.Year, now.Month, 1);
            var CurrentMonthLastDate = startDate.AddMonths(1).AddDays(-1);

            var LastMonthLastDate = CurrentMonthLastDate.AddDays(0 - CurrentMonthLastDate.Day);
            var NextMonthFirstDate = new DateTime(CurrentMonthLastDate.Year, CurrentMonthLastDate.Month + 1, 1);

            string JDate = CurrentMonthLastDate.ToString("yyyy-MM-dd");
            string RDate = LastMonthLastDate.ToString("yyyy-MM-dd");
            string NJDate = NextMonthFirstDate.ToString("yyyy-MM-dd");
            List<EmployeeMonthwiseModel> responseReport = _employeeMonthwiseService.GetEmployeeListMonthWise(JDate, RDate, NJDate);

            string Reportname = "Monthly_Employees_Report" + MonthYear;
            int subsheet = 0;
            List<ExportExcelColumn> excelColumn = new List<ExportExcelColumn>();

            excelColumn.Add(new ExportExcelColumn { ColumnName = "Sr. No.", PropertyName = "SrNo", ColumnWidth = 1000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Status", PropertyName = "IsActive", ColumnWidth = 5000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Joined Date", PropertyName = "JoinedDate" });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Relieving Date", PropertyName = "RelievingDate"});
            excelColumn.Add(new ExportExcelColumn { ColumnName = "AttendenceId", PropertyName = "AttendenceId" });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "UID", PropertyName = "uid" });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Name", PropertyName = "Name" });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Job Title", PropertyName = "JobTitle" });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Department", PropertyName = "DepartmentName" });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "PM Name", PropertyName = "PMName" });
            
            var memoryStream = ToExportToExcel(responseReport, subsheet, Reportname, excelColumn);

            return File(memoryStream.ToArray(), "application/vnd.ms-excel", Reportname.Trim().Replace(" ", "_") + "_" + DateTime.Now.ToString("hh_mm_ss_tt") + ".xls");
        }
        public static MemoryStream ToExportToExcel(List<EmployeeMonthwiseModel> lsObj, int isSubsheet, string Reportname, List<ExportExcelColumn> excelColumn)
        {
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
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].IsActive == "True" ? "Active" : "In Active"));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].JoinedDate));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].RelievingDate));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].AttendenceId));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].uid));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].Name));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].JobTitle));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].DepartmentName));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].PMName));

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

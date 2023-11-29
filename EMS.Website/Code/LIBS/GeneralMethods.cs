using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Web.LIBS;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;

namespace EMS.Web.Code.LIBS
{
    public static class GeneralMethods
    {
        public static MemoryStream ToExportToExcel<T>(List<T> source,
            string reportName, int isSubsheet, List<ExportExcelColumn> excelColumn = null)
        {
            var memoryStream = new MemoryStream();

            if (source != null && source.Any())
            {
                bool hasColumns = false;

                List<PropertyInfo> properties = typeof(T).GetProperties().ToList();
                if (excelColumn != null && excelColumn.Any())
                {
                    hasColumns = true;
                    properties = properties.Where(x => excelColumn.Select(s => s.PropertyName.Trim().ToLower()).Contains(x.Name.Trim().ToLower())).ToList();
                }

                if (properties != null && properties.Count > 0)
                {
                    var workbook = new HSSFWorkbook();
                    CellStyle headerLabelCellStyle = workbook.CreateCellStyle();
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

                    var workSheet = workbook.CreateSheet(reportName);
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
                    var headerRow = workSheet.CreateRow(0);
                    //Set the column names in the header row

                    var count = 0;
                    foreach (var item in properties)
                    {
                        headerRow.CreateCell(count, CellType.STRING).SetCellValue(
                         hasColumns ? excelColumn.Where(x => x.PropertyName == item.Name).Select(x => x.ColumnName).FirstOrDefault() : item.Name);
                        count = count + 1;
                    }

                    for (int i = 0; i < headerRow.LastCellNum; i++)
                    {
                        headerRow.GetCell(i).CellStyle = attendeeLabelCellStyle;
                        headerRow.Height = 350;
                    }

                    //(Optional) freeze the header row so it is not scrolled
                    workSheet.CreateFreezePane(0, 1, 0, 1);

                    int rowNumber = 1;

                    var rowCellStyleattendeeothercolumn = workbook.CreateCellStyle();
                    rowCellStyleattendeeothercolumn.VerticalAlignment = VerticalAlignment.CENTER;
                    rowCellStyleattendeeothercolumn.BorderTop = CellBorderType.THIN;
                    rowCellStyleattendeeothercolumn.BorderBottom = CellBorderType.THIN;
                    rowCellStyleattendeeothercolumn.BorderLeft = CellBorderType.THIN;
                    rowCellStyleattendeeothercolumn.BorderRight = CellBorderType.THIN;
                    var otherattendeeLabelFont = workbook.CreateFont();
                    otherattendeeLabelFont.FontHeightInPoints = 9;
                    rowCellStyleattendeeothercolumn.SetFont(otherattendeeLabelFont);

                    for (int i = 0; i < source.Count(); i++)
                    {
                        var row = workSheet.CreateRow(rowNumber++);
                        count = 0;
                        foreach (var item in properties)
                        {
                            row.CreateCell(count).SetCellValue(Convert.ToString(item.GetValue(source[i], null)));
                            count = count + 1;
                        }

                        row.Height = 350;
                        for (int j = 0; j < headerRow.LastCellNum; j++)
                        {
                            row.GetCell(j).CellStyle = rowCellStyleattendeeothercolumn;
                            string columnName = headerRow.GetCell(j).ToString();
                            workSheet.SetColumnWidth(j, hasColumns ? excelColumn.Where(x => x.ColumnName == columnName).Select(x => x.ColumnWidth).FirstOrDefault() : 5000);
                        }
                    }

                    workbook.Write(memoryStream);
                }
            }

            return memoryStream;
        }

        public static string Getip()
        {
            string ipAddress = ContextProvider.HttpContext.Connection.RemoteIpAddress.ToString();
            string hostname = Dns.GetHostName();
            IPHostEntry hostentry = Dns.GetHostEntry(hostname);
            IPAddress[] ipList = hostentry.AddressList;
            if (ipList.Any())
                ipAddress = Convert.ToString(ipList.FirstOrDefault(i => i.ToString().Split('.')[0] == "192"));

            return ipAddress;
        }
        public static void LibraryLogWriter(string fileName, string log)
        {
            string filePath = $"{Path.Combine("wwwroot/Upload/logs")}\\{fileName}_{DateTime.Today.ToFormatDateString("dd-MM-yyyy")}.txt";

            try
            {
                using (StreamWriter objWrite = System.IO.File.Exists(filePath) ? System.IO.File.AppendText(filePath) : System.IO.File.CreateText(filePath))
                {
                    objWrite.WriteLine($"{log}\n");

                    objWrite.Flush();
                    objWrite.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static void LogWriter(string fileName, string log)
        {
            if (!string.IsNullOrEmpty(SiteKey.LogFilePath))
            {
                FileStream fileStream = null;
                DirectoryInfo directoryInfo;
                FileInfo fileInfo;

                string filePath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath + SiteKey.LogFilePath, $"{fileName}.txt");

                fileInfo = new FileInfo(filePath);
                directoryInfo = new DirectoryInfo(fileInfo.DirectoryName);

                if (!directoryInfo.Exists)
                    directoryInfo.Create();

                if (!fileInfo.Exists)
                    fileStream = fileInfo.Create();
                else
                    fileStream = new FileStream(filePath, FileMode.Append);

                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.WriteLine(log);
                streamWriter.Close();
            }
        }

        public static MemoryStream ExportToExcelLeads(List<ProjectLead> source, string Reportname, int userId, int roleId)
        {
            var memoryStream = new MemoryStream();

            try
            {
                if (source != null && source.Any())
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
                    headerRow.CreateCell(0, CellType.STRING).SetCellValue("EMS LeadID");
                    headerRow.CreateCell(1, CellType.STRING).SetCellValue("CRM LeadID");
                    headerRow.CreateCell(2, CellType.STRING).SetCellValue("Country");
                    headerRow.CreateCell(3, CellType.STRING).SetCellValue("Title");
                    headerRow.CreateCell(4, CellType.STRING).SetCellValue("Generated Date");
                    headerRow.CreateCell(5, CellType.STRING).SetCellValue("Estimated Time");
                    headerRow.CreateCell(6, CellType.STRING).SetCellValue("Estimate Owner");
                    headerRow.CreateCell(7, CellType.STRING).SetCellValue("Comunication Owner");
                    headerRow.CreateCell(8, CellType.STRING).SetCellValue("Technologies");
                    headerRow.CreateCell(9, CellType.STRING).SetCellValue("Source");
                    headerRow.CreateCell(10, CellType.STRING).SetCellValue("Status");
                    headerRow.CreateCell(11, CellType.STRING).SetCellValue("Remark");
                    headerRow.CreateCell(12, CellType.BOOLEAN).SetCellValue("Is Covid19");
                    headerRow.CreateCell(13, CellType.STRING).SetCellValue("CRM Notes");

                    for (int i = 0; i < headerRow.LastCellNum; i++)
                    {
                        headerRow.GetCell(i).CellStyle = attendeeLabelCellStyle;
                        headerRow.Height = 350;
                    }

                    //(Optional) freeze the header row so it is not scrolled
                    sheet.CreateFreezePane(0, 1, 0, 1);

                    int rowNumber = 1;

                    var rowCellStyleattendeeothercolumn = workbook.CreateCellStyle();
                    rowCellStyleattendeeothercolumn.VerticalAlignment = VerticalAlignment.CENTER;
                    rowCellStyleattendeeothercolumn.BorderTop = CellBorderType.THIN;
                    rowCellStyleattendeeothercolumn.BorderBottom = CellBorderType.THIN;
                    rowCellStyleattendeeothercolumn.BorderLeft = CellBorderType.THIN;
                    rowCellStyleattendeeothercolumn.BorderRight = CellBorderType.THIN;
                    var otherattendeeLabelFont = workbook.CreateFont();
                    otherattendeeLabelFont.FontHeightInPoints = 9;
                    rowCellStyleattendeeothercolumn.SetFont(otherattendeeLabelFont);
                    //rowCellStyleattendeeothercolumn.WrapText = true;

                    // CellStyle style = workbook.CreateCellStyle();
                    //style.setWrapTex
                    // cell1.setCellStyle(style);

                    for (int i = 0; i < source.Count(); i++)
                    {
                        var row = sheet.CreateRow(rowNumber++);

                        row.CreateCell(0).SetCellValue(Convert.ToString(source[i].LeadId));
                        row.CreateCell(1).SetCellValue(!String.IsNullOrEmpty(Convert.ToString(source[i].LeadCRMId)) ? Convert.ToString(source[i].LeadCRMId) : "");
                        row.CreateCell(2).SetCellValue(source[i].AbroadPM != null ? Convert.ToString(source[i].AbroadPM.Country) : "");
                        row.CreateCell(3).SetCellValue(Convert.ToString(source[i].Title));
                        row.CreateCell(4).SetCellValue(Convert.ToDateTime(source[i].AddDate).ToString("MMM, dd yyyy hh:mm tt"));
                        row.CreateCell(5).SetCellValue(WeekAndDay(Convert.ToInt32(source[i].EstimateTimeinDay)));
                        row.CreateCell(6).SetCellValue(Convert.ToString(source[i].UserLogin1.Name));
                        row.CreateCell(7).SetCellValue(source[i].UserLogin != null ? Convert.ToString(source[i].UserLogin.Name) : "");
                        row.CreateCell(8).SetCellValue(Convert.ToString(source[i].Technologies));
                        row.CreateCell(9).SetCellValue(source[i].AbroadPM != null ? Convert.ToString(source[i].AbroadPM.Name) : "");
                        string txt = "";
                        txt = (source[i].LeadStatu.StatusName.ToString().ToLower().ContainsAny("action", "chase") ?
                            (roleId == (int)Enums.UserRoles.PM || (Convert.ToString(source[i].LeadStatu.StatusName).Equals("Action Required From (Team)")
                            && userId == Convert.ToInt32(source[i].OwnerId)) ? Convert.ToString(source[i].LeadStatu.StatusName) :
                            Convert.ToString(source[i].LeadStatu.StatusName)).Replace("Team", Convert.ToString(source[i].UserLogin1.Name)).Replace
                            ("Out of India PM", (source[i].AbroadPM != null ? Convert.ToString(source[i].AbroadPM.Name) : "") + " - " +
                            (source[i].AbroadPM != null ? Convert.ToString(source[i].AbroadPM.Country) : "")) :
                            Convert.ToString(source[i].Conclusion)).Replace("(", "\n(").Replace("(", "").Replace(")", "");

                        row.CreateCell(10).SetCellValue(txt);
                        row.CreateCell(11).SetCellValue(!String.IsNullOrEmpty(source[i].Remark) ? source[i].Remark : "");
                        row.CreateCell(12).SetCellValue(source[i].IsCovid19.HasValue ? source[i].IsCovid19.Value : false);
                        var crmNotes = LeadNotes(Convert.ToInt32(source[i].LeadCRMId));
                        row.CreateCell(13).SetCellValue(crmNotes.LeadNoteList.Count > 0 ? (crmNotes.LeadNoteList.OrderBy(x => x.SNo).Take(1).Select(x => x.notes_details).ToList().FirstOrDefault()).ToString() : string.Empty);
                        row.Height = 350;
                        for (int j = 0; j < headerRow.LastCellNum; j++)
                        {
                            row.GetCell(j).CellStyle = rowCellStyleattendeeothercolumn;
                        }
                    }
                    sheet.SetColumnWidth(0, 4000);
                    sheet.SetColumnWidth(1, 4000);
                    sheet.SetColumnWidth(2, 3000);
                    sheet.SetColumnWidth(3, 6000);
                    sheet.SetColumnWidth(4, 5000);
                    sheet.SetColumnWidth(5, 5000);
                    sheet.SetColumnWidth(6, 4000);
                    sheet.SetColumnWidth(7, 6000);
                    sheet.SetColumnWidth(8, 5000);
                    sheet.SetColumnWidth(9, 5000);
                    sheet.SetColumnWidth(10, 10000);
                    sheet.SetColumnWidth(11, 10000);
                    sheet.SetColumnWidth(12, 3000);
                    sheet.SetColumnWidth(13, 20000);

                    workbook.Write(memoryStream);
                }
            }
            catch (Exception ex)
            {
            }

            return memoryStream;
        }
        private static HttpWebResponse GetLeadNotes(int lead_id)
        {
            var request = WebRequest.CreateHttp(SiteKey.CRMApiLeadNotesUrl);
            StringBuilder postData = new StringBuilder();
            postData.Append($"lead_id={lead_id}&");

            request.Headers.Add("userid", SiteKey.CRMApiUser);
            request.Headers.Add("password", SiteKey.CRMApiPassword);

            var data = Encoding.ASCII.GetBytes(postData.ToString());

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            var result = (HttpWebResponse)(request.GetResponse());
            return result;
        }

        private static LeadNotesDto LeadNotes(int leadId)
        {
            LeadNotesDto leadNotesViewModel = new LeadNotesDto();
            if (leadId > 0)
            {
                leadNotesViewModel.LeadId = $"CRM Lead Notes (ID # {leadId})";

                var response = GetLeadNotes(leadId);
                string responseData = "";

                List<LeadNotesJson> leadNotes = new List<LeadNotesJson>();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    responseData = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    if (responseData.HasValue())
                    {
                        if (responseData != null)
                        {
                            Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(responseData);
                            if (Convert.ToBoolean(obj["Status"]))
                            {
                                String json = null;
                                //json = JsonConvert.SerializeObject(obj.SelectToken("Data.notesInfo.response.result"));
                                json = JsonConvert.SerializeObject(obj.SelectToken("Data.response"));
                                leadNotes = JsonConvert.DeserializeObject<List<LeadNotesJson>>(json);

                                leadNotesViewModel.LeadNoteList = leadNotes.Select((x, index) => new LeadNote
                                {
                                    SNo = index + 1,
                                    lead_id = x.lead_id,
                                    notes_details = x.notes_details,
                                    //notes_time = x.notes_time,
                                    //notes_time = UnixTimeStampToDateTime(Convert.ToDouble(x.notes_time)).ToFormatDateString("MMM dd yyyy hh:mm tt"),
                                    user_name = x.user_name,
                                }).ToList();
                            }
                        }
                    }
                }
            }
            return leadNotesViewModel;
        }
        private static string WeekAndDay(int noofday)
        {
            int weeks = noofday / 5;
            int days = noofday % 5;
            string result = ""; ;
            string weektypes = "Weeks";
            string daytypes = "Days";

            if (days == 1) { daytypes = "Day"; }
            if (weeks == 1) { weektypes = "Week"; }

            if (days == 0) { daytypes = ""; }
            if (weeks == 0) { weektypes = ""; }

            if (weeks == 0)
                result = days + " " + daytypes;
            if (days == 0)
                result = weeks.ToString() + " " + weektypes;
            if (weeks != 0 && days != 0)
                result = weeks.ToString() + " " + weektypes + "  " + days + " " + daytypes;
            if (weeks == 0 && days == 0)
                result = "";

            return result;
        }
        public static string SaveFile(IFormFile FileUpload, string Folder, string prefix)
        {
            if (FileUpload != null && FileUpload.Length > 0)
            {
                string fileName = FileUpload.FileName;
                string ext = Path.GetExtension(fileName.ToLower());
                fileName = Path.GetFileNameWithoutExtension(fileName);
                fileName = fileName.Replace(' ', '-');

                fileName = prefix + fileName + "-" + DateTime.Now.ToString("MM-dd-yyyy" + 'T' + "HH-mm-ss") + ext;

                string FilePath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath + "/" + Folder, fileName);
                using (var stream = new FileStream(FilePath, FileMode.Create))
                {
                    FileUpload.CopyTo(stream);
                }
                // FileUpload..SaveAs(FilePath);

                return fileName;
            }

            return String.Empty;
        }
    }

    public class ExportExcelColumn
    {
        public string ColumnName { get; set; }
        public string PropertyName { get; set; }
        public int ColumnWidth { get; set; }
    }
}
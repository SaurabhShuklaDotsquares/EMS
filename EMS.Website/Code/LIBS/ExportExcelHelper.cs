using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
namespace EMS.Web.Code.LIBS
{
    public class ExportExcelHelper
    {
        #region Fields

        public static string ExcelContentType
        {
            get
            { return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; }
        }

        #endregion       

        #region ToDataTable
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                var descriptions = (DisplayNameAttribute[])prop.GetCustomAttributes(typeof(DisplayNameAttribute), false);
                var propName = prop.Name;
                if (descriptions.Length != 0)
                {
                    propName = descriptions[0].DisplayName;
                }
               
                dataTable.Columns.Add(propName);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
        public static DataTable VaccinationToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                var descriptions = (DisplayNameAttribute[])prop.GetCustomAttributes(typeof(DisplayNameAttribute), false);
                var propName = prop.Name;
                if (descriptions.Length != 0)
                {
                    propName = descriptions[0].DisplayName;
                }

                switch (propName)
                {
                    case "EmployeeName":
                        propName = "Employee Name";
                        break;
                }
                switch (propName)
                {
                    case "ManagerName":
                        propName = "Project Manager";
                        break;
                }
                switch (propName)
                {
                    case "PhoneNumber":
                        propName = "Phone Number";
                        break;
                }
                switch (propName)
                {
                    case "Email":
                        propName = "Email";
                        break;
                }
                switch (propName)
                {
                    case "VaccinationStatus":
                        propName = "Vaccination Status";
                        break;
                }
               
              
                dataTable.Columns.Add(propName);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++){
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        public static DataTable ActivityToDataTable<T>(List<T> items) {
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props) {
                var descriptions = (DisplayNameAttribute[])prop.GetCustomAttributes(typeof(DisplayNameAttribute), false);
                var propName = prop.Name;
                if (descriptions.Length != 0) {
                    propName = descriptions[0].DisplayName;
                }

                switch (propName) {
                    case "Technology_Expert":
                        propName = "Technology (Expert)";
                        break;
                }
                switch (propName) {
                    case "Technology_Intermediate":
                        propName = "Technology (Intermediate)";
                        break;
                }
                switch (propName) {
                    case "Technology_Beginner":
                        propName = "Technology (Beginner)";
                        break;
                }
                switch (propName) {
                    case "Technology_Interested":
                        propName = "Technology (Interested)";
                        break;
                }
                switch (propName) {
                    case "OtherTechnology":
                        propName = "Other Technology";
                        break;
                }
                switch (propName)
                {
                    case "RunningDevelopers":
                        propName = "Number of Running Developers";
                        break;
                }
                switch (propName)
                {
                    case "RunningProjects":
                        propName = "Number of Running Projects";
                        break;
                }

                dataTable.Columns.Add(propName);
            }
            foreach (T item in items) {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++) {
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
        #endregion

        #region Export Excel

        public static byte[] ExportExcel(DataTable dataTable, string heading = "", bool showSrNo = false, params string[] columnsToTake)
        {
            byte[] result = null;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(String.Format("{0} Data", heading));

                ProcessWorkSheet(workSheet, dataTable, heading, showSrNo, columnsToTake);

                result = package.GetAsByteArray();

            }
            return result;
        }
        public static byte[] ExportExcel(DataTable dataTable, string heading = "",bool Stylefooter=false, bool showSrNo = false, params string[] columnsToTake)
        {
            byte[] result = null;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(String.Format("{0} Data", heading));

                ProcessWorkSheet(workSheet, dataTable, heading, Stylefooter, showSrNo, columnsToTake);

                result = package.GetAsByteArray();

            }
            return result;
        }

        public static byte[] ExportExcel(List<ExportExcelData> dataList)
        {
            byte[] result = null;
            using (ExcelPackage package = new ExcelPackage())
            {
                foreach (var data in dataList)
                {
                    ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(String.Format("{0} Data", data.Heading));

                    ProcessWorkSheet(workSheet, data.DataTable, data.Heading, data.ShowSrNo, data.ColumnsToTake);
                }

                result = package.GetAsByteArray();
            }
            return result;
        }


        public static byte[] ActivityExportExcel(DataTable dataTable, string heading = "", bool showSrNo = false, params string[] columnsToTake) {
            byte[] result = null;
            using (ExcelPackage package = new ExcelPackage()) {
                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(String.Format("{0} Data", heading));

                ActivityProcessWorkSheet(workSheet, dataTable, heading, showSrNo, columnsToTake);

                result = package.GetAsByteArray();

            }
            return result;
        }
       
        public static byte[] VaccinationExportExcel(DataTable dataTable, string heading = "", bool showSrNo = false, params string[] columnsToTake)
        {
            byte[] result = null;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(String.Format("{0} Data", heading));

                VaccinationProcessWorkSheet(workSheet, dataTable, heading, showSrNo, columnsToTake);

                result = package.GetAsByteArray();

            }
            return result;
        }


        private static void VaccinationProcessWorkSheet(ExcelWorksheet workSheet, DataTable dataTable, string heading = "", bool showSrNo = false, params string[] columnsToTake)
        {
            int startRowFrom = String.IsNullOrEmpty(heading) ? 1 : 2;
            if (showSrNo)
            {
                DataColumn dataColumn = dataTable.Columns.Add("Sr.No.", typeof(int));
                dataColumn.SetOrdinal(0);
                int index = 1;
                foreach (DataRow item in dataTable.Rows)
                {
                    item[0] = index;
                    index++;
                }
            }
            // add the content into the Excel file  
            workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);

            // autofit width of cells with small content  
            int columnIndex = 1;
            foreach (DataColumn column in dataTable.Columns)
            {
                //column.
                ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];
                int maxLength = columnCells.Max(cell => String.Format("{0}", cell.Value).Length);
                if (maxLength < 150)
                {
                    workSheet.Column(columnIndex).AutoFit();
                }
                columnIndex++;
            }

            // format header - bold, yellow on black  
            using (ExcelRange r = workSheet.Cells[startRowFrom, 1, startRowFrom, dataTable.Columns.Count])
            {
                r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                r.Style.Font.Bold = true;
                r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#1fb5ad"));
            }

            // format cells - add borders  
            using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
            {
                r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
            }

            // removed ignored columns  
            for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
            {
                if (i == 0 && showSrNo)
                {
                    continue;
                }

            }

            if (!String.IsNullOrEmpty(heading))
            {
                workSheet.Cells["A1"].Value = heading;
                workSheet.Cells["A1"].Style.Font.Size = 20;
                workSheet.InsertColumn(1, 1);
                workSheet.InsertRow(1, 1);
                workSheet.Column(1).Width = 5;
            }
        }


        //private static void ProcessWorkSheetOfVaccination(ExcelWorksheet workSheet, DataTable dataTable, string heading = "", bool showSrNo = false, string certificatePath = "", params string[] columnsToTake)
        //{
        //    int startRowFrom = String.IsNullOrEmpty(heading) ? 1 : 2;
        //    if (showSrNo)
        //    {
        //        DataColumn dataColumn = dataTable.Columns.Add("Sr.No.", typeof(int));
        //        dataColumn.SetOrdinal(0);
        //        int index = 1;
        //        foreach (DataRow item in dataTable.Rows)
        //        {
        //            item[0] = index;
        //            index++;
        //        }
        //    }
        //    // add the content into the Excel file  
        //    workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);

        //    // autofit width of cells with small content  
        //    int columnIndex = 1;
        //    foreach (DataColumn column in dataTable.Columns)
        //    {

        //        //var propName = column.ColumnName;
        //        //if(column.ColumnName=="Vaccination Certificate")
        //        //{

        //        //}


        //        //column.
        //        ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];
        //        //if (columnIndex == 5)
        //        //{
        //        //       columnCells.Hyperlink = new Uri("http://www.google.com", UriKind.Absolute);
        //        //       columnCells.Value = columnCells.Value;
                    
        //        //}
        //        int maxLength = columnCells.Max(cell => String.Format("{0}", cell.Value).Length);
        //        if (maxLength < 150)
        //        {
        //            //string FileRootPath = "http://www.google.com";
        //            //workSheet.Cells[5, 1].Formula = "HYPERLINK(\"" + FileRootPath + "\",\"" + DisplayText + "\")";
        //            workSheet.Column(columnIndex).AutoFit();
        //        }
        //        columnIndex++;
        //    }

        //    // format header - bold, yellow on black  
        //    using (ExcelRange r = workSheet.Cells[startRowFrom, 1, startRowFrom, dataTable.Columns.Count])
        //    {
        //        r.Style.Font.Color.SetColor(System.Drawing.Color.White);
        //        r.Style.Font.Bold = true;
        //        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //        r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        //        r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#1fb5ad"));
        //    }

        //    // format cells - add borders  
        //    using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
        //    {
        //        r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //        r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //        r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //        r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //        r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
        //        r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
        //        r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
        //        r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
        //    }

        //    // removed ignored columns  
        //    for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
        //    {
        //        if (i == 0 && showSrNo)
        //        {
        //            continue;
        //        }

        //        if (!columnsToTake.Contains(dataTable.Columns[i].ColumnName))
        //        {
        //            workSheet.DeleteColumn(i + 1);
        //        }
        //    }

        //    if (!String.IsNullOrEmpty(heading))
        //    {
        //        workSheet.Cells["A1"].Value = heading;
        //        workSheet.Cells["A1"].Style.Font.Size = 20;
        //        workSheet.InsertColumn(1, 1);
        //        workSheet.InsertRow(1, 1);
        //        workSheet.Column(1).Width = 5;
        //    }
        //}

       
        private static void ProcessWorkSheet(ExcelWorksheet workSheet, DataTable dataTable, string heading = "", bool showSrNo = false, params string[] columnsToTake)
        {
            int startRowFrom = String.IsNullOrEmpty(heading) ? 1 : 2;
            if (showSrNo)
            {
                DataColumn dataColumn = dataTable.Columns.Add("Sr.No.", typeof(int));
                dataColumn.SetOrdinal(0);
                int index = 1;
                foreach (DataRow item in dataTable.Rows)
                {
                    item[0] = index;
                    index++;
                }
            }
            // add the content into the Excel file  
            workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);

            // autofit width of cells with small content  
            int columnIndex = 1;
            foreach (DataColumn column in dataTable.Columns)
            {
                //column.
                ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];
                int maxLength = columnCells.Max(cell => String.Format("{0}", cell.Value).Length);
                if (maxLength < 150)
                {
                    workSheet.Column(columnIndex).AutoFit();
                }
                columnIndex++;
            }

            // format header - bold, yellow on black  
            using (ExcelRange r = workSheet.Cells[startRowFrom, 1, startRowFrom, dataTable.Columns.Count])
            {
                r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                r.Style.Font.Bold = true;
                r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#1fb5ad"));
            }
           
            // format cells - add borders  
            using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
            {
                r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
            }

            // removed ignored columns  
            for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
            {
                if (i == 0 && showSrNo)
                {
                    continue;
                }
             
                if (!columnsToTake.Contains(dataTable.Columns[i].ColumnName))
                {
                    workSheet.DeleteColumn(i + 1);
                }
            }

            if (!String.IsNullOrEmpty(heading))
            {
                workSheet.Cells["A1"].Value = heading;
                workSheet.Cells["A1"].Style.Font.Size = 20;
                workSheet.InsertColumn(1, 1);
                workSheet.InsertRow(1, 1);
                workSheet.Column(1).Width = 5;
            }
        }

        private static void ProcessWorkSheet(ExcelWorksheet workSheet, DataTable dataTable, string heading = "", bool Stylefooter = false, bool showSrNo = false, params string[] columnsToTake)
        {
            int startRowFrom = String.IsNullOrEmpty(heading) ? 1 : 2;
            if (showSrNo)
            {
                DataColumn dataColumn = dataTable.Columns.Add("Sr.No.", typeof(int));
                dataColumn.SetOrdinal(0);
                int index = 1;
                foreach (DataRow item in dataTable.Rows)
                {
                    item[0] = index;
                    index++;
                }
            }
            // add the content into the Excel file  
            workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);

            // autofit width of cells with small content  
            int columnIndex = 1;
            foreach (DataColumn column in dataTable.Columns)
            {
                //column.
                ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];
                int maxLength = columnCells.Max(cell => String.Format("{0}", cell.Value).Length);
                if (maxLength < 150)
                {
                    workSheet.Column(columnIndex).AutoFit();
                }
                columnIndex++;
            }

            // format header - bold, yellow on black  
            using (ExcelRange r = workSheet.Cells[startRowFrom, 1, startRowFrom, dataTable.Columns.Count])
            {
                r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                r.Style.Font.Bold = true;
                r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#1fb5ad"));
            }

            // format Footer - bold
            if (Stylefooter)
            {
                using (ExcelRange r = workSheet.Cells[startRowFrom + dataTable.Rows.Count, 1, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
                {
                    r.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Font.Bold = true;
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    // r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#1fb5ad"));
                }
            }
            // format cells - add borders  
            using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
            {
                r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
            }

            // removed ignored columns  
            for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
            {
                if (i == 0 && showSrNo)
                {
                    continue;
                }

                if (!columnsToTake.Contains(dataTable.Columns[i].ColumnName))
                {
                    workSheet.DeleteColumn(i + 1);
                }
            }

            if (!String.IsNullOrEmpty(heading))
            {
                workSheet.Cells["A1"].Value = heading;
                workSheet.Cells["A1"].Style.Font.Size = 20;
                workSheet.InsertColumn(1, 1);
                workSheet.InsertRow(1, 1);
                workSheet.Column(1).Width = 5;
            }
        }
        private static void ActivityProcessWorkSheet(ExcelWorksheet workSheet, DataTable dataTable, string heading = "", bool showSrNo = false, params string[] columnsToTake) {
            int startRowFrom = String.IsNullOrEmpty(heading) ? 1 : 2;
            if (showSrNo) {
                DataColumn dataColumn = dataTable.Columns.Add("Sr.No.", typeof(int));
                dataColumn.SetOrdinal(0);
                int index = 1;
                foreach (DataRow item in dataTable.Rows) {
                    item[0] = index;
                    index++;
                }
            }
            // add the content into the Excel file  
            workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);

            // autofit width of cells with small content  
            int columnIndex = 1;
            foreach (DataColumn column in dataTable.Columns) {
                //column.
                ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];
                int maxLength = columnCells.Max(cell => String.Format("{0}", cell.Value).Length);
                if (maxLength < 150) {
                    workSheet.Column(columnIndex).AutoFit();
                }
                columnIndex++;
            }

            // format header - bold, yellow on black  
            using (ExcelRange r = workSheet.Cells[startRowFrom, 1, startRowFrom, dataTable.Columns.Count]) {
                r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                r.Style.Font.Bold = true;
                r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#1fb5ad"));
            }

            // format cells - add borders  
            using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count]) {
                r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
            }

            // removed ignored columns  
            for (int i = dataTable.Columns.Count - 1; i >= 0; i--) {
                if (i == 0 && showSrNo) {
                    continue;
                }

            }

            if (!String.IsNullOrEmpty(heading)) {
                workSheet.Cells["A1"].Value = heading;
                workSheet.Cells["A1"].Style.Font.Size = 20;
                workSheet.InsertColumn(1, 1);
                workSheet.InsertRow(1, 1);
                workSheet.Column(1).Width = 5;
            }
        }

        public static byte[] VaccinationExportExcel<T>(List<T> data, string Heading = "", bool showSlno = false, params string[] ColumnsToTake)
        {
            return VaccinationExportExcel(VaccinationToDataTable<T>(data), Heading, showSlno, ColumnsToTake);
        }


        public static byte[] ExportExcel<T>(List<T> data, string Heading = "", bool showSlno = false, params string[] ColumnsToTake)
        {
            return ExportExcel(ToDataTable<T>(data), Heading, showSlno, ColumnsToTake);
        }
        public static byte[] ExportExcel<T>(List<T> data, string Heading = "", bool Stylefooter = false, bool showSlno = false, params string[] ColumnsToTake)
        {
            return ExportExcel(ToDataTable<T>(data), Heading, Stylefooter, showSlno, ColumnsToTake);
        }


        public static byte[] ActivityExportToExcel<T>(List<T> data, string Heading = "", bool showSlno = false, params string[] ColumnsToTake) {
            return ActivityExportExcel(ActivityToDataTable<T>(data), Heading, showSlno, ColumnsToTake);
        }
        #endregion

    }

    public class ExportExcelData
    {
        public DataTable DataTable { get; set; }
        public string Heading { get; set; }
        public bool ShowSrNo { get; set; }
        public string[] ColumnsToTake { get; set; }
    }
}
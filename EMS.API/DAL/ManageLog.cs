using EMS.Core;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace EMS.API.DAL
{
    public  class ManageLog : IManageLog
    {
        private IHostingEnvironment _env;
        public ManageLog(IHostingEnvironment hostingEnvironment)
        {
            _env = hostingEnvironment;
        }

        public  void WriteLogFile(string FolderName,string Description)  
        {


                FileStream fileStream = null;
                DirectoryInfo directoryInfo;
                FileInfo fileInfo;

                var webRoot = _env.WebRootPath;
                var filePath = Path.Combine(webRoot, FolderName, $"APILog_{DateTime.Today.ToFormatDateString("dd - MM - yyyy")}.txt");
                // string filePath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath, $"{SiteKey.LogFilePath}{fileName}.txt");

                fileInfo = new FileInfo(filePath);
                directoryInfo = new DirectoryInfo(fileInfo.DirectoryName);

                if (!directoryInfo.Exists)
                    directoryInfo.Create();

                if (!fileInfo.Exists)
                    fileStream = fileInfo.Create();
                else
                    fileStream = new FileStream(filePath, FileMode.Append);

                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.WriteLine(Description);
                streamWriter.Close();

                // System.IO.File.WriteAllText(filePath, description);
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace UpdateCRMProjectSchedular.DAL
{
    public static class ManageLog
    {
        public static void WriteLogFile(string Description)
        {

            //string filePath = $"{GetApplicationRoot()}\\UpdateCRMProjectLog.txt";
            string filePath = SiteKeys.LogFilePath + "UpdateCRMProjectLog.txt";
            //Console.WriteLine("=============== " + filePath + " =====================");
            StreamWriter objWrite = default(StreamWriter);

            if (File.Exists(filePath))//append  
            {
                objWrite = File.AppendText(filePath);
            }
            else//create                
                objWrite = File.CreateText(filePath);

            objWrite.WriteLine(Description + Environment.NewLine);

            objWrite.Flush();
            objWrite.Close();
            objWrite.Dispose();
        }

        public static string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(System.Reflection
                              .Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return appRoot;
        }
    }
}

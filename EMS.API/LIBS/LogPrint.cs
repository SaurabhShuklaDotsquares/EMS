using System.IO;

namespace EMS.API.LIBS
{
    public static class LogPrint
    {
        public static void WriteIntoFile(string filepath, string message)
        {
            if (System.IO.File.Exists(filepath))
            {
                using (var tw = new StreamWriter(filepath, true))
                {
                    tw.WriteLine(message);
                }
            }
            else
            {
                // Create File Here First then Use this code
                using (var tw = new StreamWriter(filepath, true))
                {
                    tw.WriteLine(message);
                }
            }

        }
    }
}

using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;
using System.IO;

namespace EMS.Web.Code.LIBS
{
    public class ZipHelper
    {
        public static MemoryStream ZipFilesInStream(List<string> zipFileList)
        {
            MemoryStream baseOutputStream = new MemoryStream();
            ZipOutputStream zipOutput = new ZipOutputStream(baseOutputStream);
            zipOutput.IsStreamOwner = false;

            /*
            * Higher compression level will cause higher usage of reources
            * If not necessary do not use highest level 9
            */
            zipOutput.SetLevel(3);

            byte[] buffer = new byte[4096];
            foreach (string fileName in zipFileList)
            {
                using (Stream fs = File.OpenRead(fileName))
                {
                    ZipEntry entry = new ZipEntry(ZipEntry.CleanName(Path.GetFileName(fileName)));
                    entry.Size = fs.Length;

                    zipOutput.PutNextEntry(entry);

                    int count = fs.Read(buffer, 0, buffer.Length);
                    while (count > 0)
                    {
                        zipOutput.Write(buffer, 0, count);
                        count = fs.Read(buffer, 0, buffer.Length);
                    }
                }
            }

            zipOutput.Finish();
            zipOutput.Close();

            /* Set position to 0 so that cient start reading of the stream from the begining */
            baseOutputStream.Position = 0;
            return baseOutputStream;
        }
    }
}
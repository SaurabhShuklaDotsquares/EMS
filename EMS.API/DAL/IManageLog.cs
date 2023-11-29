namespace EMS.API.DAL
{
    public interface IManageLog
    {
        void WriteLogFile(string FolderName,string Description);
    }
}
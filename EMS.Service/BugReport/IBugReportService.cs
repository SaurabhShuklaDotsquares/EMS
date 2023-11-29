using EMS.Data;
using EMS.Dto;
using System;
using System.Collections.Generic;

namespace EMS.Service
{
    public interface IBugReportService : IDisposable
    {
        List<ReportBug> GetBugReportByPaging(out int total, PagingService<ReportBug> pagingService);
        ReportBug Save(BugReportDto model);
        ReportBug GetBugReportById(int id);
        ReportBug UpdateStatus(BugStatusDto model);
        bool Delete(int id);
    }
}

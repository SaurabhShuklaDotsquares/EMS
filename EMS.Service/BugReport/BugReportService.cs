using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using EMS.Core;

namespace EMS.Service
{
    public class BugReportService : IBugReportService
    {
        private readonly IRepository<ReportBug> repoBugReport;
        public BugReportService(IRepository<ReportBug> _repoBugReport)
        {
            repoBugReport = _repoBugReport;
        }
        public ReportBug GetBugReportById(int id)
        {
            return repoBugReport.FindById(id);
        }

        public List<ReportBug> GetBugReportByPaging(out int total, PagingService<ReportBug> pagingService)
        {
            return repoBugReport.Query()
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort)
                .GetPage(pagingService.Start, pagingService.Length, out total)
                .ToList();
        }

        public ReportBug Save(BugReportDto model)
        {
            ReportBug bugReport = null;
            if(model.Id == 0)
            {
                bugReport = new ReportBug
                {
                    SectionName = model.SectionName,
                    SectionDescription = model.SectionDescription,
                    ImageName = model.ImageName,
                    AddDate = DateTime.Now,
                    ModifyDate = DateTime.Now,                    
                    IP = model.IP,
                    UserId = model.AddedBy,
                    IsClosed = false,
                    PagePath = model.PagePath,
                    Remark  = model.Remark,
                    IsApproved = false,
                    Status = (byte)Enums.BugReportStatus.Pending
                };
                repoBugReport.Insert(bugReport);
            }
            return bugReport;
        }
        public bool Delete(int id)
        {
            var entity = GetBugReportById(id);
            if(entity!=null)
            {
                repoBugReport.Delete(id);
                return true;
            }
            return false;
        }

        public ReportBug UpdateStatus(BugStatusDto model)
        {
            ReportBug bugReport = null;
            var entity = GetBugReportById(model.Id);
            if (entity != null)
            {
                entity.Status = (byte)model.StatusId;
                entity.Remark = model.Comment;
                repoBugReport.SaveChanges();
                return entity;
            }
            return bugReport;
        }

        public void Dispose()
        {
            if (repoBugReport != null)
            {
                repoBugReport.Dispose();
            }

        }

    }
}

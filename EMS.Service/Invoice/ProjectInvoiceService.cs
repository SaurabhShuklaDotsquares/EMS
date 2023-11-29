using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service
{
    public class ProjectInvoiceService : IProjectInvoiceService
    {
        #region "Fields"
        private IRepository<ProjectInvoice> repoProjectInvoice;
        private IRepository<ProjectInvoiceComment> repoProjectInvoiceComment;

        #endregion

        #region "Cosntructor"
        public ProjectInvoiceService(IRepository<ProjectInvoice> repoProjectInvoice, IRepository<ProjectInvoiceComment> repoProjectInvoiceComment)
        {
            this.repoProjectInvoice = repoProjectInvoice;
            this.repoProjectInvoiceComment = repoProjectInvoiceComment;
        }

        public List<ProjectInvoice> GetInvoicesByPaging(out int total, PagingService<ProjectInvoice> pagingSerices)
        {

            return repoProjectInvoice
                  .Query()
                  .Filter(pagingSerices.Filter)
                  .OrderBy(pagingSerices.Sort)
                  .GetPage(pagingSerices.Start, pagingSerices.Length, out total)
                  .ToList();
        }

        
        public string GetInvoicesSummary(Expression<Func<ProjectInvoice, bool>> filters)
        {
            var summaries = repoProjectInvoice
                      .Query()
                      .Filter(filters)
                      .GetQuerable()
                      .GroupBy(x => x.CurrencyID)
                      .Select(x => new
                      {
                          CurrencySign = x.FirstOrDefault().Currency.CurrSign,
                          Amount = x.Sum(a => a.InvoiceAmount)
                      }).ToList();

            return string.Join(", ", summaries.Select(x => $"{x.CurrencySign} {x.Amount}"));
        }

        public List<ProjectInvoice> GetInvoiceList(PagingService<ProjectInvoice> pagingSerices)
        {
            return repoProjectInvoice
                .Query()
                .Filter(pagingSerices.Filter)
                .OrderBy(pagingSerices.Sort)
                .Get()
                .ToList();
        }

        public List<ProjectInvoice> GetInvoices()
        {
            return repoProjectInvoice.Query().Get().ToList();
        }

        public List<ProjectInvoice> GetDataByInvoiceNumbers(int pmuid, IEnumerable<string> invoiceNumbers)
        {
            return repoProjectInvoice.Query().Filter(P => P.PMID == pmuid && invoiceNumbers.Contains(P.InvoiceNumber)).Get().ToList();
        }

        public List<ProjectInvoice> GetUnPaidDataPMwise(int pmuid)
        {
            return repoProjectInvoice.Query().Filter(x => x.PMID == pmuid && x.InvoiceStatus != (int)Enums.ProjectInvoiceStatus.Paid && x.InvoiceStatus !=
                (int)Enums.ProjectInvoiceStatus.Cancelled).Get().OrderByDescending(x => x.Modified).ToList();
        }
        public ProjectInvoice CheckValidProject(int projectID, string invoiceNumber)
        {
            return repoProjectInvoice.Query().Filter(P => P.ProjectId == projectID && P.InvoiceNumber == invoiceNumber).Get().FirstOrDefault();
        }
        public List<ProjectInvoice> GetUnPaidData(int Uid)
        {
            return repoProjectInvoice.Query().Filter(x => x.InvoiceStatus != (int)Enums.ProjectInvoiceStatus.Paid && x.InvoiceStatus != (int)Enums.ProjectInvoiceStatus.Cancelled && (x.Uid_BA == Uid || x.Uid_TL == Uid)).Get().OrderByDescending(x => x.Modified).ToList();
        }
        public void DeleteInvoice(int id)
        {
            repoProjectInvoice.Delete(id);
        }
        public ProjectInvoice GetInvoiceByID(int InvoiceId)
        {
            return repoProjectInvoice.FindById(InvoiceId);
        }

        public ProjectInvoice Save(ProjectInvoice entity)
        {
            if (entity.Id > 0)
            {
                repoProjectInvoice.Update(entity);
            }
            else
            {
                repoProjectInvoice.Insert(entity);
            }
            return ProjectInvoiceById(entity.Id);
        }

        public ProjectInvoice SaveComment(InvoiceChaseDto model)
        {
            ProjectInvoice invoiceEntity = GetInvoiceByID(model.InvoiceId);
            if (invoiceEntity != null && (invoiceEntity.Uid_BA == model.CurrentUserId || invoiceEntity.Uid_TL == model.CurrentUserId || invoiceEntity.PMID == model.CurrentUserId))
            {
                repoProjectInvoiceComment.Insert(new ProjectInvoiceComment()
                {
                    ProjectInvoiceId = model.InvoiceId,
                    ChaseDate = model.ChaseDate.ToDateTime("dd/MM/yyyy"),
                    InvoiceComments = model.Comment,
                    Created = DateTime.Now
                });
                
                return ProjectInvoiceById(model.InvoiceId);
            }

            return null;
        }

        private ProjectInvoice ProjectInvoiceById(int Id)
        {
            return repoProjectInvoice.Query().Filter(x => x.Id == Id).Get().FirstOrDefault();
        }

        public ProjectInvoice UpdateStatus(ProjectInvoice entity)
        {
            repoProjectInvoice.SaveChanges();
            return GetInvoiceByID(entity.Id);
        }

        #endregion
        public void Dispose()
        {
            if (repoProjectInvoice != null)
            {
                repoProjectInvoice.Dispose();
                repoProjectInvoice = null;
            }

        }
    }
}

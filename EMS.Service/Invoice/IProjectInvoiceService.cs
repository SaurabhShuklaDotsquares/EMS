using EMS.Data;
using EMS.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service
{
    public interface IProjectInvoiceService : IDisposable
    {
        List<ProjectInvoice> GetInvoicesByPaging(out int total,  PagingService<ProjectInvoice> pagingSerices);
        string GetInvoicesSummary(Expression<Func<ProjectInvoice, bool>> filters);

        List<ProjectInvoice> GetInvoiceList(PagingService<ProjectInvoice> pagingSerices);

        List<ProjectInvoice> GetInvoices();
        ProjectInvoice GetInvoiceByID(int id);
        ProjectInvoice Save(ProjectInvoice entity);

        ProjectInvoice SaveComment(InvoiceChaseDto model);
        ProjectInvoice UpdateStatus(ProjectInvoice entity);
        void DeleteInvoice(int id);
        ProjectInvoice CheckValidProject(int projectID, string invoiceNumber);
        List<ProjectInvoice> GetUnPaidDataPMwise(int pmuid);
        List<ProjectInvoice> GetUnPaidData(int Uid);

        List<ProjectInvoice> GetDataByInvoiceNumbers(int pmuid, IEnumerable<string> invoiceNumbers);

    }
}

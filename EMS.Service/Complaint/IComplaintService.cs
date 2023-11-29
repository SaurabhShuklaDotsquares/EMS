using EMS.Data;
using EMS.Data.Model;
using EMS.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service
{
    public interface IComplaintService : IDisposable
    {
        List<Complaint> GetComplaintByPaging(out int total, PagingService<Complaint> pagingService);
        Complaint Save(ComplaintDto model);
        Complaint GetComplaintById(int id);
        List<ComplaintUser> GetComplaintUserById(int id);
        bool Delete(int id);
        //List<Complaint> GetComplaints(int uid, DateTime startDate, DateTime endDate);
        List<Complaint> GetComplaintsByFilter(Expression<Func<Complaint, bool>> filter);
        List<ComplaintUser> GetComplaintUserIdById(int id);
        List<Complaint> GetProjectByComplainIds(List<int> currentUserProjectComplaintId);
        List<Complaint> GetComplaintUserIdByPMId(int id);
        List<ComplaintUser> GetEmployeeByIds(List<int> currentUserProjectComplaintId);
    }
}

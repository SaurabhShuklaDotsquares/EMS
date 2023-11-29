using System;
using System.Collections.Generic;
using EMS.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service
{
    public interface ILeadServices : IDisposable
    {
        bool Save(EstimateDocument entity);
        List<EstimateDocument> GetData();
        EstimateDocument GetEstimateById(int id);
        List<EstimateDocument> GetEstimateDocByPaging(out int total, PagingService<EstimateDocument> pagingServices);
        List<EstimateDocument> GetEstimatesDocuments();
        List<ProjectLead> GetLeadsByPaging(out int total, PagingService<ProjectLead> pagingService);
        IEnumerable<ProjectLead> GetFilterdLeads(PagingService<ProjectLead> filterExp);
        List<ProjectLead> GetProjectLeads();
        ProjectLead GetLeadById(int leadId);
        List<LeadStatu> GetLeadStatus(int? parentId);
        LeadStatu GetLeadStatusById(int id);
        List<TypeMaster> GetLeadType(string leadGroup);
        List<LeadClient> GetLeadClient(int pmUID);
        void SaveLeadClient(LeadClient entity);
        void SaveLead(ProjectLead entity);
        void SaveLeadTransaction(LeadTransaction entity);
        List<AbroadPM> getAbroadPM();
        List<int> GetTakenUsers();
        List<int> TakenTechnologies();
        LeadStatu GetLeadStatusByName(string statusName);
        void SaveLeadTechnology(List<ProjectLeadTech> technologyList);
        void SaveLeadTechnicians(List<LeadTechnician> technicianList);
        void DeleteLead(int id);
        void DeleteEstimateDocuments(int id);
        List<LeadTransaction> GetLeadTransaction(int leadId);
        List<ProjectLead> GetProjectLeads(int? status = null);
        List<ProjectLead> GetAllLeadsByUserId(int ownerId, int communicatorId, int technicianId, int? status = null);
        List<UserLogin> GetTakenUsersByPM(int pmUID);
        List<UserLogin> GetTakenUsersByPM1(int pmUid);
        List<ProjectLead> GetLeads(int uid, DateTime? startDate, DateTime? endDate);
        List<ProjectLead> GetLeadsAwarded(int uid, DateTime? startDate, DateTime? endDate);
        void SaveProjectLeadIndustry(List<ProjectLeadIndustry> entity);
        List<ProjectLeadIndustry> GetProjectLeadIndustry(int leadId);
        void SaveEstimateDocumentIndustry(List<EstimateDocumentIndustry> entity);
        List<EstimateDocumentIndustry> GetEstimateDocumentIndustry(int Id);
        List<ProjectLead> GetProjectLeadOnDate(int AddedByUid, DateTime NextChasedDate);
        ProjectLead GetProjectLeadByProjectClosureId(int ProjectClosureId);
        int GetProjectClosureLeadId(int ProjectClosureId);
    }
}

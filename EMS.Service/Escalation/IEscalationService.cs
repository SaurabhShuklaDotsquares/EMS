using EMS.Data;
using EMS.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service
{
    public interface IEscalationService : IDisposable
    {
        Escalation GetFirstEscalation();

        List<Escalation> GetEscalations(out int total, PagingService<Escalation> pagingService);

        Escalation GetEscalationById(int escalationId);

          new void  Dispose();

        Escalation Save(Escalation entity);

        void SaveCollection(List<Escalation> entityCollection);
        bool EscalationUserDeleted(Escalation escalation);
        bool EscalationFoundUserDeleted(Escalation escalation);
        bool EscalationDocumentDeleted(Escalation escalation);
        bool DeleteDocumentFile(int documentId);
        void SaveEscalationConclusion(EscalationConclusion entity);
        List<EscalationConclusion> GetEscalationConclusions(out int total, PagingService<EscalationConclusion> pagingService);
        EscalationConclusion GetEscalationConclusionById(int id);
    }
}
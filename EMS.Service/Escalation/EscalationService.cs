using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EMS.Service
{
    public class EscalationService : IEscalationService
    {
        private readonly IRepository<Escalation> repoEscalation;
        private readonly IRepository<EscalationDocument> escalationDocumentRepository;
        private readonly IRepository<UserLogin> repoUserLogin;
        private readonly IRepository<EscalationConclusion> repoEscalationConclusion;
        //private readonly IRepository<EsculationForUser> repoEsculationForUser;
        //private readonly IRepository<EsculationFoundForUser> repoEsculationFoundForUser;
        public EscalationService(IRepository<Escalation> repoEscalation,
            IRepository<EscalationDocument> escalationDocumentRepository,
            IRepository<UserLogin> repoUserLogin,
            IRepository<EscalationConclusion> repoEscalationConclusion
            //IRepository<EsculationForUser> repoEsculationForUser,
            //IRepository<EsculationFoundForUser> repoEsculationFoundForUser
            )
        {
            this.repoEscalation = repoEscalation;
            this.escalationDocumentRepository = escalationDocumentRepository;
            this.repoUserLogin = repoUserLogin;
            this.repoEscalationConclusion = repoEscalationConclusion;
            //this.repoEsculationForUser = repoEsculationForUser;
            //this.repoEsculationFoundForUser = repoEsculationFoundForUser;
        }

        public Escalation GetFirstEscalation()
        {
            return repoEscalation.Query().Get().OrderBy(x => x.AddDate).FirstOrDefault();
        }

        public List<Escalation> GetEscalations(out int total, PagingService<Escalation> pagingService)
        {

            return repoEscalation.Query()
            .Filter(pagingService.Filter)
            .Include(x => x.EscalationTypeNavigation)
            .OrderBy(pagingService.Sort)
            .GetPage(pagingService.Start, pagingService.Length, out total).ToList();
        }

        public List<EscalationConclusion> GetEscalationConclusions(out int total, PagingService<EscalationConclusion> pagingService)
        {
            return repoEscalationConclusion.Query()
            .Filter(pagingService.Filter)
            .OrderBy(pagingService.Sort)
            .GetPage(pagingService.Start, pagingService.Length, out total).ToList();
        }

        public Escalation GetEscalationById(int escalationId)
        {
            return repoEscalation.FindById(escalationId);
        }

        public void Dispose()
        {
            if (repoEscalation != null)
            {
                repoEscalation.Dispose();
                // repoEscalation = null;
            }
        }
        public void SaveEscalationConclusion(EscalationConclusion entity)
        {
            if (entity.Id == 0)
            {
                entity.ModifyDate = DateTime.Now;
                entity.AddDate = DateTime.Now;
                repoEscalationConclusion.Insert(entity);
            }
            else
            {
                var escalationEntity = repoEscalationConclusion.FindById(entity.Id);
                escalationEntity.Resolution = entity.Resolution;
                escalationEntity.LessonLearnExplanation = entity.LessonLearnExplanation;
                escalationEntity.ModifyDate = DateTime.Now;
                repoEscalationConclusion.SaveChanges();
            }
        }

        public Escalation Save(Escalation entity)
        {
            if (entity.Id == 0)
            {
                return repoEscalation.InsertCallback(entity);
            }
            else
            {
                var escalationEntity = repoEscalation.FindById(entity.Id);

                escalationEntity.EscalationDescription = entity.EscalationDescription;
                escalationEntity.DateofEscalation = entity.DateofEscalation;
                escalationEntity.EscalationType = entity.EscalationType;
                escalationEntity.RootCause = entity.RootCause;
                escalationEntity.ProjectId = entity.ProjectId;
                escalationEntity.SeverityLevel = entity.SeverityLevel;
                escalationEntity.Status = entity.Status;
                escalationEntity.IsActive = true;
                escalationEntity.EsculationForUser = entity.EsculationForUser;
                escalationEntity.EsculationFoundForUser = entity.EsculationFoundForUser;
                escalationEntity.EscalationDocument = entity.EscalationDocument;

                if (entity.EsculationForUser != null && entity.EsculationForUser.Any())
                {
                    entity.EsculationForUser.ToList().ForEach(a =>
                    {
                        escalationEntity.EsculationForUser.Add(a);
                    });
                }
                if (entity.EsculationFoundForUser != null && entity.EsculationFoundForUser.Any())
                {
                    entity.EsculationFoundForUser.ToList().ForEach(a =>
                    {
                        escalationEntity.EsculationFoundForUser.Add(a);
                    });
                }

                if (entity.EscalationDocument != null && entity.EscalationDocument.Any())
                {
                    entity.EscalationDocument.ToList().ForEach(a =>
                    {
                        escalationEntity.EscalationDocument.Add(a);
                    });
                }
                repoEscalation.SaveChanges();
                return entity;
            }
        }

        public void SaveCollection(List<Escalation> entityCollection)
        {
            repoEscalation.InsertCollection(entityCollection);
        }

        public bool EscalationUserDeleted(Escalation escalation)
        {
            repoEscalation.ChangeEntityCollectionState(escalation.EsculationForUser, ObjectState.Deleted);
            escalation.EsculationForUser.Clear();
            repoEscalation.SaveChanges();
            return true;
        }

        public bool EscalationFoundUserDeleted(Escalation escalation)
        {
            repoEscalation.ChangeEntityCollectionState(escalation.EsculationFoundForUser, ObjectState.Deleted);
            escalation.EsculationFoundForUser.Clear();
            repoEscalation.SaveChanges();
            return true;
        }

        public bool EscalationDocumentDeleted(Escalation escalation)
        {
            repoEscalation.ChangeEntityCollectionState(escalation.EscalationDocument, ObjectState.Deleted);
            escalation.EscalationDocument.Clear();
            repoEscalation.SaveChanges();
            return true;
        }
        public bool DeleteDocumentFile(int documentId)
        {
            escalationDocumentRepository.Delete(documentId);
            return true;
        }

        public EscalationConclusion GetEscalationConclusionById(int id)
        {
            return repoEscalationConclusion.FindById(id);
        }
    }
}
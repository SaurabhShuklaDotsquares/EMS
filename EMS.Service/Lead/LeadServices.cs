using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Core;

namespace EMS.Service
{
    public class LeadServices : ILeadServices
    {
        #region "Fileds"
        private IRepository<EstimateDocument> repoEstimateDocument;
        private IRepository<ProjectLead> repoProjectLead;
        private IRepository<LeadStatu> repoLeadStatus;
        private IRepository<TypeMaster> repoTypeMaster;
        private IRepository<LeadClient> repoLeadClient;
        private IRepository<LeadTransaction> repoleadTransaction;
        private IRepository<AbroadPM> repoAbroadPM;
        private IRepository<LeadTechnician> repoLeadTechnician;
        private IRepository<ProjectLeadTech> repoLeadTechnology;
        private IRepository<UserLogin> repouserlogin;
        private IRepository<ProjectLeadIndustry> repoProjectLeadIndustry;
        private IRepository<EstimateDocumentIndustry> repoEstimateDocumentIndustry;
        #endregion

        #region "Constructor"
        public LeadServices(IRepository<UserLogin> repouserlogin, IRepository<EstimateDocument> repoEstimateDocument,
            IRepository<ProjectLead> repoProjectLead, IRepository<LeadStatu> repoLeadStatus, IRepository<TypeMaster> repoTypeMaster,
            IRepository<LeadClient> repoLeadClient, IRepository<LeadTransaction> repoleadTransaction,
            IRepository<AbroadPM> repoAbroadPM, IRepository<LeadTechnician> repoLeadTechnician,
            IRepository<ProjectLeadTech> repoLeadTechnology,
            IRepository<ProjectLeadIndustry> repoProjectLeadIndustry,
            IRepository<EstimateDocumentIndustry> repoEstimateDocumentIndustry)
        {
            this.repoEstimateDocument = repoEstimateDocument;
            this.repoProjectLead = repoProjectLead;
            this.repoLeadStatus = repoLeadStatus;
            this.repoTypeMaster = repoTypeMaster;
            this.repoLeadClient = repoLeadClient;
            this.repoleadTransaction = repoleadTransaction;
            this.repoAbroadPM = repoAbroadPM;
            this.repoLeadTechnician = repoLeadTechnician;
            this.repoLeadTechnology = repoLeadTechnology;
            this.repouserlogin = repouserlogin;
            this.repoProjectLeadIndustry = repoProjectLeadIndustry;
            this.repoEstimateDocumentIndustry = repoEstimateDocumentIndustry;
        }
        #endregion

        public bool Save(EstimateDocument entity)
        {
            if (entity.Id == 0)
            {
                repoEstimateDocument.ChangeEntityState(entity, ObjectState.Added);
            }
            else
            {
                repoEstimateDocument.ChangeEntityState(entity, ObjectState.Modified);
            }
            repoEstimateDocument.SaveChanges();
            return false;
        }

        public List<EstimateDocument> GetData()
        {
            return repoEstimateDocument.Query().Get().ToList();
        }

        public List<EstimateDocument> GetEstimateDocByPaging(out int total, PagingService<EstimateDocument> pagingSerices)
        {
            return repoEstimateDocument.Query().
                Filter(pagingSerices.Filter).
                OrderBy(pagingSerices.Sort).
                GetPage(pagingSerices.Start, pagingSerices.Length, out total).
                ToList();
        }

        public EstimateDocument GetEstimateById(int id)
        {
            return repoEstimateDocument.Query()
                        .Filter(x => x.Id == id)
                        .GetQuerable()
                        .FirstOrDefault();
        }

        public List<EstimateDocument> GetEstimatesDocuments()
        {
            return repoEstimateDocument.Query().Filter(x => (x.IsSpam == false || x.IsSpam == null)).Get().OrderByDescending(x => x.Modified).ToList(); ;
        }

        public IEnumerable<ProjectLead> GetFilterdLeads(PagingService<ProjectLead> filterExp)
        {
            return repoProjectLead.Query().Filter(filterExp.Filter).Get();
        }

        public List<ProjectLead> GetLeadsByPaging(out int total, PagingService<ProjectLead> pagingService)
        {
            var data = repoProjectLead.Query()
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort)
                .GetPage(pagingService.Start, pagingService.Length, out total)
                .ToList();

            foreach (var item in data)
            {
                item.Technologies = string.Join(", ", item.ProjectLeadTeches.Select(x => x.Technology?.Title)
                                                          .Concat(new string[] { item.Technologies })
                                                          .Where(x => !string.IsNullOrWhiteSpace(x))
                                                          .Distinct());
            }

            return data;
        }

        public List<ProjectLead> GetAllLeadsByUserId(int ownerId, int communicatorId, int technicianId, int? status = null)
        {
            List<ProjectLead> leads = new List<ProjectLead>();
            if (status.HasValue)
                leads = repoProjectLead.Query().Filter(L => L.Status == status.Value && L.OwnerId == ownerId || L.CommunicatorId == communicatorId || L.LeadTechnicians.Any(T => T.TechnicianId == technicianId)).GetQuerable().OrderByDescending(L => L.ModifyDate).ToList();
            else
                leads = repoProjectLead.Query().Filter(L => L.OwnerId == ownerId || L.CommunicatorId == communicatorId || L.LeadTechnicians.Any(T => T.TechnicianId == technicianId)).GetQuerable().OrderByDescending(L => L.ModifyDate).ToList();
            foreach (var item in leads)
            {
                string Technologies = "";
                if (item.ProjectLeadTeches.Count > 0)
                {
                    foreach (var inItem in item.ProjectLeadTeches.ToList())
                    {
                        if (Technologies == "")
                        {
                            Technologies = inItem.Technology.Title;
                        }
                        else
                        {
                            Technologies = Technologies + "," + inItem.Technology.Title;
                        }
                    }
                }
                if (item.Technologies == "")
                    item.Technologies = Technologies;
                else
                    item.Technologies = Technologies != "" ? Technologies + "," + item.Technologies : item.Technologies;
            }
            return leads;
        }

        public List<ProjectLead> GetProjectLeads(int? status = null)
        {
            List<ProjectLead> leads;
            if (status.HasValue)
                leads = repoProjectLead.Query().Filter(L => L.Status == status.Value).GetQuerable().OrderByDescending(L => L.ModifyDate).ToList();
            else
                leads = repoProjectLead.Query().GetQuerable().OrderByDescending(L => L.ModifyDate).ToList();
            foreach (var item in leads)
            {
                string Technologies = "";
                if (item.ProjectLeadTeches.Count > 0)
                {
                    foreach (var inItem in item.ProjectLeadTeches.ToList())
                    {
                        if (Technologies == "")
                        {
                            Technologies = inItem.Technology.Title;
                        }
                        else
                        {
                            Technologies = Technologies + "," + inItem.Technology.Title;
                        }
                    }
                }
                if (item.Technologies == "")
                    item.Technologies = Technologies;
                else
                    item.Technologies = Technologies != "" ? Technologies + "," + item.Technologies : item.Technologies;
            }
            return leads;
        }
        public ProjectLead GetLeadById(int leadId)
        {
            return repoProjectLead.Query().Filter(x => x.LeadId == leadId).GetQuerable().FirstOrDefault();
        }

        public List<LeadStatu> GetLeadStatus(int? parentId)
        {
            return repoLeadStatus.Query().Filter(x => (parentId.HasValue ? x.ParentId == parentId : x.ParentId == null)).Get().ToList();
        }
        public LeadStatu GetLeadStatusById(int id)
        {
            return repoLeadStatus.Query().Filter(x => x.StatusId == id).GetQuerable().FirstOrDefault();
        }
        public List<TypeMaster> GetLeadType(string leadGroup)
        {
            return repoTypeMaster.Query().Filter(x => x.TypeGroup == leadGroup).Get().ToList();
        }

        public List<ProjectLead> GetProjectLeads()
        {
            return repoProjectLead.Query().Get().ToList();
        }

        public List<LeadClient> GetLeadClient(int pmUid)
        {
            return repoLeadClient.Query().Filter(x => x.PMUid == pmUid).Get().ToList();
        }

        public void SaveLeadClient(LeadClient entity)
        {
            if (entity != null)
            {
                if (entity.LeadClientId == 0)
                {
                    repoLeadClient.ChangeEntityState(entity, ObjectState.Added);
                }
                else
                {
                    repoLeadClient.ChangeEntityState(entity, ObjectState.Modified);
                }
                repoLeadClient.SaveChanges();
            }
        }
        public void SaveLead(ProjectLead entity)
        {
            if (entity != null)
            {
                if (entity.LeadId != 0)
                {
                    repoProjectLead.ChangeEntityState(entity, ObjectState.Modified);
                    repoProjectLead.SaveChanges();
                }
                else
                {
                    repoProjectLead.Insert(entity);
                }
            }
        }

        public void DeleteLead(int id)
        {
            ProjectLead lead = repoProjectLead.FindById(id);
            repoLeadTechnician.ChangeEntityCollectionState(lead.LeadTechnicians, ObjectState.Deleted);
            repoleadTransaction.ChangeEntityCollectionState(lead.LeadTransactions, ObjectState.Deleted);
            repoLeadTechnology.ChangeEntityCollectionState(lead.ProjectLeadTeches, ObjectState.Deleted);
            repoEstimateDocument.ChangeEntityCollectionState(lead.EstimateDocuments, ObjectState.Deleted);
            repoProjectLead.ChangeEntityState(lead, ObjectState.Deleted);
            repoProjectLead.SaveChanges();
        }
        public void DeleteEstimateDocuments(int id)
        {
            ProjectLead lead = repoProjectLead.FindById(id);

            repoEstimateDocument.ChangeEntityCollectionState(lead.EstimateDocuments, ObjectState.Deleted);
            repoProjectLead.ChangeEntityState(lead, ObjectState.Deleted);
            repoProjectLead.SaveChanges();
        }

        public void SaveLeadTransaction(LeadTransaction entity)
        {
            if (entity != null)
            {
                if (entity.TransId == 0)
                {
                    repoleadTransaction.ChangeEntityState(entity, ObjectState.Added);
                }
                else
                {
                    repoleadTransaction.ChangeEntityState(entity, ObjectState.Modified);
                }
                repoleadTransaction.SaveChanges();
            }
        }

        public List<AbroadPM> getAbroadPM()
        {
            return repoAbroadPM.Query().Get().ToList();
        }

        public List<int> GetTakenUsers()
        {
            return repoLeadTechnician.Query().Get().Select(T => T.TechnicianId).Distinct().ToList();
        }

        public List<int> TakenTechnologies()
        {
            return repoLeadTechnology.Query().Get().Select(T => T.TechId).Distinct().ToList();
        }

        public LeadStatu GetLeadStatusByName(string statusName)
        {
            return repoLeadStatus.Query().Get().FirstOrDefault(x => x.StatusName.Contains(statusName));
        }

        public void SaveLeadTechnology(List<ProjectLeadTech> technologyList)
        {
            var lead = repoProjectLead.FindById(technologyList.Select(T => T.LeadId).FirstOrDefault());
            if (lead != null)
            {
                if (lead.ProjectLeadTeches.Any())
                {
                    repoLeadTechnology.ChangeEntityCollectionState(lead.ProjectLeadTeches, ObjectState.Deleted);
                }
                technologyList.ForEach(x => repoLeadTechnology.ChangeEntityState(x, ObjectState.Added));
                repoLeadTechnology.SaveChanges();
            }
        }

        public void SaveLeadTechnicians(List<LeadTechnician> techniciansList)
        {
            var lead = repoProjectLead.FindById(techniciansList.Select(T => T.LeadId).FirstOrDefault());
            if (lead != null)
            {
                if (lead.LeadTechnicians.Any())
                {
                    repoLeadTechnician.ChangeEntityCollectionState(lead.LeadTechnicians, ObjectState.Deleted);
                }
                techniciansList.ForEach(x => lead.LeadTechnicians.Add(x));
                repoLeadTechnician.SaveChanges();
            }
        }

        public List<LeadTransaction> GetLeadTransaction(int leadId)
        {
            return repoleadTransaction.Query().Filter(x => x.LeadId == leadId).GetQuerable().ToList();
        }

        public List<UserLogin> GetTakenUsersByPM(int pmUID)
        {
            var projetcLeads = repoProjectLead.Query().Filter(x => x.PMID == pmUID).GetQuerable();
            var ownerIds = projetcLeads.Select(x => x.OwnerId).Distinct().ToList();
            var communicatorIds = projetcLeads.Select(x => x.CommunicatorId).Distinct().ToList();
            var technicianIds = projetcLeads.SelectMany(x => x.LeadTechnicians.Select(y => y.TechnicianId)).Distinct().ToList();

            var allUserIds = ownerIds.Concat(communicatorIds).Concat(technicianIds).Distinct().ToList();

            return repouserlogin.Query().Filter(x => allUserIds.Contains(x.Uid) && x.IsActive == true)
                                        .OrderBy(o => o.OrderBy(x => x.Name))
                                        .Get()
                                        .ToList();



            //List<ProjectLead> projetcLeads = repoProjectLead.Query().Filter(x => x.PMID == pmUID).Get().ToList();
            //List<UserLogin> owners = projetcLeads.Select(x => repouserlogin.FindById(x.OwnerId)).Distinct().ToList();
            //List<UserLogin> communicators = projetcLeads.Select(x => repouserlogin.FindById(x.CommunicatorId)).Distinct().ToList();
            //List<UserLogin> technicians = projetcLeads.SelectMany(x => x.LeadTechnicians.Select(y => y.UserLogin)).Distinct().ToList();
            //List<UserLogin> allUsers = new List<UserLogin>();
            //int[] ownerId = owners.Select(x => x.Uid).ToArray();
            //int[] communicatorIds = communicators.Select(x => x.Uid).ToArray();
            //allUsers.AddRange(owners);
            //allUsers.AddRange(communicators.Where(c => !ownerId.Contains(c.Uid)).ToList());
            //allUsers.AddRange(technicians.Where(c => (!ownerId.Contains(c.Uid) && !communicatorIds.Contains(c.Uid))).ToList());
            //List<string> userNames = allUsers.Select(x => x.UserName).Distinct().ToList();
            //List<UserLogin> allUsers1 = new List<UserLogin>();
            //allUsers.ForEach(x =>
            //{
            //    if (!allUsers1.Any(u => u.UserName == x.UserName)) {
            //        allUsers1.Add(x);
            //    }
            //});
            //return allUsers1.OrderBy(u => u.Name).ToList();
        }

        public List<UserLogin> GetTakenUsersByPM1(int pmUid)
        {

            //var techleads = Global.Context.ProjectLeads.Where(x => x.OwnerId == id || x.CommunicatorId == id
            //|| x.LeadTechnicians.Any(T => T.TechnicianId == id)).Select(x => x.OwnerId).Distinct().ToList(); // Current Working Technicians

            List<ProjectLead> projetcLeads = repoProjectLead.Query().Filter(x => x.PMID == pmUid).Get().ToList();

            List<UserLogin> owners = projetcLeads.Select(x => x.UserLogin).Distinct().ToList();
            List<UserLogin> communicators = projetcLeads.Select(x => x.UserLogin1).Distinct().ToList();
            List<UserLogin> technicians = projetcLeads.SelectMany(x => x.LeadTechnicians.Select(y => y.UserLogin)).Distinct().ToList();

            List<UserLogin> allUsers = new List<UserLogin>();
            int[] ownerId = owners.Select(x => x.Uid).ToArray();
            int[] communicatorIds = communicators.Select(x => x.Uid).ToArray();
            allUsers.AddRange(owners);
            allUsers.AddRange(communicators.Where(c => !ownerId.Contains(c.Uid)).ToList());
            allUsers.AddRange(technicians.Where(c => (!ownerId.Contains(c.Uid) && !communicatorIds.Contains(c.Uid))).ToList());

            return allUsers.OrderBy(u => u.Name).ToList();
            //var techLeads = Global.Context.LeadTechnicians.Select(x => projetcLeads.Contains(x.LeadId)).ToList();


            //return Global.Context.UserLogins.Where(T => techleads.Contains(T.Uid) && T.PMUid == pmUid).ToList();

            // var techIds = Global.Context.LeadTechnicians.Select(T => T.TechnicianId).Distinct().ToList(); // Current Working Technicians
            // return Global.Context.UserLogins.Where(O => techIds.Contains(O.Uid) && O.PMUid == pmUid).ToList(); //  Current Working Technicians under PMUid
        }

        public List<ProjectLead> GetLeads(int uid, DateTime? startDate, DateTime? endDate)
        {
            var expr = PredicateBuilder.True<ProjectLead>();
            expr = expr.And(l => l.LeadTechnicians.Any(t => t.TechnicianId == uid));

            if (startDate.HasValue)
            {
                expr = expr.And(l => l.AssignedDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                expr = expr.And(l => l.AssignedDate <= endDate.Value);
            }

            return repoProjectLead.Query().Filter(expr)
            .Get().ToList();
        }
        public List<ProjectLead> GetLeadsAwarded(int uid, DateTime? startDate, DateTime? endDate)
        {
            var expr = PredicateBuilder.True<ProjectLead>();
            expr = expr.And(l => l.LeadTechnicians.Any(t => t.TechnicianId == uid) && l.Status == (int)Enums.LeadStatus.Converted);

            if (startDate.HasValue)
            {
                expr = expr.And(l => l.AssignedDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                expr = expr.And(l => l.AssignedDate <= endDate.Value);
            }

            return repoProjectLead.Query().Filter(expr)
            .Get().ToList();

            //return repoProjectLead.Query().Filter(l => l.AssignedDate >= startDate
            //&& l.AssignedDate <= endDate && l.LeadTechnicians.Any(t => t.TechnicianId == uid)
            //&& l.Status==(int)Enums.LeadStatus.Converted)
            //.Get().ToList();
        }

        public List<ProjectLead> GetProjectLeadOnDate(int AddedByUid, DateTime NextChasedDate)
        {
            return repoProjectLead.Query().Filter(p => (p.OwnerId == AddedByUid || p.CommunicatorId == AddedByUid) && p.NextChasedDate == NextChasedDate).Get().OrderByDescending(x => x.NextChasedDate).ToList();
        }
        public ProjectLead GetProjectLeadByProjectClosureId(int ProjectClosureId)
        {
            return repoProjectLead.Query().Filter(p => p.ProjectClosureId == ProjectClosureId).Get().FirstOrDefault();
        }

        public int GetProjectClosureLeadId(int ProjectClosureId)
        {
            return repoProjectLead.Query().Filter(p => p.ProjectClosureId == ProjectClosureId).Get().FirstOrDefault()?.LeadId ?? 0;
        }


        #region "Dispose"
        public void Dispose()
        {
            if (repoEstimateDocument != null)
            {
                repoEstimateDocument.Dispose();
                repoEstimateDocument = null;
            }

        }

        public void SaveProjectLeadIndustry(List<ProjectLeadIndustry> entity)
        {
            var lead = repoProjectLead.FindById(entity.Select(T => T.ProjectLeadId).FirstOrDefault());
            if (lead != null)
            {
                if (lead.ProjectLeadIndustry.Any())
                {
                    repoProjectLeadIndustry.ChangeEntityCollectionState(lead.ProjectLeadIndustry, ObjectState.Deleted);
                }
                entity.ForEach(x => repoProjectLeadIndustry.ChangeEntityState(x, ObjectState.Added));
                repoProjectLeadIndustry.SaveChanges();
            }
        }

        public List<ProjectLeadIndustry> GetProjectLeadIndustry(int leadId)
        {
            return repoProjectLeadIndustry.Query().Filter(x => x.ProjectLeadId == leadId).GetQuerable().ToList();
        }

        public void SaveEstimateDocumentIndustry(List<EstimateDocumentIndustry> entity)
        {
            var estimateDocument = repoEstimateDocument.FindById(entity.Select(T => T.EstimateDocumentId).FirstOrDefault());
            if (estimateDocument != null)
            {
                if (estimateDocument.EstimateDocumentIndustry != null && estimateDocument.EstimateDocumentIndustry.Any())
                {
                    repoProjectLeadIndustry.ChangeEntityCollectionState(estimateDocument.EstimateDocumentIndustry, ObjectState.Deleted);
                }
                entity.ForEach(x => repoEstimateDocumentIndustry.ChangeEntityState(x, ObjectState.Added));
                repoEstimateDocumentIndustry.SaveChanges();
            }
        }

        public List<EstimateDocumentIndustry> GetEstimateDocumentIndustry(int Id)
        {
            return repoEstimateDocumentIndustry.Query().Filter(x => x.EstimateDocumentId == Id).GetQuerable().ToList();
        }
        #endregion
    }
}

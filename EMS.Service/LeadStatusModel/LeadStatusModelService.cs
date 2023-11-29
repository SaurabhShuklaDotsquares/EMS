using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using EMS.Core;
//using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Service
{
    public class LeadStatusModelService : ILeadStatusModelService
    {

        #region Fields & Constructor
        private IRepository<LeadStatu> repoLeadStatusModel;
        public LeadStatusModelService(IRepository<LeadStatu> repoLeadStatusModel)
        {
            this.repoLeadStatusModel = repoLeadStatusModel;
        }
        #endregion
        public LeadStatu GetLeadStatusModelById(int id)
        {
            return repoLeadStatusModel
                .Query()
                .GetQuerable().FirstOrDefault(l => l.StatusId == id);
        }

        public List<LeadStatu> GetLeadStatusModelList()
        {
            return repoLeadStatusModel
                .Query()
                .Get()
                .ToList();
        }

        public bool IsLeadStatusModelExists(int statusId, string statusName)
        {
            return repoLeadStatusModel.Query()
                .Filter(x => x.StatusId != statusId && x.StatusName == statusName)
                .GetQuerable().Any();
        }

        public LeadStatu Save(LeadStatusModelDto model)
        {
            LeadStatu leadStatu = null;
            if (model.StatusId > 0)
            {
                leadStatu = GetLeadStatusModelById(model.StatusId);

                if (leadStatu == null)
                {
                    return null;
                }

            }
            else
            {
                leadStatu = new LeadStatu()
                {
                    AddDate = DateTime.Now,
                };

            }
            leadStatu.ModifyDate = DateTime.Now;
            leadStatu.StatusName = model.StatusName.HasValue()? model.StatusName.Trim():null;
            leadStatu.ParentId = model.ParentId;
            leadStatu.MailContent = model.MailContent.HasValue()? model.MailContent.Trim():null;
            leadStatu.IP = model.IP;
            leadStatu.FromEmail = model.FromEmail.HasValue()?model.FromEmail.Trim():null;
            leadStatu.To = model.To.HasValue()?model.To.Trim():null;
            leadStatu.CC = model.CC.HasValue()?model.CC.Trim():null;
            leadStatu.BCC = model.BCC.HasValue()?model.BCC.Trim():null;

            if (leadStatu.StatusId > 0)
            {
                repoLeadStatusModel.Update(leadStatu);
            }
            else
            {
                repoLeadStatusModel.Insert(leadStatu);
            }

            return leadStatu;

        }

        public List<LeadStatu> GetLeadStatusModelByPaging(out int total, PagingService<LeadStatu> pagingService)
        {
            return repoLeadStatusModel.Query()
                .Filter(pagingService.Filter).
                OrderBy(pagingService.Sort).
                GetPage(pagingService.Start, pagingService.Length, out total).
                ToList();
        }
        [HttpGet]
        public void DeleteLeadStatusModelById(int id)
        {
            LeadStatu leadStatu = repoLeadStatusModel.FindById(id);
            if (leadStatu != null)
            {
                repoLeadStatusModel.Delete(leadStatu);
            }
        }
    }
}

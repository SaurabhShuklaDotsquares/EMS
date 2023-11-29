using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EMS.Service
{
    public class PILogService : IPILogService
    {
        #region "Fields"

        private readonly IRepository<PILog> repoPILog;

        #endregion

        #region "Cosntructor"

        public PILogService(IRepository<PILog> _repoPILog)
        {
            repoPILog = _repoPILog;
        }

        #endregion

        public PILog GetPILogById(int id)
        {
            return repoPILog.FindById(id);
        }

        public PILog Save(PILogRequestDto model)
        {
            PILog logEntity = new PILog();

            if (model.Id > 0)
            {
                logEntity = GetPILogById(model.Id);
                if (model.StatusFromMoM == null && (logEntity == null || logEntity.CreateByUid != model.CurrentUserId || logEntity.Status != (byte)Enums.PILogStatus.Pending))
                {
                    return null;
                }

                if (model.StatusFromMoM != null)
                {
                    logEntity.Status = (byte)model.StatusFromMoM;
                }

                if (model.CreatedDateFromMoM != null)
                {
                    logEntity.CreateDate = (DateTime)model.CreatedDateFromMoM;
                    
                }

                if(model.EstimatedScheduleFromMoM != null && model.EstimatedScheduleFromMoM.Value != DateTime.MinValue && model.StatusFromMoM == (byte)Enums.PILogStatus.Approved)
                {
                    logEntity.EstimatedSchedule = model.EstimatedScheduleFromMoM;
                }

                if (!string.IsNullOrEmpty(model.RemarkFromMOM))
                {
                    logEntity.Remarks = model.RemarkFromMOM;
                }
            }

            logEntity.ProcessName = model.ProcessName;
            logEntity.PotentialArea = model.PotentialArea;
            logEntity.ModifyByUid = model.CurrentUserId;
            logEntity.ModifyDate = DateTime.Now;
            logEntity.MomMeetingTaskId = model.MomMeetingTaskId;
            logEntity.ProcessId = model.ProcessId;

            if (logEntity.Id == 0)
            {
                if (model.StatusFromMoM != null)
                {
                    logEntity.Status = (byte)model.StatusFromMoM;
                }
                else
                {
                    logEntity.Status = (byte)Enums.PILogStatus.Pending;
                }

                logEntity.CreateByUid = model.CurrentUserId;

                if (model.EstimatedScheduleFromMoM != null && model.EstimatedScheduleFromMoM.Value != DateTime.MinValue && model.StatusFromMoM == (byte)Enums.PILogStatus.Approved)
                {
                    logEntity.EstimatedSchedule = model.EstimatedScheduleFromMoM;
                }

                if (!string.IsNullOrEmpty(model.RemarkFromMOM))
                {
                    logEntity.Remarks = model.RemarkFromMOM;
                }

                if (model.CreatedDateFromMoM != null)
                {
                    logEntity.CreateDate = (DateTime)model.CreatedDateFromMoM;
                }
                else
                {
                    logEntity.CreateDate = DateTime.Now;  
                }

                repoPILog.InsertGraph(logEntity);
            }
            else
            {
                repoPILog.SaveChanges();
            }

            return logEntity;
        }

        public PILog UpdateApproval(PILogApprovalDto model)
        {
            var logEntity = GetPILogById(model.Id);

            if (logEntity != null && logEntity.Status == (byte)Enums.PILogStatus.Pending || logEntity.Status == (byte)Enums.PILogStatus.InProcess)
            {
                if (model.Status == (byte)Enums.PILogStatus.Approved)
                {
                    logEntity.EstimatedSchedule = model.EstimatedSchedule.ToDateTime("dd/MM/yyyy");
                    logEntity.Remarks = model.Remarks;
                }
                else if(model.Status == (byte)Enums.PILogStatus.Cancelled)
                {
                    logEntity.CancelReason = model.CancelReason;
                }
                else
                {
                    logEntity.Remarks = model.Remarks;
                }

                logEntity.ModifyDate = DateTime.Now;
                logEntity.ModifyByUid = model.CurrentUserId;
                logEntity.Status = model.Status;
                
                repoPILog.SaveChanges();

                return logEntity;
            }

            return null;
        }

        public PILog RollOut(int id, int currentUserId)
        {
            var logEntity = GetPILogById(id);

            if (logEntity != null && logEntity.Status == (byte)Enums.PILogStatus.Approved)
            {
                logEntity.ModifyDate = DateTime.Now;
                logEntity.ModifyByUid = currentUserId;
                logEntity.Status = (byte)Enums.PILogStatus.RollOut;

                repoPILog.SaveChanges();

                return logEntity;
            }

            return null;
        }

        public List<PILog> GetLogsByPaging(out int total, PagingService<PILog> pagingService)
        {
            return repoPILog.Query()
                    .Filter(pagingService.Filter)
                    .OrderBy(pagingService.Sort)
                    .Include(x => x.UserLogin)
                    .GetPage(pagingService.Start, pagingService.Length, out total)
                    .ToList();
        }
        

        #region "Dispose"

        public void Dispose()
        {
            if (repoPILog != null)
            {
                repoPILog.Dispose();
            }
        }

        #endregion
    }
}

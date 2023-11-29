using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EMS.Repo;
using EMS.Data.saralDT;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using EMS.Core;

namespace EMS.Service.SARALDT
{
    public class LevDetailsDTService : ILevDetailsDTService
    {
        private IConfiguration configuration;
        private IRepository<LevDetails> repoLevDetails;
        private IRepository<MasEmployee> repoMasEmployee;
        private IRepository<AttDefinition> repoAttDefinition;
        public LevDetailsDTService(IRepository<LevDetails> repoLevDetails, IConfiguration configuration, IRepository<MasEmployee> repoMasEmployee, IRepository<AttDefinition> repoAttDefinition)
        {
            this.configuration = configuration;
            this.repoLevDetails = repoLevDetails;
            this.repoMasEmployee = repoMasEmployee;
            this.repoAttDefinition = repoAttDefinition;
        }

        public MasEmployee GetDTEmployeeInfo(string email)
        {
            return repoMasEmployee.Query().Filter(x => x.Email.ToLower().Trim() == email.ToLower().Trim()).GetSaralDT().FirstOrDefault();
        }
        public List<LevDetails> GetDTLeaveDetailsByAttendanceId(int? AttendanceId)
        {
            return repoLevDetails.Query().Filter(x => x.Empid == AttendanceId).GetSaralDT().ToList();
        }
        public List<LevDetails> GetDTLeaveDetailsByLeaveDate(int? AttendanceId, DateTime startDate, DateTime endDate)
        {
            return repoLevDetails.Query().Filter(x => x.Empid == AttendanceId && x.Leavedate >= startDate && x.Leavedate <= endDate).GetSaralDT().ToList();
        }

        public List<AttDefinition> GetDTLeaveTypeList()
        {
            return repoAttDefinition.Query().Filter(x => x.Levid != (int)Enums.LeaveCategory.Holiday && x.Levid != (int)Enums.LeaveCategory.WeekOff).GetSaralDT().ToList();
        }
        public List<AttDefinition> GetDTLeaveTypeListByGender(string gender)
        {
            if (gender.Equals("M"))
            {
                return repoAttDefinition.Query().Filter(x => x.Levid != (int)Enums.LeaveCategory.Holiday && x.Levid != (int)Enums.LeaveCategory.WeekOff && x.Levid != (int)Enums.LeaveCategory.MaternityLeave).GetSaralDT().ToList();
            }
            else if (gender.Equals("F"))
            {
                return repoAttDefinition.Query().Filter(x => x.Levid != (int)Enums.LeaveCategory.Holiday && x.Levid != (int)Enums.LeaveCategory.WeekOff && x.Levid != (int)Enums.LeaveCategory.PaternityLeave).GetSaralDT().ToList();
            }
            return repoAttDefinition.Query().Filter(x => x.Levid != (int)Enums.LeaveCategory.Holiday && x.Levid != (int)Enums.LeaveCategory.WeekOff).GetSaralDT().ToList();

        }
        public void Save(LevDetails entity)
        {
            if (entity.Empid != 0)
            {
                repoLevDetails.ChangeEntityStateSaralDT<LevDetails>(entity, ObjectState.Added);
                repoLevDetails.InsertGraphSaralDT(entity);
            }
            //else
            //{
            //    repoLevDetails.ChangeEntityState<LevDetails>(entity, ObjectState.Modified);
            //    repoLevDetails.SaveChanges();
            //}

        }
        public void Delete(LevDetails entity)
        {
            if (entity.Empid > 0)
            {
                repoLevDetails.ChangeEntityStateSaralDT<LevDetails>(entity, ObjectState.Deleted);
                repoLevDetails.DeleteSaralDT(entity);
            }
        }
        public System.Data.DataTable GetDTLeaveBalance(int? AttendanceId, int monthYear)
        {
            string ConString = configuration.GetConnectionString("saralConnectionDT");
            System.Data.DataTable dataTable = new System.Data.DataTable();

            using (SqlConnection con = new SqlConnection(ConString))
            {
                SqlCommand cmd = new SqlCommand("levsumupdate", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.CommandTimeout = 300;
                cmd.Parameters.Add("@empid", SqlDbType.Int).Value = AttendanceId;
                cmd.Parameters.Add("@monthyear", SqlDbType.Int).Value = monthYear;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet dataSet = new DataSet();
                da.Fill(dataSet);

                if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0] != null)
                    dataTable = dataSet.Tables[0];
            }

            return dataTable;
        }
        public void SetDTUserDesignation(string EMPID, string EMPNAME, string DESIGNATION, string EMAILID, DateTime EFFFROM, string DBNAME)
        {
            using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("saralConnectionDT")))
            {
                using (SqlCommand cmd = new SqlCommand("CUSDBINTEGRATION", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;
                    cmd.Parameters.Add("@EMPID", SqlDbType.NVarChar).Value = EMPID;
                    cmd.Parameters.Add("@EMPNAME", SqlDbType.NVarChar).Value = EMPNAME;
                    cmd.Parameters.Add("@DESIGNATION", SqlDbType.NVarChar).Value = DESIGNATION;
                    cmd.Parameters.Add("@EMAILID", SqlDbType.NVarChar).Value = EMAILID;
                    cmd.Parameters.Add("@EFFFROM", SqlDbType.DateTime).Value = EFFFROM;
                    cmd.Parameters.Add("@DBNAME", SqlDbType.NVarChar).Value = DBNAME;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    // SqlDataAdapter da = new SqlDataAdapter(cmd);
                }
            }

        }
        public void Dispose()
        {
            if (repoLevDetails != null)
            {
                repoLevDetails.Dispose();
                repoLevDetails = null;
            }
        }
    }
}

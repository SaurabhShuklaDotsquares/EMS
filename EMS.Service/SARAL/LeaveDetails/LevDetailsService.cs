using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EMS.Repo;
using EMS.Data.saral;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using EMS.Core;

namespace EMS.Service.SARAL
{
    public class LevDetailsService : ILevDetailsService
    {
        private IConfiguration configuration;
        private IRepository<LevDetails> repoLevDetails;
        private IRepository<MasEmployee> repoMasEmployee;
        private IRepository<AttDefinition> repoAttDefinition;
        public LevDetailsService(IRepository<LevDetails> repoLevDetails, IConfiguration configuration, IRepository<MasEmployee> repoMasEmployee, IRepository<AttDefinition> repoAttDefinition)
        {
            this.configuration = configuration;
            this.repoLevDetails = repoLevDetails;
            this.repoMasEmployee = repoMasEmployee;
            this.repoAttDefinition = repoAttDefinition;
        }

        public MasEmployee GetEmployeeInfo(string email)
        {
            return repoMasEmployee.Query().Filter(x => x.Email.ToLower().Trim() == email.ToLower().Trim()).GetSaral().FirstOrDefault();
        }
        public List<LevDetails> GetLeaveDetailsByAttendanceId(int? AttendanceId)
        {
            return repoLevDetails.Query().Filter(x => x.Empid == AttendanceId).GetSaral().ToList();
        }
        public List<LevDetails> GetLeaveDetailsByLeaveDate(int? AttendanceId, DateTime startDate, DateTime endDate)
        {
            return repoLevDetails.Query().Filter(x => x.Empid == AttendanceId && x.Leavedate >= startDate && x.Leavedate <= endDate).GetSaral().ToList();
        }

        public List<AttDefinition> GetLeaveTypeList()
        {
            return repoAttDefinition.Query().Filter(x => x.Levid != (int)Enums.LeaveCategory.Holiday && x.Levid != (int)Enums.LeaveCategory.WeekOff && x.Levid != (int)Enums.LeaveCategory.OnOfficialDuty).GetSaral().ToList();
        }
        public List<AttDefinition> GetLeaveTypeListByGender(string gender)
        {
            if (gender.Equals("M"))
            {
                return repoAttDefinition.Query().Filter(x => x.Levid != (int)Enums.LeaveCategory.Holiday && x.Levid != (int)Enums.LeaveCategory.WeekOff && x.Levid != (int)Enums.LeaveCategory.MaternityLeave && x.Levid != (int)Enums.LeaveCategory.OnOfficialDuty).GetSaral().ToList();
            }
            else if (gender.Equals("F"))
            {
                return repoAttDefinition.Query().Filter(x => x.Levid != (int)Enums.LeaveCategory.Holiday && x.Levid != (int)Enums.LeaveCategory.WeekOff && x.Levid != (int)Enums.LeaveCategory.PaternityLeave && x.Levid != (int)Enums.LeaveCategory.OnOfficialDuty).GetSaral().ToList();
            }
            return repoAttDefinition.Query().Filter(x => x.Levid != (int)Enums.LeaveCategory.Holiday && x.Levid != (int)Enums.LeaveCategory.WeekOff && x.Levid != (int)Enums.LeaveCategory.OnOfficialDuty).GetSaral().ToList();

        }
        public void Save(LevDetails entity)
        {
            if (entity.Empid != 0)
            {
                repoLevDetails.ChangeEntityStateSaral<LevDetails>(entity, ObjectState.Added);
                repoLevDetails.InsertGraphSaral(entity);
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
                repoLevDetails.ChangeEntityStateSaral<LevDetails>(entity, ObjectState.Deleted);
                repoLevDetails.DeleteSaral(entity);
            }
        }
        public System.Data.DataTable GetLeaveBalance(int? AttendanceId, int monthYear)
        {
            string ConString = configuration.GetConnectionString("saralConnection");
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
        public void SetUserDesignation(string EMPID,string EMPNAME,string DESIGNATION,string EMAILID,DateTime EFFFROM,string DBNAME)
        {
            using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("saralConnection")))
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

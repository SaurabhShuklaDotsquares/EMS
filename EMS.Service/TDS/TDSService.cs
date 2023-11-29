using EMS.Core;
using EMS.Data.Model;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace EMS.Service
{
    public class TDSService : ITDSService
    {
        private IRepository<TdsdeductionDoc> repoTdsdeductionDoc;
        private IRepository<TdsempDeduction> repoTDSEmpDeduction;
        private IRepository<TdsdeductionType> _repoTdsdeductionType;
        private IRepository<TdsassesmentYear> repoTdsassesmentYear;
        private IRepository<Tdstype> repoTdstype;
        public TDSService(IRepository<TdsassesmentYear> repoTdsassesmentYear, IRepository<TdsdeductionType> repoTdsdeductionType, IRepository<Tdstype> repoTdstype, IRepository<TdsempDeduction> repoTDSEmpDeduction, IRepository<TdsdeductionDoc> repoTdsdeductionDoc)
        {
            this.repoTdsassesmentYear = repoTdsassesmentYear;
            this._repoTdsdeductionType = repoTdsdeductionType;
            this.repoTdstype = repoTdstype;
            this.repoTDSEmpDeduction = repoTDSEmpDeduction;
            this.repoTdsdeductionDoc = repoTdsdeductionDoc;
        }
        #region Get Task List
        public List<TdsdeductionType> GetTdsDeductionType(int? id)
        {
            if (id != null)
            {
                return _repoTdsdeductionType.Query().Filter(x => x.IsActive && x.DeductionTypeId == id).Get().ToList();

            }
            else
            {
                return _repoTdsdeductionType.Query().Filter(x => x.IsActive).Get().ToList();

            }
        }
        public List<TdsassesmentYear> GetAssesmentYear(int? id, string yearRange = "")
        {
            var expr = PredicateBuilder.True<TdsassesmentYear>();
            expr = expr.And(e => e.IsActive == true);
            if (id > 0 && id != null)
            {
                expr = expr.And(x => x.AssesmentYearId == id);
            }
            if (!string.IsNullOrEmpty(yearRange))
            {
                expr = expr.And(x => x.YearRange == yearRange);

            }
            return repoTdsassesmentYear.Query().Filter(expr).Get().ToList();

        }
        public TdsassesmentYear GetAssesmentYearActive()
        {
            return repoTdsassesmentYear.Query().Get().Where(x => x.IsActive == true).FirstOrDefault();
        }
        public List<Tdstype> GetTDSType(int? id)
        {

            return repoTdstype.Query().Filter(x => x.IsActive).Get().ToList();

        }

        public TdsempDeduction SaveTDSEmp(TdsempDeduction model, bool isAcUser, out bool IsExist)
        {
            TdsempDeduction TdsObj = new TdsempDeduction();
            var existRec = repoTDSEmpDeduction.Query().Filter(x => x.Uid == model.Uid
            && x.DeductionTypeId == model.DeductionTypeId
            && x.AssesmentYearId == model.AssesmentYearId).Get().FirstOrDefault();

            
            if (existRec != null)
            {
                TdsObj = repoTDSEmpDeduction.FindById(existRec.EmpDeductionId);
                model.EmpDeductionId = TdsObj.EmpDeductionId;
                if (isAcUser)
                {
                    model.ClaimedByEmployee = TdsObj.ClaimedByEmployee;
                    model.EmployeeRemark = TdsObj.EmployeeRemark;
                    model.ModifyByEmp = TdsObj.ModifyByEmp;
                    model.ModifyDateEmp = TdsObj.ModifyDateEmp;
                }
                else
                {
                   // model.GivenByEmployer = TdsObj.GivenByEmployer;
                    //model.EmployerRemark = TdsObj.EmployerRemark;
                    //model.ModifyDateAc = TdsObj.ModifyDateAc;
                    model.ModifyDateAc = DateTime.Now;
                    model.ModifyByAc = TdsObj.ModifyByAc;

                }
                TdsObj.IsActive = true;
                repoTDSEmpDeduction.Update(TdsObj, model);
                repoTDSEmpDeduction.SaveChanges();
                IsExist = false;
            }
            else
            {
                repoTDSEmpDeduction.Insert(model);
                IsExist = true;
            }
            return model;
        }

        public List<TdsempDeduction> UpdateTDSLockUnlock(List<TdsempDeduction> model)
        {
            repoTDSEmpDeduction.UpdateCollection(model);
            repoTDSEmpDeduction.SaveChanges();
            return model;
        }
        public TdsdeductionDoc SaveTdsdeductionDoc(TdsdeductionDoc model)
        {
            repoTdsdeductionDoc.Insert(model);
            return model;
        }
        public TdsdeductionDoc UpdateTdsdeductionDoc(TdsdeductionDoc model)
        {
            repoTdsdeductionDoc.Update(model);
            return model;
        }
        public bool DeleteTdsdeductionDoc(int TdsDecId)
        {
            var repoDoc = repoTdsdeductionDoc.Query().Filter(x => x.EmpDeductionId == TdsDecId).Get().ToList();
            if(repoDoc != null && repoDoc.Any())
            {
                repoTdsdeductionDoc.DeleteBulk(repoDoc);
                repoTdsdeductionDoc.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteTdsEmpDeductiondoc(int TdsDecId)
        {
            var repoDoc = repoTDSEmpDeduction.Query().Filter(x => x.EmpDeductionId == TdsDecId).Get().ToList();
            if (repoDoc != null && repoDoc.Any())
            {
                repoTDSEmpDeduction.DeleteBulk(repoDoc);
                repoTDSEmpDeduction.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<TdsempDeduction> GetTdsempDeductionByPaging(out int total, PagingService<TdsempDeduction> pagingSerices)
        {
               var records = repoTDSEmpDeduction.Query().Include(x => x.TdsdeductionDoc)
                .Filter(pagingSerices.Filter)
                 .OrderBy(pagingSerices.Sort)
                 .Get()
                 //GetPage(pagingSerices.Start, pagingSerices.Length, out total)
                 .GroupBy(x => x.Uid).Select(x => x.OrderByDescending(y => y.ModifyDateAc).FirstOrDefault())
                 //.ToList()
                 //.OrderByDescending(x=>x.ModifyDateAc.HasValue? x.ModifyDateAc.Value:x.ModifyDateEmp)
                 .ToList();
              total = records.Count();

            return records
                           .Skip((pagingSerices.Start - 1) * pagingSerices.Length)
                           .Take(pagingSerices.Length)
                           .ToList();

        }

        public List<TdsempDeduction> GetTdsempDeductionForExcelReport(int TDSTypeId)
        {
            List<TdsempDeduction> list = new List<TdsempDeduction>();

            if (TDSTypeId == (int)Enums.TdsType.HRADedution13A)
            {
                list = repoTDSEmpDeduction.Query()
                                      .Include(x => x.TdsdeductionDoc)
                                      .Filter(x => x.Is13Ahradedution == true)
                                      .Get()
                                      .ToList();
            }
            else if (TDSTypeId == (int)Enums.TdsType.HouseLoan24B)
            {
                list = repoTDSEmpDeduction.Query()
                                      .Include(x => x.TdsdeductionDoc)
                                      .Filter(x => x.Is13Ahradedution == true)
                                      .Get()
                                      .ToList();
            }

            return list;
        }
        public List<TdsempDeduction> GetTdsempDeductionByUIDAndYear(int EmpId, int AssesmentYearId)
        {
            return repoTDSEmpDeduction.Query().Filter(x => x.Uid == EmpId && x.AssesmentYearId == AssesmentYearId).Get().ToList();
        }
        public TdsempDeduction EmpDeductionByUIDAndYear(int EmpId, int AssesmentYearId)
        {
            return repoTDSEmpDeduction.Query().Get().Where(x => x.Uid == EmpId && x.AssesmentYearId == AssesmentYearId).FirstOrDefault();
        }

        public List<TDSDeductionModel> GetTdsEmpDedDocByUIDAndYear(int EmpId, int AssesmentYearId)
        {

            var list = (from s in repoTDSEmpDeduction.Query().Filter(x => x.Uid == EmpId && x.AssesmentYearId == AssesmentYearId).Get().ToList()
                        select new TDSDeductionModel
                        {
                            TdsempDeduction = s,
                            TdsDoc = repoTdsdeductionDoc.Query().Filter(x => x.EmpDeductionId == s.EmpDeductionId).Get().ToList()

                        }).ToList();

            return list;
        }

        public List<TdsdeductionDoc> GetTdsDocList(int TdsDedId)
        {
            return repoTdsdeductionDoc.Query().Filter(x => x.EmpDeductionId == TdsDedId).Get().ToList();
        }

        public TdsdeductionDoc GetTdsDocListByUidAndAssesmentYearId(int UID , int AssesmentYearId)
        {
            return repoTdsdeductionDoc.Query()
                .Filter(x => x.Uid == UID && x.EmpDeduction.AssesmentYearId == AssesmentYearId)
                .Get().FirstOrDefault();
        }


        public TdsempDeduction GetTdsempDeductionById(int Uid)
        {
            return repoTDSEmpDeduction.Query().Filter(x => x.Uid == Uid).Get().FirstOrDefault();
        }

        public List<TdsdeductionDoc> GetTdsDocListByUidAssesmentYear(int Uid, int AssesmentYearId)
        {
            var TDSEmpList = repoTDSEmpDeduction.Query().Filter(x => x.Uid == Uid && x.AssesmentYearId == AssesmentYearId).Get().Select(x => x.EmpDeductionId).ToList();
            return repoTdsdeductionDoc.Query().Filter(x => TDSEmpList.Contains(x.EmpDeductionId)).Get().ToList();

        }
        public bool DeleteDoc(int id)
        {
            try
            {
                repoTdsdeductionDoc.Delete(id);
                repoTdsdeductionDoc.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }

        }

        public List<TdsempDeduction> GetLastModifiedEmployeeList()
        {
            return repoTDSEmpDeduction.Query().Get()
                .GroupBy(x => x.ModifyByEmp).Select(x => x.OrderBy(y => y.ModifyDateAc).FirstOrDefault()).ToList();
        }

        public bool WriteLog(int UID , int AssesmentYearId)
        {
            

            string[] lines = { "EmpDeductionID", "DeductionTypeID", "TypeID", "UID", "StatusID", "AssesmentYearId", "OtherDeduction", "ClaimedByEmployee", "GivenByEmployer", "EmployeeRemark", "EmployerRemark", "CreatedDate", "ModifyDateEmp", "ModifyDateAc", "CreatedBy", "ModifyBy", "IsActive" };
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var TdsList = repoTDSEmpDeduction.Query().Filter(x => x.Uid == UID && x.AssesmentYearId == AssesmentYearId).Get().ToList();
            // Write the string array to a new file named "WriteLines.txt".
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/TDS/"+ TdsList.FirstOrDefault().Uid.ToString() +"_"+TdsList.FirstOrDefault().U.Name);
            //string path = Path.Combine(@"D:\Local\EMSWebCore\EMS.Website", "wwwroot/Upload/TDS/" + TdsList.FirstOrDefault().Uid.ToString() + "_" + TdsList.FirstOrDefault().U.Name);

        
        DataTable dt = new DataTable();

            foreach (var items in lines)
            {
                dt.Columns.Add(items);

            }
            foreach (var item in TdsList)
            {
                dt.Rows.Add(item.EmpDeductionId, (item.DeductionType == null ? null : item.DeductionType.DeductionTypeName),( item.Type == null ? null : item.Type.TypeName), item.StatusId, item.AssesmentYear.YearRange, (item.OtherDeduction == string.Empty ? "" : item.OtherDeduction), item.ClaimedByEmployee, item.GivenByEmployer, item.EmployeeRemark, item.EmployerRemark, item.CreatedDate, (item.ModifyDateEmp, item.ModifyDateAc == null ? item.ModifyDateEmp : item.ModifyDateAc), item.CreatedByNavigation.Name, "", item.IsActive);

            }
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (!File.Exists(Path.Combine(path, "UpdateLog.txt")))
            {


                File.Create(Path.Combine(path, "UpdateLog.txt")).Dispose();

            }
            using (StreamWriter sw = File.AppendText(Path.Combine(path, "UpdateLog.txt")))
            {
                string txt = string.Empty;
                foreach (DataColumn column in dt.Columns)
                {
                    txt += column.ColumnName + "\t\t";
                }
                txt += "\r\n";
                foreach (DataRow row in dt.Rows)
                {
                    int i = 0;
                    foreach (DataColumn column in dt.Columns)
                    {
                         
                            txt +=  row[column.ColumnName] + "\t\t";

                        i++;
                    }
                    txt += "\r\n";
                }
                string error = "Log Written Date:" + " " + DateTime.Now.ToString();
                sw.WriteLine("----------------------------"+ error+"---------------------------------------------------------");
                sw.WriteLine(txt);

                sw.WriteLine("--------------------------------*End*------------------------------------------");
                sw.Flush();
                sw.Close();

            }
            return true;
        }
        #endregion

        

    }
}

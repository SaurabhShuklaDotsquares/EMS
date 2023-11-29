using EMS.Data.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service
{
    public interface ITDSService 
    {
        List<TdsdeductionType> GetTdsDeductionType(int ? id);
        List<TdsassesmentYear> GetAssesmentYear(int ? id, string yearRange = "");
        List<Tdstype> GetTDSType(int ? id);
        TdsempDeduction SaveTDSEmp(TdsempDeduction TdsempDeduction,bool isAcUser , out bool IsExist);
        TdsdeductionDoc SaveTdsdeductionDoc(TdsdeductionDoc TdsdeductionDoc);
        bool DeleteTdsdeductionDoc(int TdsDecId);

        List<TdsempDeduction> GetTdsempDeductionByPaging(out int total, PagingService<TdsempDeduction> pagingSerices);
        List<TdsempDeduction> GetTdsempDeductionByUIDAndYear(int EmpId, int AssesmentYearId);
        TdsempDeduction EmpDeductionByUIDAndYear(int EmpId, int AssesmentYearId);
        List<TDSDeductionModel> GetTdsEmpDedDocByUIDAndYear(int EmpId, int AssesmentYearId);
        List<TdsdeductionDoc> GetTdsDocList(int TdsDedId);
        List<TdsdeductionDoc> GetTdsDocListByUidAssesmentYear(int Uid, int AssesmentYearId);

        bool DeleteDoc(int id);

        bool WriteLog(int UID, int AssesmentYearId);

        List<TdsempDeduction> UpdateTDSLockUnlock(List<TdsempDeduction> model);
        List<TdsempDeduction> GetLastModifiedEmployeeList();
        TdsempDeduction GetTdsempDeductionById(int Uid);
        TdsdeductionDoc GetTdsDocListByUidAndAssesmentYearId(int UID, int AssesmentYearId);
        TdsdeductionDoc UpdateTdsdeductionDoc(TdsdeductionDoc model);
        List<TdsempDeduction> GetTdsempDeductionForExcelReport(int TDSTypeId);
        bool DeleteTdsEmpDeductiondoc(int TdsDecId);
        TdsassesmentYear GetAssesmentYearActive();
    }
}

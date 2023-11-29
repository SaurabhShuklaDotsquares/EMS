using EMS.Data;
using EMS.Dto;
using System.Collections.Generic;

namespace EMS.Service
{
    public interface IInvestmentService
    {
        List<FinancialYear> GetFinancialYears();
        FinancialYear GetCurrentFinancialYear();

        List<InvestmentType> GetInvestmentTypes(int financialYearId);
        List<Investment> GetinvestmentByPaging(out int total, PagingService<Investment> pagingSerices);
        Investment Save(InvestmentDto model);
        Investment GetinvestmentById(int id);

        Investment GetinvestmentByUserAndFY(int userId, int financialYearId);
    }
}

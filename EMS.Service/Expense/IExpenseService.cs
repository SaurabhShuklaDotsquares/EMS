using System;
using System.Collections.Generic;
using EMS.Data;
using EMS.Dto;
using System.Linq.Expressions;

namespace EMS.Service
{
    public interface IExpenseService : IDisposable
    {
        Expense GetExpenseById(int id);

        Expense Save(ExpenseDto model);
        
        void UpdateStatus(ExpenseApproveDto model);

        void MarkAsReimbursed(ExpenseDto model);

        void MarkAsReimbursed(ExpenseReimburserdDto model);

        List<Expense> GetExpensesByPaging(out int total, PagingService<Expense> pagingService);

        List<ExpenseSummary> GetExpensesSummary(Expression<Func<Expense, bool>> filters);

        void DeleteExpenses(int id);
    }
}
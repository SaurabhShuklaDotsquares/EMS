using System.Collections.Generic;
using System.Linq;
using EMS.Data;
using EMS.Repo;
using EMS.Dto;
using System;
using EMS.Core;
using System.Linq.Expressions;

namespace EMS.Service
{
    public class ExpenseService : IExpenseService
    {
        #region "Fields"

        private readonly IRepository<Expense> repoExpense;
        private readonly IRepository<UserLogin> repoUserLogin;

        #endregion

        #region "Cosntructor"

        public ExpenseService(IRepository<Expense> _repoExpense,
            IRepository<UserLogin> _repoUserLogin)
        {
            repoExpense = _repoExpense;
            repoUserLogin = _repoUserLogin;
        }

        #endregion

        public Expense GetExpenseById(int id)
        {
            return repoExpense.FindById(id);
        }

        public Expense Save(ExpenseDto model)
        {

            Expense expense = null;
            if (model.Id > 0)
            {
                expense = GetExpenseById(model.Id);

                if (expense == null || expense.CreateByUid != model.CurrentUserId || expense.Status != (int)Enums.ExpensePaymentStatus.Pending)
                {
                    return null;
                }
            }
            else
            {
                expense = new Expense
                {
                    CreateByUid = model.CurrentUserId,
                    CreateDate = DateTime.Now,
                    Status = model.PaidThrough == (byte)Enums.ExpensePaymentThrough.CompanyCard ? (byte)Enums.ExpensePaymentStatus.Approved : (byte)Enums.ExpensePaymentStatus.Pending
                };
            }
            expense.CurrencyId = model.CurrencyId;
            expense.Amount = Convert.ToDecimal(model.Amount);
            expense.Descp = model.Descp;
            expense.ExpenseDate = model.ExpenseDate.ToDateTime("dd/MM/yyyy").Value;
            expense.ModifyByUid = model.CurrentUserId;
            expense.ModifyDate = DateTime.Now;
            expense.PaidThrough = model.PaidThrough;
            expense.ReceiptPath = model.ReceiptPath.HasValue() ? model.ReceiptPath : model.HasReceipt ? expense.ReceiptPath : null;
            expense.ReimburseDate = model.PaidThrough == (byte)Enums.ExpensePaymentThrough.CompanyCard ? DateTime.Now : (DateTime?)null;

            if (expense.Id > 0)
            {
                repoExpense.SaveChanges();
            }
            else
            {
                repoExpense.InsertGraph(expense);
            }

            return expense;
        }

        public void UpdateStatus(ExpenseApproveDto model)
        {
            var expenseList = repoExpense.Query().AsTracking()
                .Filter(x => model.ExpenseIds.Contains(x.Id) && x.Status == (byte)Enums.ExpensePaymentStatus.Pending)
                .Get().ToList();

            if (expenseList.Count > 0)
            {                
                foreach (var expense in expenseList)
                {                    
                    expense.Status = model.Status;
                    expense.ApprovedByUid = model.Status == (byte)Enums.ExpensePaymentStatus.Approved ? model.CurrentUserId : expense.ApprovedByUid;
                }

                repoExpense.SaveChanges();
            }
        }

        public void DeleteExpenses(int Id)
        {
            try
            {
                if (Id != 0)
                {
                    Expense model = GetExpenseById(Id);
                    if (model != null && model.Id != 0)
                    {
                        repoExpense.Delete(model.Id);
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        public void MarkAsReimbursed(ExpenseDto model)
        {
            if (model.Id > 0)
            {
                var expense = GetExpenseById(model.Id);

                if (expense == null || expense.Status != (int)Enums.ExpensePaymentStatus.Approved || expense.ReimburseDate.HasValue)
                {
                    return;
                }

                expense.ReimburseDate = model.ReimburseDate.ToDateTime("dd/MM/yyyy").Value;
                expense.ReimbursedByUid = model.CurrentUserId;

                repoExpense.SaveChanges();
            }
        }

        public void MarkAsReimbursed(ExpenseReimburserdDto model)
        {
            try
            {
                var expense = repoExpense.Query().Filter(x => model.ExpenseIds.Contains(x.Id)).Get();

                foreach (var item in expense)
                {
                    if (item == null || item.Status != (int)Enums.ExpensePaymentStatus.Approved || item.ReimburseDate.HasValue)
                    {
                        return;
                    }

                    item.ReimburseDate = model.ReimburseDate.ToDateTime("dd/MM/yyyy").Value;
                    item.ReimbursedByUid = model.CurrentUserId;
                    repoExpense.ChangeEntityState<Expense>(item, ObjectState.Modified);
                }
                repoExpense.SaveChanges();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<Expense> GetExpensesByPaging(out int total, PagingService<Expense> pagingService)
        {
            return repoExpense.Query()
                .Filter(pagingService.Filter).
                OrderBy(pagingService.Sort).
                GetPage(pagingService.Start, pagingService.Length, out total).
                ToList();
        }

        public List<ExpenseSummary> GetExpensesSummary(Expression<Func<Expense, bool>> filters)
        {
            var summaries = repoExpense
                      .Query()
                      .Filter(filters)
                      .GetQuerable()
                      .GroupBy(x => x.Status)
                      .Select(g => new ExpenseSummary
                      {
                          Status = ((Enums.ExpensePaymentStatus)g.Key).ToString(),
                          TotalSummary = g.GroupBy(x => x.CurrencyId)
                                           .Select(y => y.FirstOrDefault().Currency.CurrSign + " " + y.Sum(a => a.Amount)).ToList(),
                          CompanyCardSummary = g.Where(x => x.PaidThrough == (byte)Enums.ExpensePaymentThrough.CompanyCard)
                                             .GroupBy(x => x.CurrencyId)
                                             .Select(y => y.FirstOrDefault().Currency.CurrSign + " " + y.Sum(a => a.Amount)).ToList(),
                          CashOrPersonalCardSummary = g.Where(x => x.PaidThrough == (byte)Enums.ExpensePaymentThrough.CashOrPersonalCard)
                                                    .GroupBy(x => x.CurrencyId)
                                                    .Select(y => y.FirstOrDefault().Currency.CurrSign + " " + y.Sum(a => a.Amount)).ToList()
                      }).OrderBy(x => x.Status).ToList();



            return summaries;

        }

        #region "Dispose"

        public void Dispose()
        {
            if (repoExpense != null)
            {
                repoExpense.Dispose();
            }

            if (repoUserLogin != null)
            {
                repoUserLogin.Dispose();
            }
        }

        #endregion
    }
}

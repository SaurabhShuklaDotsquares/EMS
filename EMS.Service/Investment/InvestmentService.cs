using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EMS.Service
{
    public class InvestmentService : IInvestmentService
    {
        IRepository<InvestmentType> repoempInvestmentType;
        IRepository<Investment> repoInvestment;
        IRepository<FinancialYear> repoFinancialYear;

        public InvestmentService(IRepository<InvestmentType> repoempInvestmentType, IRepository<Investment> repoInvestment, IRepository<FinancialYear> repoFinancialYear)
        {
            this.repoempInvestmentType = repoempInvestmentType;
            this.repoInvestment = repoInvestment;
            this.repoFinancialYear = repoFinancialYear;
        }

        public List<InvestmentType> GetInvestmentTypes(int financialYearId)
        {
            return repoempInvestmentType.Query().Filter(i => i.FinancialYearId == financialYearId).Get().ToList();
        }

        public List<FinancialYear> GetFinancialYears()
        {
            return repoFinancialYear.Query().Get().ToList();
        }

        public FinancialYear GetCurrentFinancialYear()
        {
            return repoFinancialYear.Query()
                .OrderBy(o => o.OrderByDescending(x => x.Id))
                .GetQuerable().FirstOrDefault();
        }

        public List<Investment> GetinvestmentByPaging(out int total, PagingService<Investment> pagingSerices)
        {
            return repoInvestment.Query().Filter(pagingSerices.Filter).
                OrderBy(pagingSerices.Sort).
                GetPage(pagingSerices.Start, pagingSerices.Length, out total).
                ToList();
        }

        public Investment Save(InvestmentDto model)
        {
            try
            {
                Investment investmentEntity = null;
                var currentDateTime = DateTime.Now;
                if (model.Id > 0)
                {
                    investmentEntity = GetinvestmentById(model.Id);

                    if (investmentEntity != null && investmentEntity.UserloginId != model.CurrentUserId && !investmentEntity.IsDraft)
                    {
                        return null;
                    }
                }
                else
                {
                    investmentEntity = GetinvestmentByUserAndFY(model.CurrentUserId, model.FinancialYearId);

                    if (investmentEntity != null)
                    {
                        return null;
                    }

                    investmentEntity = new Investment
                    {
                        UserloginId = model.CurrentUserId,
                        FinancialYearId = 1,
                        CreateDate = currentDateTime
                    };
                }

                if (investmentEntity != null)
                {
                    investmentEntity.HomeAddress = model.HomeAddress;
                    investmentEntity.Name = model.Name;
                    investmentEntity.FatherName = model.FatherName;
                    investmentEntity.AttendanceCode = model.AttendanceCode;
                    investmentEntity.PAN = Convert.ToString(model.PAN).ToUpper();
                    investmentEntity.DOB = model.DOB.ToDateTime("dd/MM/yyyy").Value;
                    investmentEntity.ModifyDate = currentDateTime;
                    investmentEntity.IsDraft = model.IsDraft;
                    if (investmentEntity.Id > 0)
                    {
                        repoInvestment.ChangeEntityCollectionState(investmentEntity.InvestmentMonths, ObjectState.Deleted);
                        repoInvestment.ChangeEntityCollectionState(investmentEntity.InvestmentTypeAmountMaps, ObjectState.Deleted);
                    }

                    var rents = model.InvestmentMonths.Where(m => m.MonthlyRent.HasValue && m.MonthlyRent.Value > 0);
                    foreach (var rent in rents)
                    {
                        investmentEntity.InvestmentMonths.Add(new InvestmentMonth()
                        {
                            InvMonth = rent.InvMonth,
                            InvYear = rent.InvYear,
                            MonthlyRent = rent.MonthlyRent.Value,
                            ModifyDate = currentDateTime
                        });
                    }

                    var investmentTypes = model.InvestmentTypeAmountMaps.Where(t => t.InvestmentTypeId > 0 && t.Amount.HasValue && t.Amount.Value > 0);
                    foreach (var type in investmentTypes)
                    {
                        investmentEntity.InvestmentTypeAmountMaps.Add(new InvestmentTypeAmountMap()
                        {
                            Amount = type.Amount.Value,
                            InvestmentTypeId = type.InvestmentTypeId,
                            ModifyDate = currentDateTime,
                        });
                    }

                    var incomeTypes = model.IncomeTypeAmountMaps.Where(t => t.InvestmentTypeId > 0 && t.Amount.HasValue && t.Amount.Value > 0);
                    foreach (var type in incomeTypes)
                    {
                        investmentEntity.InvestmentTypeAmountMaps.Add(new InvestmentTypeAmountMap()
                        {
                            Amount = type.Amount.Value,
                            InvestmentTypeId = type.InvestmentTypeId,
                            ModifyDate = currentDateTime,
                        });
                    }

                    if (model.InvestmentDocuments.Any(x => x.Id == 0))
                    {
                        var docs = model.InvestmentDocuments.Where(x => x.Id == 0 && !string.IsNullOrWhiteSpace(x.DocumentUrl));
                        foreach (var doc in docs)
                        {
                            investmentEntity.InvestmentDocuments.Add(new InvestmentDocument()
                            {
                                CreateDate = currentDateTime,
                                ModifyDate = currentDateTime,
                                DocumentName = doc.DocumentName,
                                DocumentPath = doc.DocumentUrl,
                                InvestmentTypeId = doc.DocumentTypeId > 0 ? doc.DocumentTypeId : (int?)null
                            });
                        }
                    }

                    if (model.Id > 0)
                    {
                        var removedDocs = investmentEntity.InvestmentDocuments.Where(x => x.Id > 0).Select(x => x.Id)
                                                .Except(model.InvestmentDocuments.Where(x => x.Id > 0).Select(x => x.Id));

                        if (removedDocs.Any())
                        {
                            repoInvestment.ChangeEntityCollectionState(investmentEntity.InvestmentDocuments.Where(x => removedDocs.Contains(x.Id)).ToList(), ObjectState.Deleted);
                        }

                        repoInvestment.SaveChanges();
                    }
                    else
                    {
                        repoInvestment.InsertGraph(investmentEntity);
                    }
                }

                return investmentEntity;
            }
            catch
            {
                throw;
            }
        }

        public Investment GetinvestmentById(int id)
        {
            return repoInvestment.FindById(id);
        }

        public Investment GetinvestmentByUserAndFY(int userId, int financialYearId)
        {
            return repoInvestment.Query()
                .Filter(x => x.UserloginId == userId && x.FinancialYearId == financialYearId)
                .OrderBy(o => o.OrderByDescending(x => x.Id))
                .GetQuerable().FirstOrDefault();
        }
    }
}

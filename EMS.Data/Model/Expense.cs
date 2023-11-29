using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Expense
    {
        public int Id { get; set; }
        public string Descp { get; set; }
        public DateTime ExpenseDate { get; set; }
        public decimal Amount { get; set; }
        public string ReceiptPath { get; set; }
        public byte PaidThrough { get; set; }
        public byte Status { get; set; }
        public int CurrencyId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateByUid { get; set; }
        public DateTime ModifyDate { get; set; }
        public int ModifyByUid { get; set; }
        public DateTime? ReimburseDate { get; set; }
        public int? ApprovedByUid { get; set; }
        public int? ReimbursedByUid { get; set; }

        public virtual UserLogin UserLogin  { get; set; }
        public virtual UserLogin UserLogin1  { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual UserLogin UserLogin2  { get; set; }
        public virtual UserLogin UserLogin3  { get; set; }
    }
}

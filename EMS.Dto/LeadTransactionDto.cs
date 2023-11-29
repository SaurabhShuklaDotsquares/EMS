using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Dto
{
    public class LeadTransactionDto
    {
        public string LeadId{ get; set; }
        public string  NextChaseDate { get; set; }
        public List<TransactionDto> LeadTransactionList { get; set; }
        public LeadTransactionDto()
        {
            LeadTransactionList = new List<TransactionDto>();
        }
    }

    public class TransactionDto
    {
        public int SNo { get; set; }
        public string TrasnactionDescrtiption { get; set; }
        public string Document { get; set; }
        public string DocumentName { get; set; }
        public string Status { get; set; }
        public string AddedBy { get; set; }
        public string AddedDate { get; set; }
    }
}

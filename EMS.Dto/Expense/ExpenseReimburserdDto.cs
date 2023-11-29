using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Dto
{
    public class ExpenseReimburserdDto
    {

        public int[] ExpenseIds { get; set; }

        [DisplayName("Payment Date")]
        public string ReimburseDate { get; set; }

        public int CurrentUserId { get; set; }
        
    }
}

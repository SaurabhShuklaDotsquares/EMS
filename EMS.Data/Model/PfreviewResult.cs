using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class PfreviewResult
    {
        public int Id { get; set; }
        public int PfreviewSubmittedId { get; set; }
        public int PfreviewQuestionId { get; set; }
        public byte PfreviewAnswer { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public bool? IsActive { get; set; }

        public virtual PfreviewQuestion PfreviewQuestion { get; set; }
        public virtual PfreviewSubmitted PfreviewSubmitted { get; set; }
    }
}

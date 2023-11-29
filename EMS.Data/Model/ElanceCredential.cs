using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ElanceCredential
    {
        public int Id { get; set; }
        public string ElanceClientId { get; set; }
        public string ElanceClientSecret { get; set; }
        public string ElanceCode { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string RedirectUri { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}

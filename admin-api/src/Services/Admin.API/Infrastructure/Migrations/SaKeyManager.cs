using System;
using System.Collections.Generic;

namespace Admin.API.Infrastructure.Migrations
{
    public partial class SaKeyManager
    {
        public decimal Id { get; set; }
        public string Username { get; set; }
        public string Bearertoken { get; set; }
        public string Idpermissiontoken { get; set; }
        public decimal Isvalid { get; set; }
        public DateTime? Createdondate { get; set; }       
    }
}

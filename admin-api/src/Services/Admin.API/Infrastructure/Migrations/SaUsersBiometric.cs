using System;
using System.Collections.Generic;

namespace Admin.API.Infrastructure.Migrations
{
    public partial class SaUsersBiometric
    {
        public decimal Id { get; set; }
        public string Username { get; set; }
        public string Device_id { get; set; }
        public string Device_info { get; set; }
        public DateTime? Lastlogindate { get; set; }
        public string Ip { get; set; }
        public string Bm1 { get; set; }
        public string Bm2 { get; set; }
        public string Bm3 { get; set; }
        public string Bm4 { get; set; }
        public string Bm5 { get; set; }
        public string Bm6 { get; set; }
        public string Bm7 { get; set; }
        public string Bm8 { get; set; }
        public string Bm9 { get; set; }
        public string Status { get; set; }
        public string Createdby { get; set; }
        public DateTime? Createddate { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? Lastmodifieddate { get; set; }
        public string Biometric_token { get; set; }       
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }
}

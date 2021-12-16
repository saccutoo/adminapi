using System;
using System.Collections.Generic;

namespace Admin.API.Infrastructure.Migrations
{
    public partial class SaUsers
    {
        public decimal Id { get; set; }
        public decimal? Departmentid { get; set; }
        public string Lastname { get; set; }
        public string DepartmentName { get; set; }
        public string Firstname { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Passwordsalt { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public DateTime? Birthday { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public DateTime? Lastlogindate { get; set; }
        public DateTime? Lastlogoutdate { get; set; }
        public int? Failedpasscount { get; set; }
        public DateTime? Locktime { get; set; }
        public bool? Forcechangepass { get; set; }
        public string Status { get; set; }
        public DateTime? Opendate { get; set; }
        public string Maker { get; set; }
        public DateTime? Makerondate { get; set; }
        public string Approver { get; set; }
        public DateTime? Approverondate { get; set; }
        public string Ip { get; set; }
        public string Agent { get; set; }
        public string PostingCode { get; set; }
        public string Bm1 { get; set; }
        public string Bm2 { get; set; }
        public string Bm3 { get; set; }
        public string Bm4 { get; set; }
        public string Bm5 { get; set; }
        public string Bm6 { get; set; }
        public string Bm7 { get; set; }
        public string Bm8 { get; set; }
        public string Bm9 { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }
}

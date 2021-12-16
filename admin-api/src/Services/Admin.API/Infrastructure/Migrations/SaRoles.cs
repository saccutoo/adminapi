using System;
using System.Collections.Generic;

namespace Admin.API.Infrastructure.Migrations
{
    public partial class SaRoles
    {
        public decimal Id { get; set; }
        public decimal? Applicationid { get; set; }
        public string Applicationname { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? Opendate { get; set; }
        public string Maker { get; set; }
        public DateTime? Makerondate { get; set; }
        public string Approver { get; set; }
        public DateTime? Approverondate { get; set; }
        public decimal Isdefault { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Admin.API.Infrastructure.Migrations
{
    public partial class SaDepartments
    {
        public decimal Id { get; set; }
        public decimal? Parent_dept_id { get; set; }
        public string Name { get; set; }
        public string Dept_id { get; set; }
        public string Icon { get; set; }
        public string Tel { get; set; }
        public int? Orderview { get; set; }
        public string Status { get; set; }
        public string Maincd { get; set; }
        public string Poscd { get; set; }
        public string Postingcode { get; set; }
        public DateTime? Opendate { get; set; }
        public string Maker { get; set; }
        public DateTime? Makerondate { get; set; }
        public string Approver { get; set; }
        public DateTime? Approverondate { get; set; }
    }
}

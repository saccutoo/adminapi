using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.API.Infrastructure.Migrations
{
    public class SaUserGroup
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public decimal OrderView { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public string LastModifiedBy { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }
}

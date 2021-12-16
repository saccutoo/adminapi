using System;
using System.Collections.Generic;

namespace Admin.API.Infrastructure.Migrations
{
    public partial class SaUserDeparts
    {
        public decimal Id { get; set; }
        public decimal? Userid { get; set; }
        public decimal? Departmentid { get; set; }
    }
}

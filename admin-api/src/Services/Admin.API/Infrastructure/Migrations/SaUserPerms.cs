using System;
using System.Collections.Generic;

namespace Admin.API.Infrastructure.Migrations
{
    public partial class SaUserPerms
    {
        public decimal Id { get; set; }
        public decimal? Userid { get; set; }
        public decimal? Applicationid { get; set; }
        public decimal? Funcpermid { get; set; }
        public decimal? Roleid { get; set; }
    }
}

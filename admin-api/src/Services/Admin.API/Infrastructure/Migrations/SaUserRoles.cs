using System;
using System.Collections.Generic;

namespace Admin.API.Infrastructure.Migrations
{
    public partial class SaUserRoles
    {
        public long Id { get; set; }
        public long? Userid { get; set; }
        public long? Roleid { get; set; }
        public string RoleName { get; set; }
    }
}

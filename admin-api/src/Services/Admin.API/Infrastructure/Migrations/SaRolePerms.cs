using System;
using System.Collections.Generic;

namespace Admin.API.Infrastructure.Migrations
{
    public partial class SaRolePerms
    {
        public long Id { get; set; }
        public long? Funcpermid { get; set; }
        public long? Roleid { get; set; }
    }
}

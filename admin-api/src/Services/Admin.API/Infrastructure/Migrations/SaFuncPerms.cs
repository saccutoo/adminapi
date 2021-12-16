using System;
using System.Collections.Generic;

namespace Admin.API.Infrastructure.Migrations
{
    public partial class SaFuncPerms
    {
        public decimal Id { get; set; }
        public decimal? Functionid { get; set; }
        public decimal? Permissionid { get; set; }
        public string Code { get; set; }
        public decimal? Orderview { get; set; }
        public decimal? Isdefault { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FunctionName { get; set; }
        public string ApplicationName { get; set; }
        public decimal? IsDisable { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }
}

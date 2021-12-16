using System;
using System.Collections.Generic;

namespace Admin.API.Infrastructure.Migrations
{
    public partial class SaFunctions
    {
        public decimal Id { get; set; }
        public decimal? Applicationid { get; set; }
        public decimal? Parentid { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public string CssClass { get; set; }
        public decimal? Orderview { get; set; }
        public decimal? Ispublic { get; set; }
        public string Status { get; set; }
        public string NavigationMobile { get; set; }
        public decimal? Level_fun { get; set; }
        public DateTime? Opendate { get; set; }
        public string Maker { get; set; }
        public DateTime? Makerondate { get; set; }
        public string Approver { get; set; }
        public DateTime? Approverondate { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
        public string ApplicationName { get; set; }
        public string ParentName { get; set; }
    }
}

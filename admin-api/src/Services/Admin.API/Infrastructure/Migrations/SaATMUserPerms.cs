using System;
using System.Collections.Generic;

namespace Admin.API.Infrastructure.Migrations
{
    public partial class SaATMUserPerms
    {
        public decimal Id { get; set; }
        public string FullName { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
        public string PosCD { get; set; }
        public string Pos_CD { get; set; }      //dùng để get info từ oos cho chấm điểm kpi kế toán
        public string PosName { get; set; }
        public string ListPosCD { set; get; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }
}

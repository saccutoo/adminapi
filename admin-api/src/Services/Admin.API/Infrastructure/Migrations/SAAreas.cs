using System;
using Utils;

namespace UTILS.API.Infrastructure.Migrations
{
    public class SAAreas
    {
        public decimal Id { get; set; }
        public decimal ParentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal OrderView { get; set; }
        public string Status { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string ApproverBy { get; set; }
        public DateTime ApproverDate { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }
}

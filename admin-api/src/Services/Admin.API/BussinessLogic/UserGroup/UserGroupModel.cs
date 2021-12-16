using System;
using System.Collections.Generic;
using Utils;

namespace API.BussinessLogic
{
    public class UserGroupModel
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
        public List<decimal> ListRoleId { get; set; }
        public bool IsChecked { get; set; } = false;
    }
    public class UserGroupCreateModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public decimal OrderView { get; set; }
        public string CreatedBy { get; set; }
        public List<decimal> ListRoleId { get; set; }
    }
    public class UserGroupUpdateModel
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public decimal OrderView { get; set; }
        public string LastModifiedBy { get; set; }
        public List<decimal> ListRoleId { get; set; }
    }
    public class UserGroupQueryModel : PaginationRequest
    {
        public string Status { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public class RolePermsBaseModel
    {
        public long Id { get; set; }
        public long? FuncpermId { get; set; }
        public long? RoleId { get; set; }
    }

    public class RolePermsCreateModel
    {
        public long? FuncpermId { get; set; }
        public long? RoleId { get; set; }
    }

    public class RolePermsQueryModel : PaginationRequest
    {
        public decimal? IsPublic { get; set; }
        public string Status { get; set; }
    }

    public class RolePermsResponseModel
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public class UserRolesBaseModel
    {
        public decimal Id { get; set; }
        public decimal? UserId { get; set; }
        public decimal? RoleId { get; set; }
    }

    public class UserRolesViewModel : UserRolesBaseModel
    {
        public string RoleName { get; set; }
    }

    public class UserRolesCreateModel 
    {
        public decimal? UserId { get; set; }
        public decimal? RoleId { get; set; }
    }

    public class UserRolesQueryModel : PaginationRequest
    {
        public decimal? IsPublic { get; set; }
        public string Status { get; set; }
    }

    public class UserRolesResponseModel 
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}

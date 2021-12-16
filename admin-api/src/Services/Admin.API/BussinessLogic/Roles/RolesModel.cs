using System.Collections.Generic;
using Utils;

namespace API.BussinessLogic
{
    public class RolesBaseModel : BaseModel
    {
        public decimal Id { get; set; }
        public decimal? ApplicationId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public decimal IsDefault { get; set; }
    }

    public class RolesViewModel : RolesBaseModel
    {
        public string ApplicationName { get; set; }
    }

    public class RolesCreateModel 
    {
        public decimal? ApplicationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public List<decimal> ListFuncPermsId { get; set; }
    }
    public class RolesUpdateModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public List<decimal> ListFuncPermsId { get; set; }
    }
    public class RolesQueryModel : PaginationRequest
    {
        public decimal? ApplicationId { get; set; }
        public decimal? IsPublic { get; set; }
        public string Status { get; set; }
    }

    public class RoleResponseModel 
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}

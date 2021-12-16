using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public class UserPermisionBaseModel 
    {
        public decimal Id { get; set; }
        public decimal? UserId { get; set; }
        public decimal? ApplicationId { get; set; }
        public decimal? FuncpermId { get; set; }
        public decimal? RoleId { get; set; }
    }

    public class UserPermisionCreateModel
    {
        public decimal? Userid { get; set; }
        public decimal? Applicationid { get; set; }
        public decimal? Funcpermid { get; set; }
        public decimal? RoleId { get; set; }
    }

    public class UserPermisionUpdateModel
    {
        public string Maker { get; set; }
        public List<UserPermisionCreateModel> ListFuncPerms { get; set; }
    }
    public class UserPermisioUpdateModel
    {
        public List<UserPermisionCreateModel> UpdateModel { get; set; }
    }
    public class UserPermisionResponseModel 
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }

    public class UserPermisionQueryModel : PaginationRequest
    {
        public decimal? IsPublic { get; set; }
        public string Status { get; set; }
    }

    #region Tree user permission model by Func
    public class TreeUserPermModel
    {
        public decimal? ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public bool? IsChecked { get; set; }
        public List<RoleByAppIdModel> ListRole { get; set; }
    }

    public class RoleByAppIdModel : RolesViewModel
    {
        public bool? IsChecked { get; set; }
        public List<TreeFuncPermsModel> ListFunctions { get; set; }
    }

    public class TreeFuncPermsModel : FunctionsBaseModel
    {
        public bool? HasChild { get; set; }
        public bool? HasSeperate { get; set; }
        public bool? IsChecked { get; set; }
        public List<TreeFuncPermsModel> Childs { get; set; }
        public List<FuncPermsBaseModel> ListFuncPerms { get; set; }
    }
    #endregion

    #region Tree userPermission by Perms
    public class TreeUserPerByPerms
    {
        public decimal? ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public bool? IsChecked { get; set; }
        public List<RoleByAppIdGroupByPermsModel> ListRole { get; set; }
    }

    public class RoleByAppIdGroupByPermsModel : RolesViewModel
    {
        public bool? IsChecked { get; set; }
        public List<TreeFuncPermsGroupByPermsModel> ListPerms { get; set; }
    }

    public class TreeFuncPermsGroupByPermsModel : PermissionsBaseModel
    {
        //public bool? HasChild { get; set; }
        //public bool? HasSeperate { get; set; }
        public bool? IsChecked { get; set; }
        public List<FuncPermsBaseModel> ListFuncPerms { get; set; }
    }
    #endregion
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public class FuncPermsBaseModel 
    {
        public decimal Id { get; set; }
        public decimal? FunctionId { get; set; }
        public decimal? PermissionId { get; set; }
        public decimal? OrderView { get; set; }
        public decimal? IsDefault { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsChecked { get; set; }
        public string FunctionName { get; set; }
        public string ApplicationName { get; set; }
        public decimal? IsDisable { get; set; }
    }

    public class FuncPermsCreateModel 
    {
        public decimal Id { get; set; }
        public decimal? FunctionId { get; set; }
        public decimal? PermissionId { get; set; }
        public decimal? OrderView { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? IsDefault { get; set; }
        public decimal? IsDisable { get; set; }
    }

    public class FuncPermsUpdateModel
    {
        public decimal? OrderView { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? IsDefault { get; set; }
    }
    
    #region Model tree by Func
    public class FuncPermsTreeModel
    {
        public decimal ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public List<FunctionModel> ListFunction { get; set; }
    }
    public class FunctionModel : FunctionsBaseModel
    {
        public bool? HasChild { get; set; }
        public bool? HasSeperate { get; set; }
        public bool? IsChecked { get; set; }
        public List<FunctionModel> Childs { get; set; }
        public List<FuncPermsBaseModel> ListFuncPerm { get; set; }
    }
    #endregion

    #region Model tree by Perm
    public class FuncPermsTreeByPermModel
    {
        public decimal ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public List<PermModel> ListPerm { get; set; }
    }
    public class PermModel : PermissionsBaseModel
    {
        //public bool? HasChild { get; set; }
        //public bool? HasSeperate { get; set; }
        public bool? IsChecked { get; set; }
        public List<FuncPermsBaseModel> ListFuncPerm { get; set; }
    }
    #endregion

    public class FuncPermsQueryModel : PaginationRequest
    {
        public decimal? FunctionId { get; set; }
        public int? IsPublic { get; set; }
        public string Status { get; set; }
    }
    public class FuncPermsDeleteModel
    {
        public List<decimal> ListId { get; set; }
    }

    public class FuncPermsResponseModel
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}

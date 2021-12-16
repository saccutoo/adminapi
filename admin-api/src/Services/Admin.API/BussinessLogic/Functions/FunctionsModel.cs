using System;
using System.Collections.Generic;
using Utils;

namespace API.BussinessLogic
{
    public class FunctionsBaseModel : BaseModel
    {
        public decimal Id { get; set; }
        public decimal? ApplicationId { get; set; }
        public decimal? ParentId { get; set; }
        public string Name { get; set; }
        public string NameSelect { get; set; }
        public string CssClass { get; set; }
        public string Code { get; set; }
        public string Icon { get; set; }
        public string NavigationMobile { get; set; }
        public string Url { get; set; }
        public decimal? Orderview { get; set; }
        public decimal? Ispublic { get; set; }
        public string Status { get; set; }
        public decimal? Level_Fun { get; set; }
        public string ApplicationName { get; set; }
        public string ParentName { get; set; }
    }

    public class FunctionsCreateModel
    {
        public decimal? ApplicationId { get; set; }
        public decimal? ParentId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Icon { get; set; }
        public string CssClass { get; set; }
        public string NavigationMobile { get; set; }
        public string Url { get; set; }
        public decimal? Orderview { get; set; }
        public decimal? Ispublic { get; set; }
        public string Status { get; set; }
        public decimal? Level_Fun { get; set; }
        public List<decimal> ListChanels { get; set; }
        public List<FuncPermsCreateModel> ListFuncPerms { get; set; }
    }
    public class FunctionsUpdateModel
    {
        public decimal? ApplicationId { get; set; }
        public decimal? ParentId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string CssClass { get; set; }
        public string NavigationMobile { get; set; }
        public string Url { get; set; }
        public decimal? Orderview { get; set; }
        public decimal? Ispublic { get; set; }
        public string Status { get; set; }
        public decimal? Level_Fun { get; set; }
        public List<decimal> ListChanels { get; set; }
        public List<FuncPermsCreateModel> ListFuncPerms { get; set; }
    }

    public class FunctionsTreeModel : FunctionsBaseModel
    {
        public bool? HasChild { get; set; }
        public bool? HasSeperate { get; set; }
        public List<FunctionsTreeModel> Childs { get; set; }
    }
    public class FunctionsQueryModel : PaginationRequest
    {
        public decimal? ApplicationId { get; set; }
        public decimal? IsPublic { get; set; }
        public string Status { get; set; }
    }
}

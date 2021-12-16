using UTILS.API.Infrastructure.Migrations;
using System;
using System.Collections.Generic;
using Utils;

namespace API.BussinessLogic
{
    public class SAAreasBaseModel : BaseCRUDModel
    {
        public decimal Id { get; set; }
        public decimal ParentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal OrderView { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }
    public class SAAreasViewModel : SAAreasBaseModel
    {
    }
    public class SAAreasCreateModel : BaseCRUDModel
    {
        public decimal ParentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal OrderView { get; set; }
    }

    public class SAAreasUpdateModel : BaseCRUDModel
    {
        public decimal ParentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string OrderView { get; set; }
    }
    public class SAAreasDeleteModel
    {
        public List<decimal> ListId { get; set; }
    }
    public class SAAreasApprovedModel : BaseCRUDModel
    {
        public decimal Id { get; set; }
        public string Status { get; set; }
    }
    public class SAAreasQueryModel : PaginationRequest
    {
        public string Status { get; set; }
    }
}

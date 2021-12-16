using System;
using Utils;

namespace API.BussinessLogic
{
    public class ApplicationBaseModel
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Icon { get; set; }
        public string Cssclass { get; set; }
        public string Url { get; set; }
        public decimal? OrderView { get; set; }
        public decimal? IsPublic { get; set; }
        public string Status { get; set; }
        public decimal? FuncPermsId { get; set; }
    }
    public class ApplicationCreateModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Icon { get; set; }
        public string Cssclass { get; set; }
        public string Url { get; set; }
        public decimal? OrderView { get; set; }
        public decimal? IsPublic { get; set; }
        public string Status { get; set; }
    }
    public class ApplicationQueryModel : PaginationRequest
    {
        public decimal? IsPublic { get; set; }
        public string Status { get; set; }
    }

    public class ApplicationResponseModel
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}

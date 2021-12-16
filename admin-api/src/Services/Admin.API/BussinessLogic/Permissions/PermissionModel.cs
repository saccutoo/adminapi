using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public class PermissionsBaseModel : BaseModel
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public bool IsChecked { get; set; }
        public decimal? IsDisable { get; set; }
        public decimal? OrderView { get; set; }
        public decimal? IsDefault { get; set; }
    }

    public class PermissionsCreateModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
    public class PermissonsQueryModel : PaginationRequest
    {
        public decimal? IsPublic { get; set; }
        public string Status { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public class FuncChanelsBaseModel
    {
        public decimal? Id { get; set; }
        public decimal? FunctionId { get; set; }
        public decimal? ChanelId { get; set; }
    }

    public class FuncChanelsCreateModel
    {
        public decimal? FunctionId { get; set; }
        public decimal? ChanelId { get; set; }
    }

    public class FuncChanelsQueryModel : PaginationRequest
    {
        public decimal? IsPublic { get; set; }
        public string Status { get; set; }
    }
}

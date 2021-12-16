using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Utils
{
   
    public class PaginationRequest
    {
        public string Sort { get; set; } = "+Id";
        public string Fields { get; set; }  
        [Range(1, int.MaxValue)] public int? PageIndex { get; set; } = 1;
        [Range(1, int.MaxValue)] public int? PageSize { get; set; } = 20;
        public string Filter { get; set; } = "{}";
        public string FullTextSearch { get; set; }
        public decimal? Id { get; set; }
        public List<decimal> ListId   { get; set; }
    }

}
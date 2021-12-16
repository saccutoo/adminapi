using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Utils;

namespace API.BussinessLogic
{
    public class LogActionsModel
    {
        public decimal Id { get; set; }
        public string FunctionCode { get; set; }
        public string FunctionName { get; set; }
        public string ActionType { get; set; }
        public string ActionName { get; set; }
        public string ActionByUser { get; set; }
        public DateTime? ActionTime { get; set; }
        public string ObjectContentNew { get; set; }
        public string ObjectContentOld { get; set; }
    }
    public class LogActionsQueryModel : PaginationRequest
    {
        public string ActionType { get; set; }
        public string ActionName { get; set; }
        public string ActionByUser { get; set; }
    }
}

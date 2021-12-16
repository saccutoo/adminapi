using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public class UserDepartsBaseModel
    {

        public decimal Id { get; set; }
        public decimal? Userid { get; set; }
        public decimal? Departmentid { get; set; }
        public decimal? IsHost { get; set; }
        
    }

    public class UserDepartsCreateModel 
    {
       // public decimal Id { get; set; }
        public decimal? Userid { get; set; }
        public decimal? Departmentid { get; set; }
        public decimal? IsHost { get; set; }
    }
    public class UserDepartsResponseModel 
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }

    public class UserDepartsUpdateModel 
    {
        public List<UserDepartsCreateModel> updateModel { get; set; }
    }
    public class UserDepartsQueryModel : PaginationRequest
    {
        public decimal? IsPublic { get; set; }
        public string Status { get; set; }
    }
}

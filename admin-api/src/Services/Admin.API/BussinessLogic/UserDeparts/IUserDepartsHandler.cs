using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public interface IUserDepartsHandler
    {
        Task<Response> UpdateUserInDepartment(decimal id, UserDepartsUpdateModel model);
        Task<Response> UpdateDepartmentByUserId(UserDepartsBaseModel model);
        Task<Response> CreateAsync(UserDepartsCreateModel model);
    }
}
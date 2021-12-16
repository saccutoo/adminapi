using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public interface IUserRolesHandler
    {
        Task<Response> CreateAsync(UserRolesCreateModel model);
        Task<Response> UpdateAsync(decimal id, UserRolesBaseModel model);
        Task<Response> GetAllAsync();
        Task<Response> FindAsync(decimal id);
        Task<Response> GetRolesOfUserAsync(decimal userId);
        Task<Response> GetRolesNotOfUserAsync(decimal userId);
        Task<Response> DeleteRolesByUserId(decimal userId);
        Task<Response> GetUserByRoleIdAsync(decimal roleId);
    }
}

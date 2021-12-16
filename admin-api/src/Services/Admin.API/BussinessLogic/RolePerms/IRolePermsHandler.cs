using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public interface IRolePermsHandler
    {
        Task<Response> CreateAsync(RolePermsCreateModel model);
        Task<Response> DeleteByIdAsync(decimal roleId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public interface IUserPermisionHandler
    {
        Task<Response> UpdateFunctionPermissionOfUserAsync(UserPermisioUpdateModel model);
        Task<Response> CreateAsync(UserPermisionCreateModel model);
        Task<Response> CreateAllUserIdWithFuncPerms(decimal? appId, decimal funcpermsId);
        Task<Response> UpdateAsync(decimal userId,UserPermisionUpdateModel model);
        Task<Response> DeleteUserPermsOfUser(decimal userId);
        Task<Response> DeleteUserPermsByRoleId(decimal roleId);
        Task<Response> DeleteUserPermsByListFuncPerms(string listFuncPerms);
        Task<Response> GetUserPermsByUserId(decimal? userId);
        Task<Response> GetTreeFuncPermByUserIdGroupByFunc(decimal? userId = null);
        Task<Response> GetTreeFuncPermByUserIdGroupByPerms(decimal? userId = null);
        Task<Response> GetTreeAllAppThenRole(decimal? userGroupId = null);
    }
}
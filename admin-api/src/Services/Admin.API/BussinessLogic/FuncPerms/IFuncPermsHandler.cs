using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public interface IFuncPermsHandler
    {
        Task<Response> CreateManyAsync(List<FuncPermsCreateModel> listModel);
        Task<Response> CreateAsync(FuncPermsCreateModel model);
        Task<Response> UpdateAsync(decimal id, FuncPermsUpdateModel model);
        Task<Response> DeleteAsync(decimal id);
        Task<Response> DeleteAllByFuncIdAsync(decimal funcId);
        Task<Response> DeleteManyAsync(FuncPermsDeleteModel model);
        Task<Response> DeleteManyByListFuncPermsAsync(decimal funcId, string strListFuncPerms);
        Task<Response> GetFunctionPermissionByFilterAsync(FuncPermsQueryModel model);
        Task<Response> GetAllFuncPermsActiveAsync();
        Task<Response> FindAsync(decimal id);
        Task<Response> GetTreeFuncPermsByAppIdGroupByFuncAsync(decimal appId, decimal? roleId = null);
        Task<Response> GetTreeFuncPermsByAppIdGroupByPermAsync(decimal appId, decimal? roleId = null);
        Task<Response> GetFuncPermsByRoleId(decimal? roleId);
        Task<Response> GetFuncPermsByFuncId(decimal funcId);
        Task<Response> GetFuncPermsByFuncIdRoleId(decimal roleId,decimal funcId);
        Task<Response> GetFuncPermsByUserId(decimal userId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public interface IRolesHandler
    {
        Task<Response> CreateAsync(RolesCreateModel model, BaseModel baseModel);
        Task<Response> UpdateAsync(decimal id, RolesUpdateModel model);
        Task<Response> GetAllAsync();
        Task<Response> GetRoleByFuncPermAsync(string listFuncPerms);
        Task<Response> GetRoleById(decimal id);
        Task<Response> DeleteRoleMany(List<decimal> listRoleId);
        Task<Response> DeleteAsync(decimal id);
        Task<Response> FindAsync(decimal id);
        Task<Response> GetRolesByFilterAsync(RolesQueryModel filterModel);
        Task<Response> GetRolesByAppId(decimal appId);
    }
}

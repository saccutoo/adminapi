using System;
using System.Threading.Tasks;
using Utils;
namespace API.BussinessLogic
{
    public interface IPermissionsHandler
    {
        Task<Response> GetAllPermsAsync(decimal? funcId = null);
        Task<Response> GetPermsByAppIdAsync(decimal appId);
        Task<Response> GetPermsByFuncAsync(decimal? funcId);
        Task<Response> GetPermsByRoleAsync(decimal? roleId);
        Task<Response> CreateAsync(PermissionsCreateModel model, BaseModel baseModel);
    }
}

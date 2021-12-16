using System;
using System.Threading.Tasks;
using Utils;
namespace API.BussinessLogic
{
    public interface IFunctionsHandler
    {
        Task<Response> CreateAsync(FunctionsCreateModel model, BaseModel baseModel);
        Task<Response> UpdateAsync(decimal id,FunctionsUpdateModel model);
        Task<Response> FindAsync(decimal id);
        Task<Response> DeleteAsync(decimal id);
        Task<Response> GetAllAsync();
        Task<Response> GetTreeFunctionsByUserAsync(decimal userId, decimal appId, string channelCode);
        Task<Response> GetTreeFunctionsWithoutAppAsync(decimal userId, string channelCode);
        Task<Response> GetAllFunctionByAppIdAsync(decimal appId, string type = null);
        Task<Response> GetAllFunctionByAppIdAsync_1(decimal appId);
        Task<Response> GetFunctionByRoleId(decimal roleId);
        Task<Response> GetFunctionsByFilterAsync(FunctionsQueryModel filterModel);
    }
}

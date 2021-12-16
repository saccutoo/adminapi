using System;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public interface IApplicationHandler
    {
        Task<Response> FindAsync(decimal id);
        Task<Response> DeleteAsync(decimal id);
        Task<Response> GetAllAsync();
        Task<Response> GetApplicationActive();
        Task<Response> CreateAsync(ApplicationCreateModel model, BaseModel baseModel);
        Task<Response> UpdateAsync(decimal id, ApplicationCreateModel model);
        Task<Response> GetApplicationByUserAsync(decimal userId, string channelCode);
        Task<Response> GetApplicationByRolesIdAsync(decimal roleId);
        Task<Response> GetApplicationByFilterAsync(ApplicationQueryModel filterModel);
    }
}

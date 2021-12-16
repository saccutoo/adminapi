using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public interface IUserGroupHandler
    {
        Task<Response> CreateAsync(UserGroupCreateModel model);
        Task<Response> UpdateAsync(decimal id, UserGroupUpdateModel model);
        Task<Response> DeleteByIdAsync(decimal id);
        Task<Response> GetByFilterAsync(UserGroupQueryModel queryModel);        
        Task<Response> GetById(decimal id); 
        Task<Response> GetAllByUser(decimal? userId = null);

    }
}

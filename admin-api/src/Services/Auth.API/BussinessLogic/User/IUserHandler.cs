using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public interface IUserHandler
    {
        Task<Response> FindAsync(decimal id);
        Task<Response> GetByUserName(string userName);
        Task<Response> UpdateLoginAsync(decimal id, UserLoginModel model);

    }
}

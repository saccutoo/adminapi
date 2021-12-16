using System;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public interface IAuthHandler
    {      
        Task<Response> Authenticate(string userName, string passWord, bool? isGetToken = null);
    }
}

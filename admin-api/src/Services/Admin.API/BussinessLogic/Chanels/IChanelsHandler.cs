using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public interface IChanelsHandler
    {
        Task<Response> GetAllChanels(decimal? funcId = null);
        Task<Response> GetChanelsByFuncId(decimal? funcId);
    }
}

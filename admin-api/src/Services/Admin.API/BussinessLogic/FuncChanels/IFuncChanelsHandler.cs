using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public interface IFuncChanelsHandler
    {
        Task<Response> CreateAsync(FuncChanelsCreateModel model);
        Task<Response> DeleteByFuncIdAsync(decimal funcId);
    }
}
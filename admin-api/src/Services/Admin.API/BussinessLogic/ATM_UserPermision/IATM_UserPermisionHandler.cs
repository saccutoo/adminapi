using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public interface IATM_UserPermisionHandler
    {
        Task<Response> GetATMUserPermsByFillter(ATM_UserPermisionQueryModel model);
        Task<Response> GetUserTTDVCNTT();
        Task<Response> GetChiNhanh();
        Task<Response> GetChiNhanhByUserName(string UserName);
        Task<Response> GetListPosCDById(int idUser);
        Task<Response> GrandATMUserPerms(string listPosCD, int idUser);
        Task<Response> GetInfoFromOOS(string Data);
    }
}
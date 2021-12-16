using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.BussinessLogic;
using Utils;
using API.Filters;
using Newtonsoft.Json;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Admin.API.Controllers
{
    [Route("api/admin/atmuserpermision")]
    public class ATM_UserPermisionController : ControllerBase
    {
        private readonly IATM_UserPermisionHandler _callServiceDB;
        public ATM_UserPermisionController(IATM_UserPermisionHandler employeeInterfaceHandler)
        {
            _callServiceDB = employeeInterfaceHandler;
        }

        #region GET
        /// <summary>
        /// Lấy thông tin ứng dụng theo thong tin nguoi dung
        /// </summary>
        /// <param name="model">model dung de filter</param>
        /// <returns></returns>
        [HttpGet]
        [Route("filter")]
        [ProducesResponseType(typeof(ResponseObject<ATM_UserPermisionBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserByFilterAsync(string filterModel)
        {
            ATM_UserPermisionQueryModel model = JsonConvert.DeserializeObject<ATM_UserPermisionQueryModel>(filterModel);
            var result = await _callServiceDB.GetATMUserPermsByFillter(model);
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("user-ttdvcntt")]
        [ProducesResponseType(typeof(ResponseObject<ATM_UserPermisionBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserTTDVCNTTAsync()
        {
            var result = await _callServiceDB.GetUserTTDVCNTT();
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("get-chi-nhanh")]
        [ProducesResponseType(typeof(ResponseObject<ATM_UserPermisionBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChiNhanhAsync()
        {
            var result = await _callServiceDB.GetChiNhanh();
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("get-chi-nhanh-by-user")]
        [ProducesResponseType(typeof(ResponseObject<ATM_UserPermisionBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChiNhanhByUserAsync()
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _callServiceDB.GetChiNhanhByUserName(requestInfo.UserName);
            return RequestHelpers.TransformData(result);
        }

        [HttpPost]
        [Route("grand-atm-user-perms")]
        [ProducesResponseType(typeof(ResponseObject<ATM_UserPermisionBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GrandATMUserPermsAsync(string listPosCD,int idUser)
        {
            var result = await _callServiceDB.GrandATMUserPerms(listPosCD,idUser);
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("get-list-poscd-by-id/{idUser}")]
        [ProducesResponseType(typeof(ResponseObject<ATM_UserPermisionBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListPosCDByIdAsync(int idUser)
        {
            var result = await _callServiceDB.GetListPosCDById(idUser);
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("get-info-from-oos")]
        [ProducesResponseType(typeof(ResponseObject<ATM_UserPermisionBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListPosCDByIdAsync(string query)
        {
            var result = await _callServiceDB.GetInfoFromOOS(query);
            return RequestHelpers.TransformData(result);
        }

        #endregion

    }
}

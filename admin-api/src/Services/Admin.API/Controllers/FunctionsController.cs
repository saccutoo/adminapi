using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.BussinessLogic;
using Utils;
using System;
using Newtonsoft.Json;
using API.Filters;

namespace API.Controllers
{
    [Route("api/admin/functions")]
    [ApiController]
    public class FunctionsController : ControllerBase
    {
        private readonly IFunctionsHandler _functionsInterfaceHandler;
        public FunctionsController(IFunctionsHandler employeeInterfaceHandler)
        {
            _functionsInterfaceHandler = employeeInterfaceHandler;
        }

        #region GET
        /// <summary>
        /// Lấy danh sách menu các chức năng theo ứng dụng cua nguoi dung dang nhap
        /// </summary>
        /// <param name="userId">User ID dang nhap</param>
        /// <param name="appId">Ma ung dung</param>
        /// <param name="channelCode">Ma kenh</param>
        /// <returns></returns>
        [HttpGet]
        [Route("user")]
        [ProducesResponseType(typeof(ResponseObject<FunctionsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTreeFunctionsByUserAsync(decimal userId, decimal appId, string channelCode)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _functionsInterfaceHandler.GetTreeFunctionsByUserAsync(userId, appId, channelCode);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách menu các chức năng cua nguoi dung dang nhap
        /// </summary>
        /// <param name="userId">User ID dang nhap</param>
        /// <param name="channelCode">Ma kenh</param>
        /// <returns></returns>
        [HttpGet]
        [Route("user-without-app")]
        [ProducesResponseType(typeof(ResponseObject<FunctionsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTreeFunctionsWithoutAppAsync(decimal userId, string channelCode)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _functionsInterfaceHandler.GetTreeFunctionsWithoutAppAsync(userId, channelCode);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách functions theo app code
        /// </summary>
        /// <param name="appId">Ma ung dung</param>
        /// <returns></returns>
        [HttpGet]
        [Route("application")]
        [ProducesResponseType(typeof(ResponseObject<FunctionsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllFunctionByAppIdAsync(decimal appId, string type = null)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _functionsInterfaceHandler.GetAllFunctionByAppIdAsync(appId, type);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách functions theo filter
        /// </summary>
        /// <param name="appId">Ma ung dung</param>
        /// <returns></returns>
        [HttpGet]
        [Route("filter")]
        [Permission(TypeFilter.CheckPermission, "VIEW_CHUC_NANG,CREATE_CHUC_NANG,UPDATE_CHUC_NANG,DELETE_CHUC_NANG")]
        [ProducesResponseType(typeof(ResponseObject<FunctionsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllFunctionByFilterAsync(string filterModel)
        {
            FunctionsQueryModel roleQuery = JsonConvert.DeserializeObject<FunctionsQueryModel>(filterModel);
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _functionsInterfaceHandler.GetFunctionsByFilterAsync(roleQuery);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Lấy function by Id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<FunctionsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFunctionByIdAsync(decimal id)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _functionsInterfaceHandler.FindAsync(id);
            return RequestHelpers.TransformData(result);
        }
        #endregion

        #region CRUD
        /// <summary>
        /// Thêm mới function
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Permission(TypeFilter.CheckPermission, "CREATE_CHUC_NANG")]
        [ProducesResponseType(typeof(ResponseObject<FunctionsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] FunctionsCreateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new BaseModel
            {
                Maker = requestInfo.UserName,
                MakerOnDate = DateTime.Now
            };
            var result = await _functionsInterfaceHandler.CreateAsync(model, baseModel);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// chỉnh sửa function
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Permission(TypeFilter.CheckPermission, "UPDATE_CHUC_NANG")]
        [ProducesResponseType(typeof(ResponseObject<FunctionsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync( decimal id,[FromBody] FunctionsUpdateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _functionsInterfaceHandler.UpdateAsync(id,model);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// chỉnh sửa function
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        [Permission(TypeFilter.CheckPermission, "DELETE_CHUC_NANG")]
        [ProducesResponseType(typeof(ResponseObject<FunctionsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync(decimal id)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _functionsInterfaceHandler.DeleteAsync(id);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}
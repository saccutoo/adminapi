using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.BussinessLogic;
using Utils;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Admin.API.Controllers
{
    [Route("api/admin/funcperms")]
    [ApiController]
    public class FuncPermsController : ControllerBase
    {
        private readonly IFuncPermsHandler _funcpermsInterfaceHandler;
        public FuncPermsController(IFuncPermsHandler funcpermsInterfaceHandler)
        {
            _funcpermsInterfaceHandler = funcpermsInterfaceHandler;
        }

        #region GET
        /// <summary>
        /// Lấy 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseObject<FuncPermsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAsync()
        {            
            var result = await _funcpermsInterfaceHandler.GetAllFuncPermsActiveAsync();
            return RequestHelpers.TransformData(result);
        }
        /// <summary>
        /// Lấy thông tin theo thong tin nguoi dung
        /// </summary>
        /// <param name="model">model dung de filter</param>
        /// <returns></returns>
        [HttpGet]
        [Route("filter")]
        [ProducesResponseType(typeof(ResponseObject<FuncPermsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByFilterAsync(string filterModel)
        {
            //conver string json to model
            FuncPermsQueryModel model = JsonConvert.DeserializeObject<FuncPermsQueryModel>(filterModel);
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _funcpermsInterfaceHandler.GetFunctionPermissionByFilterAsync(model);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Lấy tree group by Func
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("funcs")]
        [ProducesResponseType(typeof(ResponseObject<FuncPermsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTreeFuncPermsByAppIdGroupByFuncAsync(decimal appId, decimal? roleId = null)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _funcpermsInterfaceHandler.GetTreeFuncPermsByAppIdGroupByFuncAsync(appId, roleId);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Lấy tree group by Perm
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("perms")]
        [ProducesResponseType(typeof(ResponseObject<FuncPermsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTreeFuncPermsByAppIdGroupByPermAsync(decimal appId, decimal? roleId = null)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _funcpermsInterfaceHandler.GetTreeFuncPermsByAppIdGroupByPermAsync(appId, roleId);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("func/{id}")]
        [ProducesResponseType(typeof(ResponseObject<FuncPermsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFuncPermsByFuncIdAsync(decimal id)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _funcpermsInterfaceHandler.GetFuncPermsByFuncId(id);
            return RequestHelpers.TransformData(result);
        }
        #endregion

        #region CRUD
        /// <summary>
        /// Thêm mới func permission
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseObject<FuncPermsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] List<FuncPermsCreateModel> listModel)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _funcpermsInterfaceHandler.CreateManyAsync(listModel);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Cập nhật func permission
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(ResponseObject<FuncPermsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(decimal id, [FromBody] FuncPermsUpdateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _funcpermsInterfaceHandler.UpdateAsync(id, model);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Delete func permission
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<FuncPermsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUserAsync(decimal id)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _funcpermsInterfaceHandler.DeleteAsync(id);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Delete func permission many
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deletemany")]
        [ProducesResponseType(typeof(ResponseObject<FuncPermsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUserManyAsync(FuncPermsDeleteModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _funcpermsInterfaceHandler.DeleteManyAsync(model);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.BussinessLogic;
using Utils;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using API.Filters;

namespace API.Controllers
{
    [Route("api/admin/roles")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRolesHandler _rolesInterfaceHandler;
        public RolesController(IRolesHandler rolesInterfaceHandler)
        {
            _rolesInterfaceHandler = rolesInterfaceHandler;
        }

        #region GET
        /// <summary>
        /// Lấy tất cả roles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseObject<ApplicationBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAsync()
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _rolesInterfaceHandler.GetAllAsync();
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Lấy role theo filter
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("filter")]
        [Permission(TypeFilter.CheckPermission, "VIEW_NHOM_QUYEN,CREATE_NHOM_QUYEN,UPDATE_NHOM_QUYEN,DELETE_NHOM_QUYEN")]
        [ProducesResponseType(typeof(ResponseObject<RolesBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRolesByFilterAsync(string filterModel)
        {
            RolesQueryModel roleQuery = JsonConvert.DeserializeObject<RolesQueryModel>(filterModel);
            var result = await _rolesInterfaceHandler.GetRolesByFilterAsync(roleQuery);
            return RequestHelpers.TransformData(result);
        }
        
        /// <summary>
        /// Lấy role theo roleid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<RolesBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRoleById(decimal id)
        {
        
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _rolesInterfaceHandler.GetRoleById(id);
            return RequestHelpers.TransformData(result);

        }
        #endregion

        #region CRUD
        /// <summary>
        /// Tạo mới roles
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Permission(TypeFilter.CheckPermission, "CREATE_NHOM_QUYEN")]
        [ProducesResponseType(typeof(ResponseObject<RolesBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] RolesCreateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new BaseModel
            {
                Maker = requestInfo.UserName,
                MakerOnDate = DateTime.Now
            };
            var result = await _rolesInterfaceHandler.CreateAsync(model, baseModel);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Update roles
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Permission(TypeFilter.CheckPermission, "UPDATE_NHOM_QUYEN")]
        [ProducesResponseType(typeof(ResponseObject<RolesBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(decimal id,[FromBody] RolesUpdateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _rolesInterfaceHandler.UpdateAsync(id, model);
            return RequestHelpers.TransformData(result);
        }


        /// <summary>
        /// Delete roles
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [Permission(TypeFilter.CheckPermission, "DELETE_NHOM_QUYEN")]
        [ProducesResponseType(typeof(ResponseObject<RolesBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync(decimal id)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _rolesInterfaceHandler.DeleteAsync(id);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Delete many roles
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("deletemany")]
        [Permission(TypeFilter.CheckPermission, "DELETE_NHOM_QUYEN")]
        [ProducesResponseType(typeof(ResponseObject<RolesBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteManyAsync(List<decimal> listRoleId)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _rolesInterfaceHandler.DeleteRoleMany(listRoleId);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}
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
    [Route("api/admin/user-group")]
    [ApiController]
    public class UserGroupController : ControllerBase
    {
        private readonly IUserGroupHandler _iUserGroupHandler;
        public UserGroupController(IUserGroupHandler iUserGroupHandler)
        {
            _iUserGroupHandler = iUserGroupHandler;
        }

        #region GET
        /// <summary>
        /// Lấy nhóm người dùng theo bộ lọc
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("filter")]
        //[Permission(TypeFilter.CheckPermission, "VIEW_NHOM_QUYEN,CREATE_NHOM_QUYEN,UPDATE_NHOM_QUYEN,DELETE_NHOM_QUYEN")]
        [ProducesResponseType(typeof(ResponseObject<UserGroupModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByFilterAsync(string queryModel)
        {
            UserGroupQueryModel roleQuery = JsonConvert.DeserializeObject<UserGroupQueryModel>(queryModel);
            var result = await _iUserGroupHandler.GetByFilterAsync(roleQuery);
            return RequestHelpers.TransformData(result);
        }
        
        /// <summary>
        /// Lấy chi tiết theo id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<UserGroupModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(decimal id)
        {
            var result = await _iUserGroupHandler.GetById(id);
            return RequestHelpers.TransformData(result);

        }

        /// <summary>
        /// Lấy danh sách nhóm NND được phân quyền
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("by-user")]
        [ProducesResponseType(typeof(ResponseObject<UserGroupModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllByUser(decimal? userId = null)
        {
            var result = await _iUserGroupHandler.GetAllByUser(userId);
            return RequestHelpers.TransformData(result);

        }

        #endregion

        #region CRUD
        /// <summary>
        /// Tạo mới nhóm người dùng
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[Permission(TypeFilter.CheckPermission, "CREATE_NHOM_QUYEN")]
        [ProducesResponseType(typeof(ResponseObject<UserGroupModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] UserGroupCreateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null) model.CreatedBy = requestInfo.UserName;
            var result = await _iUserGroupHandler.CreateAsync(model);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Cập nhật nhóm người dùng
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        //[Permission(TypeFilter.CheckPermission, "UPDATE_NHOM_QUYEN")]
        [ProducesResponseType(typeof(ResponseObject<UserGroupModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(decimal id,[FromBody] UserGroupUpdateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null) model.LastModifiedBy = requestInfo.UserName;
            var result = await _iUserGroupHandler.UpdateAsync(id, model);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Xóa theo id
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<UserGroupModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteByIdAsync(decimal id)
        {
            var result = await _iUserGroupHandler.DeleteByIdAsync(id);
            return RequestHelpers.TransformData(result);

        }
        #endregion
    }
}
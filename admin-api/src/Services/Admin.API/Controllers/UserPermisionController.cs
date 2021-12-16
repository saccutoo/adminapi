using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.BussinessLogic;
using Utils;
using API.Filters;
using System;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Admin.API.Controllers
{
    [Route("api/admin/userpermision")]
    public class UserPermisionController : ControllerBase
    {
        private readonly IUserPermisionHandler _userPermisionInterfaceHandler;
        public UserPermisionController(IUserPermisionHandler employeeInterfaceHandler)
        {
            _userPermisionInterfaceHandler = employeeInterfaceHandler;
        }

        #region GET
        /// <summary>
        /// Lấy thông tin ứng dụng theo thong tin nguoi dung
        /// </summary>
        /// <param name="model">model dung de filter</param>
        /// <returns></returns>
        [HttpGet]
        [Route("funcs")]
        [ProducesResponseType(typeof(ResponseObject<UserPermisionBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTreeFuncPermByUserIdGroupByFunc(decimal? userId)
        {
            var result = await _userPermisionInterfaceHandler.GetTreeFuncPermByUserIdGroupByFunc(userId);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Lấy thông tin ứng dụng theo thong tin nguoi dung
        /// </summary>
        /// <param name="model">model dung de filter</param>
        /// <returns></returns>
        [HttpGet]
        [Route("perms")]
        [ProducesResponseType(typeof(ResponseObject<UserPermisionBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTreeFuncPermByUserIdGroupByPerms(decimal? userId)
        {
            var result = await _userPermisionInterfaceHandler.GetTreeFuncPermByUserIdGroupByPerms(userId);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Lấy cây ứng dụng - nhóm quyền không theo người dùng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("tree-app-role")]
        [ProducesResponseType(typeof(ResponseObject<UserGroupModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTreeAllAppThenRole(decimal? userGroupId = null)
        {
            var result = await _userPermisionInterfaceHandler.GetTreeAllAppThenRole(userGroupId);
            return RequestHelpers.TransformData(result);
        }
        #endregion

        #region CRUD
        /// <summary>
        /// Cập nhật user permission
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Permission(TypeFilter.CheckPermission, "UPDATE_NGUOI_DUNG")]
        [ProducesResponseType(typeof(ResponseObject<UserPermisionBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(decimal userId, [FromBody] UserPermisionUpdateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null) model.Maker = requestInfo.UserName;
            var result = await _userPermisionInterfaceHandler.UpdateAsync(userId, model);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.BussinessLogic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Utils;

namespace Admin.API.Controllers
{
    [Route("api/admin/userroles")]
    [ApiController]
    public class UserRolesController : ControllerBase
    {
        private readonly IUserRolesHandler _userRolesInterfaceHandler;
        public UserRolesController(IUserRolesHandler userRolesInterfaceHandler)
        {
            _userRolesInterfaceHandler = userRolesInterfaceHandler;
        }

        #region GET
        /// <summary>
        /// Lấy tất cả roles by user id
        /// </summary>
        /// <param name="userId">Ma user</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{userId}/roles")]
        [ProducesResponseType(typeof(ResponseObject<UserRolesBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRolesOfUserAsync(decimal userId)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _userRolesInterfaceHandler.GetRolesOfUserAsync(userId);
            return RequestHelpers.TransformData(result);
        }
        /// <summary>
        /// Lấy tất cả roles by user id
        /// </summary>
        /// <param name="userId">Ma user</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{userId}/rolesnotexits")]
        [ProducesResponseType(typeof(ResponseObject<UserRolesBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRolesNotOfUserAsync(decimal userId)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _userRolesInterfaceHandler.GetRolesNotOfUserAsync(userId);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}
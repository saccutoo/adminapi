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
    [Route("api/admin/permissions")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionsHandler _permissionsInterfaceHandler;
        public PermissionsController(IPermissionsHandler employeeInterfaceHandler)
        {
            _permissionsInterfaceHandler = employeeInterfaceHandler;
        }

        #region GET
        /// <summary>
        /// Lấy tất cả permissions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseObject<PermissionsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAsync(decimal? funcId = null)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _permissionsInterfaceHandler.GetAllPermsAsync(funcId);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}
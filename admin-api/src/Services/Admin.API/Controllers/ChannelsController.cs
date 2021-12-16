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
    [Route("api/admin/channels")]
    [ApiController]
    public class ChannelsController : ControllerBase
    {
        private readonly IChanelsHandler _chanelsInterfaceHandler;
        public ChannelsController(IChanelsHandler chanesInterfaceHandler)
        {
            _chanelsInterfaceHandler = chanesInterfaceHandler;
        }

        #region GET
        /// <summary>
        /// Lấy chanels
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseObject<ChanelsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAsync(decimal? funcId = null)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _chanelsInterfaceHandler.GetAllChanels(funcId);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}
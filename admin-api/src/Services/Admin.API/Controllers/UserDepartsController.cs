using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.BussinessLogic;
using Utils;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Admin.API.Controllers
{
    [Route("api/admin/userdeparts")]
    public class UserDepartsController : ControllerBase
    {
        private readonly IUserDepartsHandler _userDepartsInterfaceHandler;
        public UserDepartsController(IUserDepartsHandler userDepartsInterfaceHandler)
        {
            _userDepartsInterfaceHandler = userDepartsInterfaceHandler;
        }
        /// <summary>
        /// Cập nhật ứng dụng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(ResponseObject<UserDepartsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(decimal id, [FromBody] UserDepartsUpdateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _userDepartsInterfaceHandler.UpdateUserInDepartment(id, model);
            return RequestHelpers.TransformData(result);
        }
    }
}

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.BussinessLogic;
using Utils;

namespace API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthHandler _authInterfaceHandler;
        public AuthController(IAuthHandler authHandler)
        {
            _authInterfaceHandler = authHandler;
        }

        #region GET
        /// <summary>
        /// Đăng nhập AD trả về Token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(typeof(ResponseObject<UserLoginModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> LoginAD([FromBody] LoginModel model)
        {
            var result = await _authInterfaceHandler.Authenticate(model.UserName, model.PassWord, true);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Đăng nhập AD không trả về Token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("loginwithouttoken")]
        [ProducesResponseType(typeof(ResponseObject<UserLoginModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> LoginADWithoutToken([FromBody] LoginModel model)
        {
            var result = await _authInterfaceHandler.Authenticate(model.UserName, model.PassWord, false);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.BussinessLogic;
using Utils;
using Newtonsoft.Json;

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
            //_authInterfaceHandler.Init();
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
            var result = await _authInterfaceHandler.Authenticate(model.UserName, model.PassWord, model.Zone);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Đăng nhập sinh trắc học
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login-biometric")]
        [ProducesResponseType(typeof(ResponseObject<UserLoginModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> LoginBiometric([FromBody] UserBiometricBaseModel model)
        {
            var result = await _authInterfaceHandler.AuthenticateBiometric(model.UserName, model.DeviceId, model.BiometricToken);
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
            var result = await _authInterfaceHandler.Authenticate(model.UserName, model.PassWord, model.Zone, false);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Kiểm tra key manager
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("check-key-manager")]
        [ProducesResponseType(typeof(ResponseObject<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckKeyManager(string userName, string idPermissionToken, string bearerToken)
        {
            var result = await _authInterfaceHandler.CheckKeyManager(userName, idPermissionToken, bearerToken);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Thu hồi key khi đăng xuất
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("revoke-key")]
        [ProducesResponseType(typeof(ResponseObject<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RevokeKey()
        {
            var idPermissionToken = string.Empty;
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (!string.IsNullOrEmpty(requestInfo.PermissionToken))
            {
                // Giai ma token theo khoa 
                var permissionTokenDecrypt = EncryptedString.DecryptString(requestInfo.PermissionToken, Helpers.GetConfig("Encrypt:Key"));
                // Chuyen doi thanh model
                PermissionTokenModel permissionTokenModel = JsonConvert.DeserializeObject<PermissionTokenModel>(permissionTokenDecrypt);

                idPermissionToken = permissionTokenModel.Id.ToString();
            }
            
            var result = await _authInterfaceHandler.RevokeKey(idPermissionToken);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.BussinessLogic;
using Utils;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using API.Filters;

namespace Admin.API.Controllers
{
    [Route("api/admin/biometric")]
    [ApiController]
    public class UserBiometricController : ControllerBase
    {
        private readonly IUserBiometricHandler _userBiometricHandler;
        public UserBiometricController(IUserBiometricHandler userBiometricHandler)
        {
            _userBiometricHandler = userBiometricHandler;
        }
      
        #region CRUD
        /// <summary>
        /// Đăng ký biometric
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("register")]
        [HttpPost]
        [ProducesResponseType(typeof(ResponseObject<UserBiometricModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RegisterBiometric([FromBody] UserBiometricCreateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null) model.CreatedBy = requestInfo.UserName;
            model.Status = "REGISTER";
            var result = await _userBiometricHandler.SetBiometric(model);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Đăng ký biometric
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("unregister")]
        [HttpPost]
        [ProducesResponseType(typeof(ResponseObject<UserBiometricModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UnRegisterBiometric([FromBody] UserBiometricCreateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null) model.CreatedBy = requestInfo.UserName;
            model.Status = "UNREGISTER";
            var result = await _userBiometricHandler.SetBiometric(model);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}
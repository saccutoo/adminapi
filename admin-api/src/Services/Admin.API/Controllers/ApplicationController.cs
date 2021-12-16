using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.BussinessLogic;
using Utils;
using System;
using Newtonsoft.Json;
using API.Filters;

namespace API.Controllers
{
    [Route("api/admin/applications")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationHandler _applicationInterfaceHandler;
        public ApplicationController(IApplicationHandler employeeInterfaceHandler)
        {
            _applicationInterfaceHandler = employeeInterfaceHandler;
        }

        #region GET
        /// <summary>
        /// Lấy thông tin ứng dụng theo id
        /// </summary>
        /// <param name="id">Id nhân viên</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<ApplicationBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByIdAsync(decimal id)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _applicationInterfaceHandler.FindAsync(id);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Lấy tất cả ứng dụng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Permission(TypeFilter.CheckPermission,"ALL")]
        [ProducesResponseType(typeof(ResponseObject<ApplicationBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAsync()
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _applicationInterfaceHandler.GetAllAsync();
            return RequestHelpers.TransformData(result);
        }


        /// <summary>
        /// Lấy tất cả ứng dụng được phân quyền cho người dùng đăng nhập
        /// </summary>
        /// <param name="userName">Ten dang nhap</param>
        /// <param name="channelCode">Ma kenh</param>
        /// <returns></returns>
        [HttpGet]
        [Route("user")]
        [ProducesResponseType(typeof(ResponseObject<ApplicationBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetApplicationByUserAsync(decimal userId, string channelCode)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _applicationInterfaceHandler.GetApplicationByUserAsync(userId, channelCode);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Lấy tất cả ứng dụng có trạng thái active
        /// </summary>
        /// <param name="userName">Ten dang nhap</param>
        /// <param name="channelCode">Ma kenh</param>
        /// <returns></returns>
        [HttpGet]
        [Route("active")]
        [ProducesResponseType(typeof(ResponseObject<ApplicationBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetApplicationActive()
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _applicationInterfaceHandler.GetApplicationActive();
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Lấy application theo filter
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("filter")]
        [Permission(TypeFilter.CheckPermission, "VIEW_UNG_DUNG,CREATE_UNG_DUNG,UPDATE_UNG_DUNG,DELETE_UNG_DUNG")]
        [ProducesResponseType(typeof(ResponseObject<ApplicationBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetApplicationsByFilterAsync(string filterModel)
        {
            ApplicationQueryModel appQuery = JsonConvert.DeserializeObject<ApplicationQueryModel>(filterModel);
            var result = await _applicationInterfaceHandler.GetApplicationByFilterAsync(appQuery);
            return RequestHelpers.TransformData(result);
        }
        #endregion

        #region CRUD
        /// <summary>
        /// Thêm mới ứng dụng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Permission(TypeFilter.CheckPermission, "CREATE_UNG_DUNG")]
        [ProducesResponseType(typeof(ResponseObject<ApplicationBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] ApplicationCreateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new BaseModel
            {
                Maker = requestInfo.UserName,
                MakerOnDate = DateTime.Now
            };
            var result = await _applicationInterfaceHandler.CreateAsync(model, baseModel);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Cập nhật ứng dụng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        [Permission(TypeFilter.CheckPermission, "UPDATE_UNG_DUNG")]
        [ProducesResponseType(typeof(ResponseObject<ApplicationBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(decimal id, [FromBody] ApplicationCreateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _applicationInterfaceHandler.UpdateAsync(id, model);
            return RequestHelpers.TransformData(result);
        }
        /// <summary>
        /// Xóa ứng dụng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [Permission(TypeFilter.CheckPermission, "DELETE_UNG_DUNG")]
        [ProducesResponseType(typeof(ResponseObject<ApplicationBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync(decimal id)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _applicationInterfaceHandler.DeleteAsync(id);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}

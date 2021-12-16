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
    [Route("api/admin/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserHandler _userInterfaceHandler;
        public UserController(IUserHandler employeeInterfaceHandler)
        {
            _userInterfaceHandler = employeeInterfaceHandler;
        }

        #region GET
        /// <summary>
        /// Lấy danh sách các nhân sự có ngày sinh trong tháng
        /// </summary>
        /// <param name="month">tháng hiện tại</param>
        /// <returns></returns>
        [HttpGet]
        [Route("birthdaybymonth")]
        [ProducesResponseType(typeof(ResponseObject<UserBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserByBirthdayMonnthAsync(int month)
        {
            var result = await _userInterfaceHandler.GetUserByBirthdayMonnthAsync(month);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách các nhân sự có ngày sinh trong tháng và ngày hiện tại
        /// </summary>
        /// <param name="day">ngày hiện tại</param>
        /// <param name="month">tháng hiện tại</param>
        /// <returns></returns>
        [HttpGet]
        [Route("birthdaybyday")]
        [ProducesResponseType(typeof(ResponseObject<UserBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserByBirthdayDayAsync(int day, int month)
        {
            var result = await _userInterfaceHandler.GetUserByBirthdayDayAsync(day,month);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Lấy thông tin ứng dụng theo thong tin nguoi dung
        /// </summary>
        /// <param name="model">model dung de filter</param>
        /// <returns></returns>
        [HttpGet]
        [Route("filter")]
        [Permission(TypeFilter.CheckPermission, "VIEW_NGUOI_DUNG,CREATE_NGUOI_DUNG,UPDATE_NGUOI_DUNG,DELETE_NGUOI_DUNG")]
        [ProducesResponseType(typeof(ResponseObject<UserBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserByFilterAsync(string filterModel)
        {
            UserQueryModel model = JsonConvert.DeserializeObject<UserQueryModel>(filterModel);
            var result = await _userInterfaceHandler.GetUserByFilterAsync(model);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Lấy toàn bộ thông tin user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-staff-by-manager")]
        [ProducesResponseType(typeof(ResponseObject<UserBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStaffByManagerAsync()
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);           
            var result = await _userInterfaceHandler.GetStaffByManagerAsync(requestInfo.UserName);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Lấy toàn bộ danh sách nhân viên theo quản lý
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseObject<UserBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserAllAsync()
        {
            var result = await _userInterfaceHandler.GetAllAsync();
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Lấy thông tin ứng dụng theo thong tin nguoi dung
        /// </summary>
        /// <param name="model">model dung de filter</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<UserBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserByIdAsync(decimal id)
        {
            var result = await _userInterfaceHandler.FindAsync(id);
            return RequestHelpers.TransformData(result);
        }


        /// <summary>
        /// Lấy thông tin SAUser theo mã nhân viên OS
        /// </summary>
        /// <param name="Emp_ID">model dung de filter</param>
        /// <returns></returns>
        [HttpGet]
        [Route("byemp_id")]
        [ProducesResponseType(typeof(ResponseObject<UserBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByEmpIDAsync(string Emp_ID)
        {
            var result = await _userInterfaceHandler.GetByEmpIDAsync(Emp_ID);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Lấy thông tin SAUser theo email
        /// </summary>
        /// <param name="Email">model dung de filter</param>
        /// <returns></returns>
        [HttpGet]
        [Route("byemail")]
        [ProducesResponseType(typeof(ResponseObject<UserBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByEmailAsync(string Email)
        {
            var result = await _userInterfaceHandler.GetByEmailAsync(Email);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Lấy toàn bộ danh sách phòng ban Ias
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-all-phong-ban-ias")]
        [ProducesResponseType(typeof(ResponseObject<UserBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPhongBanIas()
        {
            var result = await _userInterfaceHandler.GetAllPhongBanIas();
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Lấy toàn bộ danh sách pos theo pham vi
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-all-pos-pham-vi")]
        [ProducesResponseType(typeof(ResponseObject<UserBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPosTheoPhamVi(string maPhamVi)
        {
            var result = await _userInterfaceHandler.GetPosTheoPhamVi(maPhamVi);
            return RequestHelpers.TransformData(result);
        }
        #endregion

        #region CRUD
        /// <summary>
        /// Thêm mới user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseObject<UserBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] UserCreateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new BaseModel
            {
                Maker = requestInfo.UserName,
                MakerOnDate = DateTime.Now
            };
            var result = await _userInterfaceHandler.CreateAsync(model, baseModel);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Cập nhật user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Permission(TypeFilter.CheckPermission, "UPDATE_NGUOI_DUNG")]
        [ProducesResponseType(typeof(ResponseObject<UserBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(decimal id, [FromBody] UserUpdateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
             var result = await _userInterfaceHandler.UpdateAsync(id, model);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Cập nhật FCM token của user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}/update-fcmtoken")]
        [ProducesResponseType(typeof(ResponseObject<ResponseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateFcmTokenAsync(decimal id, [FromBody] UserUpdateFCMModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if(requestInfo.CurrentUserId != id)
            {
                var response = new ResponseError(Utils.StatusCode.Fail, "Bạn không có quyền đổi token của người khác!");
                return RequestHelpers.TransformData(response);
            }
            model.UserId = id;
            var result = await _userInterfaceHandler.UpdateFcmTokenAsync(model);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Cập nhật user theo nhóm người dùng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut,Route("update-user-group")]
        [Permission(TypeFilter.CheckPermission, "UPDATE_NGUOI_DUNG")]
        [ProducesResponseType(typeof(ResponseObject<UserBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserGroupAsync(decimal userId, [FromBody] UserUserGroupUpdateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null) model.ModifiedBy = requestInfo.UserName;
            var result = await _userInterfaceHandler.UpdateUserGroupAsync(userId, model);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<UserBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUserAsync(decimal id)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _userInterfaceHandler.DeleteAsync(id);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Delete user many
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deletemany")]
        [ProducesResponseType(typeof(ResponseObject<UserBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUserManyAsync(UserDeleteModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _userInterfaceHandler.DeleteManyAsync(model);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}
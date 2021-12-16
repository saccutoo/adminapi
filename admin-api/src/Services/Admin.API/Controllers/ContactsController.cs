using Microsoft.AspNetCore.Mvc;
using API.BussinessLogic;
using API.Filters;
using Utils;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using API.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace API.Controllers
{
    [Route("api/contacts")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactsHandler _contactInterfaceHandler;
        private readonly ILogger<ContactsHandler> _logger;
        private readonly string _rootPath;
        public ContactsController(IContactsHandler contactInterfaceHandler, ILogger<ContactsHandler> logger)
        {
            _rootPath = Helpers.GetConfig("StaticFiles:Folder");
            _logger = logger;
            _contactInterfaceHandler = contactInterfaceHandler;
        }

        /// <summary>
        /// Cập nhật danh bạ 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        //[Permission(TypeFilter.CheckPermission, "UPDATE_DANH_BA_NGUOI_DUNG")]
        [ProducesResponseType(typeof(ResponseObject<UserBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(decimal id, [FromForm] ContactUpdateModel model)
        {
            // Kiem tra tinh dung dan cua file anh
            //if (model.UploadFile != null && (!FormFileExtensions.IsImageValid(model.UploadFile)))
            //{
            //    return RequestHelpers.TransformData(new ResponseError(Utils.StatusCode.Fail, "Kích thước tập tin không được lớn hơn 2MB.Định dạng cho phép: jpg,png,jpeg,jpe"));
            //}

            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null) model.LastModifiedBy = requestInfo.UserName;
            var result = await _contactInterfaceHandler.UpdateAsync(id, model);
            return RequestHelpers.TransformData(result);
        }

        #region GET
        /// <summary>
        /// Lấy danh sách danh bạ theo bộ lọc
        /// </summary>
        /// <param name="model">model dung de filter</param>
        /// <returns></returns>
        [HttpGet]
        [Route("filter")]
        [Permission(TypeFilter.CheckPermission, "VIEW_DANH_BA_NGUOI_DUNG,UPDATE_DANH_BA_NGUOI_DUNG")]
        public async Task<IActionResult> GetContactByFilterAsync(string filterModel)
        {
            ContactQueryModel model = JsonConvert.DeserializeObject<ContactQueryModel>(filterModel);
            var result = await _contactInterfaceHandler.GetContactByFilterAsync(model);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách danh bạ theo bộ lọc và nhóm theo phòng ban
        /// </summary>
        /// <param name="model">model dung de filter</param>
        /// <returns></returns>
        [HttpGet]
        [Route("group-filter")]
        //[Permission(TypeFilter.CheckPermission, "VIEW_DANH_BA")]
        public async Task<IActionResult> GetContactGroupByFilterAsync(string filterModel)
        {
            ContactQueryModel model = JsonConvert.DeserializeObject<ContactQueryModel>(filterModel);
            var result = await _contactInterfaceHandler.GetContactGroupByFilterAsync(model);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Lấy thông tin chi tiết danh bạ theo id
        /// </summary>
        /// <param name="model">model dung de filter</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetContactByIdAsync(decimal id)
        {
            var result = await _contactInterfaceHandler.FindAsync(id);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Doc avatarr
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("view-image-as-raw")]
        public async Task<IActionResult> ViewRawImageAsync(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName)) fileName = "avatar-default.jpg";
                /* Lay du lieu byte cua avatar */
                // Lay duong dan tuyet doi cua file avatar
                string fullPath = string.Join('/', new string[] { _rootPath, "avatar", fileName });
                // Nap file vao metadata
                FileInfo fileInfo = new FileInfo(fullPath);
                if (!fileInfo.Exists) return NotFound();
                // Khoi tao mang luu gia tri 
                byte[] data = new byte[fileInfo.Length];
                // Nap noi dung file vao mang byte
                using (FileStream fs = fileInfo.OpenRead()) fs.Read(data, 0, data.Length);

                Stream memory = new MemoryStream(data);
                return new FileStreamResult(memory, "image/jpeg");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception error");
                return NotFound(); 
            }
        }
        #endregion       
    }
}

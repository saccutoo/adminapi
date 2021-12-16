using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Utils;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using API.BussinessLogic;

namespace API.Controllers
{
    [Route("api/admin/saareas")]
    [ApiController]
    public class SAAreasController : ControllerBase
    {
        private readonly ISAAreasHandler _SAAreasInterfaceHandler;
        public SAAreasController(ISAAreasHandler employeeInterfaceHandler)
        {
            _SAAreasInterfaceHandler = employeeInterfaceHandler;
        }

        #region GET

        /// <summary>
        /// Lấy toàn bộ email templates theo trạng thái
        /// </summary>
        /// <param name="Type">ALL/ACTIVE : lấy theo trạng thái</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseObject<SAAreasBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAsync(string Type)
        {
            var result = await _SAAreasInterfaceHandler.GetAllAsync(Type);
            return RequestHelpers.TransformData(result);
        }


        /// <summary>
        /// Lấy email templates theo mã
        /// </summary>
        /// <param name="id">Mã khảo sát</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<SAAreasBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByIdAsync(decimal id)
        {
            var result = await _SAAreasInterfaceHandler.GetById(id);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Lấy email templates theo mã
        /// </summary>
        /// <param name="Code">Mã</param>
        /// <returns></returns>
        [HttpGet]
        [Route("bycode")]
        [ProducesResponseType(typeof(ResponseObject<SAAreasBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCodeAsync(string Code)
        {
            var result = await _SAAreasInterfaceHandler.GetByCode(Code);
            return RequestHelpers.TransformData(result);
        }



        /// <summary>
        /// Lấy dữ liệu theo filter
        /// </summary>
        /// <param name="filterModel">model dung de filter</param>
        /// <returns></returns>
        [HttpGet]
        [Route("filter")]
        [ProducesResponseType(typeof(ResponseObject<SAAreasBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByFilterAsync(string filterModel)
        {
            SAAreasQueryModel model = JsonConvert.DeserializeObject<SAAreasQueryModel>(filterModel);
            var result = await _SAAreasInterfaceHandler.GetByFilterAsync(model);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// kiem tra ten
        /// </summary>
        /// <param name="id">mã</param>
        /// <param name="code">code</param>
        /// <returns></returns>
        [HttpGet]
        [Route("checkcode")]
        [ProducesResponseType(typeof(ResponseObject<SAAreasBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckNameAsync(decimal id, string code)
        {
            var result = await _SAAreasInterfaceHandler.CheckCodeAsync(id, code);
            return RequestHelpers.TransformData(result);
        }

        #endregion

        #region CRUD
        /// <summary>
        /// Thêm mới email templates
        /// </summary>
        /// <param name="model">Các thông tin thêm mới</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseObject<SAAreasBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] SAAreasCreateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new BaseModel
            {
                Maker = requestInfo.UserName,
                MakerOnDate = DateTime.Now
            };
            var result = await _SAAreasInterfaceHandler.CreateAsync(model);
            return RequestHelpers.TransformData(result);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ResponseObject<SAAreasBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(decimal id, [FromBody] SAAreasUpdateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new BaseModel
            {
                Maker = requestInfo.UserName,
                MakerOnDate = DateTime.Now
            };
            var result = await _SAAreasInterfaceHandler.UpdateAsync(id, model);
            return RequestHelpers.TransformData(result);
        }


        /// <summary>
        /// Xóa email templates
        /// </summary>
        /// <param name="id">mã email templates</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<SAAreasBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync(decimal id)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _SAAreasInterfaceHandler.DeleteAsync(id, requestInfo.UserName);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Xóa nhiều email templates
        /// </summary>
        /// <param name="listRoleId">mảng các mã khảo sát</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deletemany")]
        [ProducesResponseType(typeof(ResponseObject<SAAreasBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteManyAsync(List<decimal> listRoleId)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _SAAreasInterfaceHandler.DeleteManyAsync(listRoleId, requestInfo.UserName);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Duyệt email templates
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("approved")]
        [ProducesResponseType(typeof(ResponseObject<SAAreasBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ApprovedAsync(SAAreasApprovedModel model)
        {
            var result = await _SAAreasInterfaceHandler.ApprovedAsync(model);
            return RequestHelpers.TransformData(result);
        }

        #endregion
    }
}
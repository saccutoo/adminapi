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
    [Route("api/admin/departments")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentsHandler _departmenstInterfaceHandler;
        public DepartmentsController(IDepartmentsHandler departmentsInterfaceHandler)
        {
            _departmenstInterfaceHandler = departmentsInterfaceHandler;
        }

        #region CRUD
        /// <summary>
        /// Cap nhat so dien thoai phong ban
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("tel")]
        [ProducesResponseType(typeof(ResponseObject<DepartmentsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateTel(decimal id, string tel)
        {
            var result = await _departmenstInterfaceHandler.UpdateTel(id, tel);
            return RequestHelpers.TransformData(result);
        }
        #endregion

        #region GET
        /// <summary>
        /// Lay cay don vi dang cay phan cap
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("tree-view")]
        [ProducesResponseType(typeof(ResponseObject<List<TreeDepartmentModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDepartmentTreeView()
        {
            var result = await _departmenstInterfaceHandler.GetDepartmentTreeView();
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Lấy tất cả departments dạng cây
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseObject<DepartmentsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTreeDepartAsync()
        {
            var result = await _departmenstInterfaceHandler.GetTreeDepartAsync();
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// get department by parent (bot current parent) flat
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getbyparentid/{parentId}")]
        [ProducesResponseType(typeof(ResponseObject<DepartmentsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFlatDepartByParentIdAsync(decimal parentId)
        {
            var result = await _departmenstInterfaceHandler.GetFlatDepartByParentIdAsync(parentId);
            return RequestHelpers.TransformData(result);
        }

        /// <summary>
        /// Lấy tất cả departments
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("select")]
        [ProducesResponseType(typeof(ResponseObject<DepartmentsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTreeDepartSelectAsync()
        {
            var result = await _departmenstInterfaceHandler.GetTreeDepartSelectAsync();
            return RequestHelpers.TransformData(result);
        }
        #endregion


        /// <summary>
        /// get AllAncestor by child
        /// </summary>
        /// <param name="childId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{childId}/allancestor")]
        [ProducesResponseType(typeof(ResponseObject<DepartmentsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAncestorAsync(decimal childId)
        {
            var result = await _departmenstInterfaceHandler.GetAllAncestorAsync(childId);
            return RequestHelpers.TransformData(result);
        }
    }
}
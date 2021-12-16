using System.Threading.Tasks;
using Utils;
using API.Infrastructure.Repositories;
using Admin.API.Infrastructure.Migrations;
using Oracle.ManagedDataAccess.Client;
using Dapper.Oracle;
using System.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace API.BussinessLogic
{
    public class DepartmentsHandler : IDepartmentsHandler
    {
        private readonly RepositoryHandler<SaDepartments, DepartmentsBaseModel, DepartmentsQueryModel> _departmentsRepositoryHandler = new RepositoryHandler<SaDepartments, DepartmentsBaseModel, DepartmentsQueryModel>();
        private readonly ILogger<DepartmentsHandler> _logger;
        private readonly decimal _defaultRootDepartmentId;
        public DepartmentsHandler(ILogger<DepartmentsHandler> logger = null)
        {
            string aaa = Helpers.GetConfig("Other:DefaultRootDepartmentId");
            _defaultRootDepartmentId = decimal.Parse(Helpers.GetConfig("Other:DefaultRootDepartmentId"));
            _logger = logger;
        }
        public async Task<Response> UpdateTel(decimal id, string tel)
        {
            try
            {
                var procName = "PKG_SA_DEPARTMENTS.UPDATE_TEL_DEPARTMENT";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("pTEL", tel, OracleMappingType.Varchar2, ParameterDirection.Input);

                return await _departmentsRepositoryHandler.ExecuteProcOracle(procName, dyParam);
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ");
                }
                else throw ex;
            }
        }
        public async Task<Response> GetDepartByBranchsAsync(decimal branchId)
        {
            try
            {
                var procName = "PKG_SA_DEPARTMENTS.GET_BY_BRANCHID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PBRANCHID", branchId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _departmentsRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ");
                }
                else throw ex;
            }
        }
        public async Task<Response> GetTreeDepartAsync()
        {
            try
            {
                var procName = "PKG_SA_DEPARTMENTS.GET_DEPARTMENT_ALL";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);

                var dataResult = await _departmentsRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam) as ResponseObject<List<DepartmentsBaseModel>>;

                if (dataResult != null)
                {
                    var result = GetParents(dataResult.Data);

                    if (result != null && result.Count > 0)
                    {
                        return new ResponseObject<List<DepartmentsTreeModel>>(result, "Thành công", StatusCode.Success);
                    }
                }

                return new ResponseObject<List<DepartmentsTreeModel>>(null, "Không thành công", StatusCode.Fail);
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ");
                }
                else throw ex;
            }
        }

        public async Task<Response> GetFlatDepartByParentIdAsync(decimal parentId)
        {
            try
            {
                var procName = "PKG_SA_DEPARTMENTS.GET_DEPARTMENT_ALL";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);

                var dataResult = await _departmentsRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam) as ResponseObject<List<DepartmentsBaseModel>>;

                if (dataResult?.Data != null)
                {
                    var result = GetByParentFlats(dataResult?.Data, parentId); //dataResult.Data?.Where(sp => sp.Parent_Dept_Id == parentId || sp.Id == parentId).OrderBy(sp => sp.OrderView).ToList(); 

                    if (result != null && result.Count > 0)
                    {
                        return new ResponseObject<List<DepartmentsBaseModel>>(result, "Thành công", StatusCode.Success);
                    }
                }

                return new ResponseObject<List<DepartmentsTreeModel>>(null, "Không thành công", StatusCode.Fail);
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ");
                }
                else throw ex;
            }
        }

        public List<DepartmentsBaseModel> GetByParentFlats(List<DepartmentsBaseModel> dataResult, decimal rootParentId)
        {
            var result = new List<DepartmentsBaseModel>();
            DepartmentsBaseModel parentObject = dataResult.Where(sp => sp.Id == rootParentId).FirstOrDefault();
            result.Add(parentObject);
            List<DepartmentsBaseModel> childObject = dataResult.Where(sp => sp.Parent_Dept_Id == rootParentId).OrderBy(x=>x.OrderView).ToList();
            if(childObject?.Count > 0)
            {

                foreach (DepartmentsBaseModel item in childObject)
                {
                    var subResult = GetByParentFlats(dataResult, item.Id);
                    if(subResult?.Count > 0)
                        result.AddRange(subResult);
                }
            }
            return result;
        }

        public List<DepartmentsTreeModel> GetParents(List<DepartmentsBaseModel> dataResult)
        {
            var result = new List<DepartmentsTreeModel>();
            List<DepartmentsBaseModel> listParent = dataResult.Where(sp => sp.Parent_Dept_Id == _defaultRootDepartmentId).OrderBy(sp => sp.OrderView).ToList();
            int icount = 0;
            foreach (DepartmentsBaseModel item in listParent)
            {
                var itemTree = new DepartmentsTreeModel
                {
                    Id = item.Id,
                    Parent_Dept_Id = item.Parent_Dept_Id,
                    Name = item.Name,
                    Dept_Id = item.Dept_Id,
                    Icon = item.Icon,
                    Tel = item.Tel,
                    OrderView = item.OrderView,
                    Status = item.Status,
                    MainCd = item.MainCd,
                    PosCd = item.PosCd,
                    PostingCode = item.PostingCode
                };


                List<DepartmentsBaseModel> listSub = dataResult.Where(sp => sp.Parent_Dept_Id == item.Id).OrderBy(sp => sp.OrderView).ToList();
                if (listSub.Count > 0)
                {
                    itemTree.Childs = GetSubChild(dataResult, listSub);
                    itemTree.HasChild = true;
                }
                else
                {
                    itemTree.HasChild = false;
                }
                if (icount < listParent.Count - 1)
                {
                    itemTree.HasSeperate = false;
                }
                else
                {
                    itemTree.HasSeperate = true;
                }
                icount++;
                result.Add(itemTree);
            }
            return result;
        }
        public List<DepartmentsTreeModel> GetSubChild(List<DepartmentsBaseModel> dataResult,
                                    List<DepartmentsBaseModel> listParent)
        {
            var result = new List<DepartmentsTreeModel>();
            int icount = 0;
            foreach (DepartmentsBaseModel item in listParent)
            {
                var itemTree = new DepartmentsTreeModel();

                itemTree.Id = item.Id;
                itemTree.Parent_Dept_Id = item.Parent_Dept_Id;
                itemTree.Name = item.Name;
                itemTree.Dept_Id = item.Dept_Id;
                itemTree.Icon = item.Icon;
                itemTree.Tel = item.Tel;
                itemTree.OrderView = item.OrderView;
                itemTree.Status = item.Status;
                itemTree.MainCd = item.MainCd;
                itemTree.PosCd = item.PosCd;
                itemTree.PostingCode = item.PostingCode;

                var resultChilds = new List<DepartmentsTreeModel>();
                List<DepartmentsBaseModel> listSub = dataResult.Where(sp => sp.Parent_Dept_Id == item.Id).OrderBy(sp => sp.OrderView).ToList();
                if (listSub.Count > 0)
                {
                    itemTree.Childs = GetSubChild(dataResult, listSub);
                    itemTree.HasChild = true;
                }
                else
                {
                    itemTree.HasChild = false;
                }
                if (icount < listParent.Count - 1)
                {
                    itemTree.HasSeperate = false;
                }
                else
                {
                    itemTree.HasSeperate = true;
                }
                icount++;
                result.Add(itemTree);
            }
            return result;
        }

        #region GET SELECT
        public async Task<Response> GetTreeDepartSelectAsync()
        {
            try
            {
                var procName = "PKG_SA_DEPARTMENTS.GET_DEPARTMENT_ALL";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);

                var dataResult = await _departmentsRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam) as ResponseObject<List<DepartmentsBaseModel>>;

                if (dataResult != null)
                {
                    var result = GetParentSelect(dataResult.Data);
                    if (result != null && result.Count > 0)
                    {
                        return new ResponseObject<List<DepartmentsBaseModel>>(result, "Thành công", StatusCode.Success);
                    }
                }
                return new ResponseObject<List<DepartmentsBaseModel>>(null, "Không thành công", StatusCode.Fail);
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ");
                }
                else throw ex;
            }
        }
        public List<DepartmentsBaseModel> GetParentSelect(List<DepartmentsBaseModel> dataResult)
        {
            var result = new List<DepartmentsBaseModel>();
            List<DepartmentsBaseModel> listParent = dataResult.Where(sp => sp.Parent_Dept_Id == _defaultRootDepartmentId).OrderBy(sp => sp.OrderView).ToList();
            int icount = 0;
            foreach (var item in listParent)
            {
                string stringTitle = "-";
                var itemTree = item;
                itemTree = item;
                itemTree.NameSelect = stringTitle + " " + item.Name;
                result.Add(itemTree);

                List<DepartmentsBaseModel> listSub = dataResult.Where(sp => sp.Parent_Dept_Id == item.Id).OrderBy(sp => sp.OrderView).ToList();
                if (listSub.Count > 0)
                {
                    List<DepartmentsBaseModel> list = GetChildSelect(dataResult, listSub, stringTitle.ToString());
                    result.AddRange(list);
                }

                icount++;
            }
            return result;
        }
        public List<DepartmentsBaseModel> GetChildSelect(List<DepartmentsBaseModel> dataResult,
                                    List<DepartmentsBaseModel> listParent, string stringTitle)
        {
            var result = new List<DepartmentsBaseModel>();
            int icount = 0;
            foreach (DepartmentsBaseModel item in listParent)
            {
                string titleSub = stringTitle + "-";
                var itemTree = item;
                itemTree.NameSelect = titleSub + " " + item.Name;
                result.Add(itemTree);

                var resultChilds = new List<DepartmentsBaseModel>();
                List<DepartmentsBaseModel> listSub = dataResult.Where(sp => sp.Parent_Dept_Id == item.Id).OrderBy(sp => sp.OrderView).ToList();

                if (listSub.Count > 0)
                {
                    List<DepartmentsBaseModel> list = GetChildSelect(dataResult, listSub, titleSub);
                    result.AddRange(list);
                }

                icount++;

            }
            return result;
        }


        public async Task<Response> GetAllAncestorAsync(decimal currentDepartmentId)
        {
            var result = new List<DepartmentsBaseModel>();

            var procName = "PKG_SA_DEPARTMENTS.GET_DEPARTMENT_ALL";
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);

            var dataResult = await _departmentsRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam) as ResponseObject<List<DepartmentsBaseModel>>;

            if (dataResult == null)
            {
                return new ResponseError(StatusCode.Fail,"Xảy ra lỗi trong quá trình lấy dữ liệu");                
            }

            var curentDepartment = dataResult.Data.FirstOrDefault(sp => sp.Id == currentDepartmentId);
            bool isRootDepartment = (curentDepartment == null || curentDepartment.Id == _defaultRootDepartmentId);
            if (curentDepartment == null)
            {
                return new ResponseError(StatusCode.Fail, "Không tìm thấy phòng ban");
            }
            result.Add(curentDepartment);
            decimal? oldDepartmentID = curentDepartment.Parent_Dept_Id;
            while (!isRootDepartment && oldDepartmentID.HasValue)
            {
                var department = dataResult.Data.FirstOrDefault(sp => sp.Id == oldDepartmentID);
                isRootDepartment = (department == null || department.Id == _defaultRootDepartmentId);
                oldDepartmentID = department.Parent_Dept_Id;
                result.Add(department);
            }
            return new ResponseObject<List<DepartmentsBaseModel>>(result);
            
        }
        #endregion

        #region TREEVIEW
        public async Task<Response> GetDepartmentTreeView()
        {
            try
            {
                var procName = "PKG_SA_DEPARTMENTS.GET_DEPARTMENT_ALL";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);

                if (await _departmentsRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam) is ResponseObject<List<DepartmentsBaseModel>> allDepartments)
                {
                    // Lay root cua cay co cau to chuc. id=1633 la "Đại hội đồng cổ đông"
                    var rootParent = allDepartments.Data.Where(sp => sp.Parent_Dept_Id.Value == _defaultRootDepartmentId).ToList();
                    var fullResult = new List<TreeDepartmentModel>() {
                        new TreeDepartmentModel
                        {
                            Id = _defaultRootDepartmentId,
                            Name = "SHB",
                            IsHasChild = true,
                            ListChilds = GetSubChildsTreeView(allDepartments.Data, rootParent),
                            OrderView = 0,
                            Parent_Dept_Id = 99999,
                            Tel=""
                        }
                    };

                    if (fullResult != null && fullResult.Count > 0) return new ResponseObject<List<TreeDepartmentModel>>(fullResult, "Thành công", StatusCode.Success);
                }

                return new ResponseObject<List<TreeDepartmentModel>>(null, "Không có dữ liệu", StatusCode.Success);
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ");
                }
                else throw ex;
            }
        }
        public List<TreeDepartmentModel> GetSubChildsTreeView(List<DepartmentsBaseModel> flatData,
                                    List<DepartmentsBaseModel> listParents)
        {
            var result = new List<TreeDepartmentModel>();
            foreach (var item in listParents)
            {
                var itemTree = new TreeDepartmentModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Parent_Dept_Id = item.Parent_Dept_Id,
                    OrderView = item.OrderView,
                    IsHasChild = false,
                    ListChilds = new List<TreeDepartmentModel>(),
                    Tel = item.Tel
                };

                List<DepartmentsBaseModel> listSub = flatData.Where(sp => sp.Parent_Dept_Id == item.Id).OrderBy(sp => sp.OrderView).ToList();
                if (listSub.Count > 0)
                {
                    itemTree.ListChilds = GetSubChildsTreeView(flatData, listSub);
                    itemTree.IsHasChild = true;
                }
                else
                {
                    itemTree.IsHasChild = false;
                }
                result.Add(itemTree);
            }
            return result;
        }
        #endregion
    }
}

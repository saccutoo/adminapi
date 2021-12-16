using Admin.API.Infrastructure.Migrations;
using API.Infrastructure.Repositories;
using Dapper.Oracle;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public class FuncPermsHandler : IFuncPermsHandler
    {
        private readonly RepositoryHandler<SaFuncPerms, FuncPermsBaseModel, FuncPermsQueryModel> _funcpermsRepositoryHandler = new RepositoryHandler<SaFuncPerms, FuncPermsBaseModel, FuncPermsQueryModel>();
        private ApplicationHandler _applicationHandler = new ApplicationHandler();
        private FunctionsHandler _functionHandler = new FunctionsHandler();
        private PermissionsHandler _permissionHandler = new PermissionsHandler();
        private readonly ILogger<FuncPermsHandler> _logger;
        public FuncPermsHandler(ILogger<FuncPermsHandler> logger = null)
        {
            _logger = logger;
        }
        public async Task<Response> CreateManyAsync(List<FuncPermsCreateModel> listModel)
        {
            try
            {
                if (listModel != null && listModel.Count > 0)
                {
                    foreach (FuncPermsCreateModel item in listModel)
                    {
                        var resCreate = await CreateAsync(item) as ResponseObject<ResponseModel>;
                    }

                    var result = new ResponseModel
                    {
                        Id = 0,
                        Name = "",
                        Status = "00",
                        Message = "Thành công"
                    };

                    return new ResponseObject<ResponseModel>(result, "Thành công", StatusCode.Success);
                }

                return new ResponseObject<ResponseModel>(null, "Không thành công", StatusCode.Fail);
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
        public async Task<Response> CreateAsync(FuncPermsCreateModel model)
        {
            try
            {
                var procName = "PKG_SA_FUNC_PERMS.ADD_FUNCTION_PERMISSION";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PFUNCTIONID", model.FunctionId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PPERMISSIONID", model.PermissionId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PORDERVIEW", model.OrderView, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PCODE", model.Code, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PNAME", model.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PDESCRIPTION", model.Description, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PISDEFAULT", model.IsDefault, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _funcpermsRepositoryHandler.ExecuteProcOracle(procName, dyParam);
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
        public async Task<Response> UpdateAsync(decimal id, FuncPermsUpdateModel model)
        {
            try
            {
                var existFuncPerms = await FindAsync(id) as ResponseObject<FuncPermsBaseModel>;

                if (existFuncPerms.Data == null)
                {
                    return new ResponseError(StatusCode.Fail, "Không thành công");
                }

                var procName = "PKG_SA_FUNC_PERMS.UPDATE_FUNCTION_PERMISSION";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PORDERVIEW", model.OrderView, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PNAME", model.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PDESCRIPTION", model.Description, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pISDEFAULT", model.IsDefault, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _funcpermsRepositoryHandler.ExecuteProcOracle(procName, dyParam);
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
        public async Task<Response> DeleteAsync(decimal id)
        {
            try
            {
                var procName = "PKG_SA_FUNC_PERMS.DELETE_FUNCTION_PERMISSION";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _funcpermsRepositoryHandler.ExecuteProcOracle(procName, dyParam);

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
        public async Task<Response> DeleteManyAsync(FuncPermsDeleteModel model)
        {
            try
            {
                var lstDeleteResult = new List<FuncPermsResponseModel>();
                using (var unitOfWorkOracle = new UnitOfWorkOracle())
                {
                    if (model.ListId != null && model.ListId.Count > 0)
                    {
                        foreach (var id in model.ListId)
                        {
                            var procName = "PKG_SA_FUNC_PERMS.DELETE_FUNCTION_PERMISSION";
                            var dyParam = new OracleDynamicParameters();
                            dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                            dyParam.Add("PID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                            var deleteResult = await _funcpermsRepositoryHandler.ExecuteProcOracle(procName, unitOfWorkOracle, dyParam) as ResponseObject<ResponseModel>;

                            if (deleteResult != null)
                            {
                                lstDeleteResult.Add(new FuncPermsResponseModel
                                {
                                    Id = deleteResult.Data.Id,
                                    Name = deleteResult.Data.Name,
                                    Status = deleteResult.Data.Status,
                                    Message = (deleteResult.Data.Status.Equals("00") ? "Thành công" : "Không thành công")
                                });
                            }
                        }

                        return new ResponseObject<List<FuncPermsResponseModel>>(lstDeleteResult, "Thành công", StatusCode.Success);
                    }
                    else
                    {
                        return new ResponseObject<List<FuncPermsResponseModel>>(lstDeleteResult, "Không thành công", StatusCode.Fail);
                    }
                }
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
        public async Task<Response> GetFunctionPermissionByFilterAsync(FuncPermsQueryModel model)
        {
            try
            {
                var procName = "PKG_SA_FUNC_PERMS.GET_FUNC_PERMS_BY_FILTER";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PPAGESIZE", model.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PPAGEINDEX", model.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PFULLTEXTSEARCH", model.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PFUNCTIONID", (!model.FunctionId.HasValue || model.FunctionId == -1) ? null : model.FunctionId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _funcpermsRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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

        public async Task<Response> GetAllFuncPermsActiveAsync()
        {
            try
            {
                FuncPermsQueryModel model = new FuncPermsQueryModel()
                {
                    PageIndex = -1,
                    PageSize = 1,
                    FullTextSearch = ""
                };
                var res = await GetFunctionPermissionByFilterAsync(model) as ResponseObject<List<FuncPermsBaseModel>>;

                if (res != null && res.Data.Count > 0)
                {
                    return new ResponseObject<List<FuncPermsBaseModel>>(res.Data,"Thành công", StatusCode.Success);
                }

                return new ResponseObject<List<FuncPermsBaseModel>>(null, "Không thành công", StatusCode.Fail);
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

        public async Task<Response> FindAsync(decimal id)
        {
            try
            {
                var procName = "PKG_SA_FUNC_PERMS.GET_FUNC_PERMS_BY_ID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _funcpermsRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, false);
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

        public FuncPermsResponseModel ReturnResponse(ResponseOracle model)
        {
            FuncPermsResponseModel res = new FuncPermsResponseModel();
            res.Id = model.ID;
            res.Name = model.NAME;
            res.Status = model.STATUS_CODE;
            res.Message = model.STATUS_CODE.Equals("00") ? "Thành công" : "Không thành công";
            return res;
        }

        public async Task<Response> GetTreeFuncPermsByAppIdGroupByFuncAsync(decimal appId, decimal? roleId = null)
        {
            try
            {
                FuncPermsTreeModel itemTree = new FuncPermsTreeModel();

                var appInfo = await _applicationHandler.FindAsync(appId) as ResponseObject<ApplicationBaseModel>;
                List<FuncPermsBaseModel> listByRoleId = new List<FuncPermsBaseModel>();

                if (appInfo == null)
                    return new ResponseObject<FuncPermsTreeModel>(null, "Không thành công", StatusCode.Fail);

                itemTree.ApplicationId = appInfo.Data.Id;
                itemTree.ApplicationName = appInfo.Data.Name;

                if (roleId != null)
                {
                    var resList = await GetFuncPermsByRoleId(roleId) as ResponseObject<List<FuncPermsBaseModel>>;
                    if (resList != null && resList.Data.Count > 0)
                        listByRoleId = resList.Data;
                }

                var listFunction = await _functionHandler.GetAllFunctionByAppIdAsync_1(appInfo.Data.Id) as ResponseObject<List<FunctionsBaseModel>>;

                if (listFunction != null && listFunction.Data.Count > 0)
                {
                    var listTreeFunc = await GetParent(listFunction.Data, listByRoleId);
                    itemTree.ListFunction = listTreeFunc;
                }

                return new ResponseObject<FuncPermsTreeModel>(itemTree, "Thành công", StatusCode.Success);
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

        public async Task<Response> GetTreeFuncPermsByAppIdGroupByPermAsync(decimal appId, decimal? roleId = null)
        {
            try
            {
                FuncPermsTreeByPermModel itemTree = new FuncPermsTreeByPermModel();

                var appInfo = await _applicationHandler.FindAsync(appId) as ResponseObject<ApplicationBaseModel>;
                List<FuncPermsBaseModel> listByRoleId = new List<FuncPermsBaseModel>();

                if (appInfo == null)
                    return new ResponseObject<FuncPermsTreeModel>(null, "Không thành công", StatusCode.Fail);

                itemTree.ApplicationId = appInfo.Data.Id;
                itemTree.ApplicationName = appInfo.Data.Name;

                if (roleId != null)
                {
                    var resList = await GetFuncPermsByRoleId(roleId) as ResponseObject<List<FuncPermsBaseModel>>;
                    if (resList != null && resList.Data.Count > 0)
                        listByRoleId = resList.Data;
                }

                var listPerms = await GetFuncPermsByAppId(appInfo.Data.Id) as ResponseObject<List<FuncPermsBaseModel>>;

                if (listPerms != null && listPerms.Data.Count > 0)
                {
                    var listTreeFunc = await GetParentPerms(roleId, listPerms.Data, listByRoleId);
                    itemTree.ListPerm = listTreeFunc;
                }

                return new ResponseObject<FuncPermsTreeByPermModel>(itemTree, "Thành công", StatusCode.Success);
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

        public async Task<List<FunctionModel>> GetParent(List<FunctionsBaseModel> dataResult, List<FuncPermsBaseModel> listCheck)
        {
            var result = new List<FunctionModel>();
            List<FunctionsBaseModel> listParent = dataResult.Where(sp => sp.ParentId == 0).OrderBy(sp => sp.Orderview).ToList();
            int icount = 0;
            foreach (FunctionsBaseModel item in listParent)
            {
                var itemTree = new FunctionModel();
                itemTree.Id = item.Id;
                itemTree.ApplicationId = item.ApplicationId;
                itemTree.ParentId = item.ParentId;
                itemTree.Name = item.Name;
                itemTree.Code = item.Code;
                itemTree.Icon = item.Icon;
                itemTree.NavigationMobile = item.NavigationMobile;
                itemTree.Url = item.Url;
                itemTree.Orderview = item.Orderview;
                itemTree.Ispublic = item.Ispublic; 
                itemTree.Status = item.Status;
                itemTree.IsChecked = false;

                List<FunctionsBaseModel> listSub = dataResult.Where(sp => sp.ParentId == item.Id).OrderBy(sp => sp.Orderview).ToList();
                var resFuncPerms = await GetFuncPermsByFuncId(item.Id) as ResponseObject<List<FuncPermsBaseModel>>;
                int countCheckFuncPerms = 0;
                if (resFuncPerms != null && resFuncPerms.Data.Count > 0)
                {
                    List<FuncPermsBaseModel> listFuncPerms = resFuncPerms.Data;

                    foreach (FuncPermsBaseModel i in listFuncPerms)
                    {

                        if (listCheck.Any(c => c.PermissionId == i.PermissionId && c.FunctionId == item.Id))
                        {
                            i.IsChecked = true;
                            countCheckFuncPerms++;
                        }
                            
                        else i.IsChecked = false;
                    }

                    itemTree.ListFuncPerm = listFuncPerms;
                }

                if (listSub.Count > 0)
                {
                    itemTree.Childs = await GetChilds(dataResult, listSub, listCheck);

                    if (itemTree.Childs != null)
                    {
                        itemTree.IsChecked = itemTree.Childs.Count(i => i.IsChecked == true) == itemTree.Childs.Count() ? true : false;
                    }
                    if (itemTree.Childs != null && resFuncPerms != null)
                    {
                        itemTree.IsChecked = ((itemTree.Childs.Count(i => i.IsChecked == true) == itemTree.Childs.Count()) && (countCheckFuncPerms == resFuncPerms.Data.Count)) ? true : false;
                    }

                    itemTree.HasChild = true;
                }
                else
                {
                    if (resFuncPerms != null)
                    {
                        itemTree.IsChecked = (countCheckFuncPerms == resFuncPerms.Data.Count) ? true : false;
                    }

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

        public async Task<List<FunctionModel>> GetChilds(List<FunctionsBaseModel> dataResult,
                                    List<FunctionsBaseModel> listParent, List<FuncPermsBaseModel> listCheck)
        {
            var result = new List<FunctionModel>();
            int icount = 0;
            foreach (FunctionsBaseModel item in listParent)
            {
                var itemTree = new FunctionModel();

                itemTree.Id = item.Id;
                itemTree.ApplicationId = item.ApplicationId;
                itemTree.ParentId = item.ParentId;
                itemTree.Name = item.Name;
                itemTree.Code = item.Code;
                itemTree.Icon = item.Icon;
                itemTree.NavigationMobile = item.NavigationMobile;
                itemTree.Url = item.Url;
                itemTree.Orderview = item.Orderview;
                itemTree.Ispublic = item.Ispublic;
                itemTree.Status = item.Status;
                itemTree.IsChecked = false;

                var resultChilds = new List<FunctionModel>();
                List<FunctionsBaseModel> listSub = dataResult.Where(sp => sp.ParentId == item.Id).OrderBy(sp => sp.Orderview).ToList();

                var resFuncPerms = await GetFuncPermsByFuncId(item.Id) as ResponseObject<List<FuncPermsBaseModel>>;
                int countCheckFuncPerms = 0;
                if (resFuncPerms != null && resFuncPerms.Data.Count > 0)
                {
                    List<FuncPermsBaseModel> listFuncPerms = resFuncPerms.Data;
                    foreach (FuncPermsBaseModel i in listFuncPerms)
                    {
                        if (listCheck.Any(c => c.PermissionId == i.PermissionId && c.FunctionId == item.Id))
                        {
                            i.IsChecked = true;
                            countCheckFuncPerms++;
                        }
                        else i.IsChecked = false;
                    }

                    itemTree.ListFuncPerm = listFuncPerms;
                }

                if (listSub.Count > 0)
                {
                    itemTree.Childs = await GetChilds(dataResult, listSub, listCheck);

                    if (itemTree.Childs != null)
                    {
                        itemTree.IsChecked = itemTree.Childs.Count(i => i.IsChecked == true) == itemTree.Childs.Count() ? true : false;
                    }
                    if (itemTree.Childs != null && resFuncPerms != null)
                    {
                        itemTree.IsChecked = ((itemTree.Childs.Count(i => i.IsChecked == true) == itemTree.Childs.Count()) && (countCheckFuncPerms == resFuncPerms.Data.Count)) ? true : false;
                    }

                    itemTree.HasChild = true;
                }
                else
                {
                    if (resFuncPerms != null)
                    {
                        itemTree.IsChecked = (countCheckFuncPerms == resFuncPerms.Data.Count) ? true : false;
                    }

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

        public async Task<Response> GetFuncPermsByRoleId(decimal? roleId)
        {
            try
            {
                var procName = "PKG_SA_FUNC_PERMS.GET_FUNC_PERMS_BY_ROLEID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PROLEID", roleId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _funcpermsRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
        public async Task<Response> GetFuncPermsByFuncId(decimal funcId)
        {
            try
            {
                var procName = "PKG_SA_FUNC_PERMS.GET_FUNC_PERMS_BY_FUNCID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PFUNCID", funcId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _funcpermsRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
        public async Task<Response> GetFuncPermsByFuncIdRoleId(decimal roleId, decimal funcId)
        {
            try
            {
                var procName = "PKG_SA_FUNC_PERMS.GET_BY_FUNCID_ROLEID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PFUNCID", funcId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PROLEID", roleId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _funcpermsRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
        public async Task<List<PermModel>> GetParentPerms(decimal? roleId,List<FuncPermsBaseModel> dataResult, List<FuncPermsBaseModel> listCheck)
        {
            var result = new List<PermModel>();
            ResponseObject<List<PermissionsBaseModel>> resPermis;

            //if(roleId != null)
            //    resPermis = await _permissionHandler.GetPermsByRoleAsync(roleId) as ResponseObject<List<PermissionsBaseModel>>;
            //else
                resPermis = await _permissionHandler.GetAllPermsAsync() as ResponseObject<List<PermissionsBaseModel>>;

            if (resPermis != null && resPermis.Data.Count > 0)
            {
                foreach (PermissionsBaseModel item in resPermis.Data)
                {
                    var itemTree = new PermModel();
                    itemTree.Id = item.Id;
                    itemTree.Name = item.Name;
                    itemTree.Code = item.Code;
                    itemTree.Status = item.Status;
                    itemTree.Description = item.Description;
                    itemTree.OpenDate = item.OpenDate;
                    itemTree.Maker = item.Maker;
                    itemTree.MakerOnDate = item.MakerOnDate;
                    itemTree.Approver = item.Approver;
                    itemTree.ApproverOnDate = item.ApproverOnDate;
                    itemTree.IsChecked = false;
                    itemTree.OrderView = item.OrderView;

                    List<FuncPermsBaseModel> listPerms = dataResult.Where(sp => sp.PermissionId == item.Id).OrderBy(sp => sp.OrderView).ToList();

                    int countCheckFuncPerms = 0;

                    foreach (FuncPermsBaseModel i in listPerms)
                    {
                        if (listCheck.Any(c => c.PermissionId == i.PermissionId && c.Id == i.Id))
                        {
                            i.IsChecked = true;
                            countCheckFuncPerms++;
                        }
                        else i.IsChecked = false;
                    }

                    if (listPerms.Count() > 0)
                    {
                        itemTree.ListFuncPerm = listPerms;
                        itemTree.IsChecked = (countCheckFuncPerms == listPerms.Count) ? true : false;
                    }
                    else
                    {
                        itemTree.IsChecked = false;
                    }

                    result.Add(itemTree);
                }
            }

            return result;
        }
        public async Task<Response> GetFuncPermsByAppId(decimal appId)
        {
            try
            {
                var procName = "PKG_SA_FUNC_PERMS.GET_FUNC_PERMS_BY_APPID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PAPPID", appId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _funcpermsRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
        public async Task<Response> GetFuncPermsByUserId(decimal userId)
        {
            try
            {
                var procName = "PKG_SA_FUNC_PERMS.GET_ALL_FUNC_PERMS_BY_USERID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pUserId", userId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _funcpermsRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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

        public async Task<Response> DeleteManyByListFuncPermsAsync(decimal funcId, string strListFuncPerms)
        {
            try
            {
                var procName = "PKG_SA_FUNC_PERMS.DELETE_NOT_IN_LIST_FUNC_PERMS";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pListFuncPerms", strListFuncPerms, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pFunctionId", funcId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _funcpermsRepositoryHandler.ExecuteProcOracle(procName, dyParam);
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

        public async Task<Response> DeleteAllByFuncIdAsync(decimal funcId)
        {
            try
            {
                var procName = "PKG_SA_FUNC_PERMS.DELETE_ALL_BY_FUNCID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pFunctionId", funcId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _funcpermsRepositoryHandler.ExecuteProcOracle(procName, dyParam);

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
    }
}

using System.Threading.Tasks;
using Utils;
using API.Infrastructure.Repositories;
using Admin.API.Infrastructure.Migrations;
using Oracle.ManagedDataAccess.Client;
using Dapper;
using Dapper.Oracle;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace API.BussinessLogic
{
    public class FunctionsHandler : IFunctionsHandler
    {
        private readonly RepositoryHandler<SaFunctions, FunctionsBaseModel, FunctionsQueryModel> _employeeRepositoryHandler = new RepositoryHandler<SaFunctions, FunctionsBaseModel, FunctionsQueryModel>();

        public FuncPermsHandler _funcPermsHandler;
        public UserPermisionHandler _userpermsHandler;
        public FuncChanelsHandler _funcchanelsHandler;
        private readonly ILogger<FunctionsHandler> _logger;
        public FunctionsHandler(ILogger<FunctionsHandler> logger = null)
        {
            _logger = logger;
        }
        public async Task<Response> CreateAsync(FunctionsCreateModel model, BaseModel baseModel)
        {
            _funcPermsHandler = new FuncPermsHandler();
            _userpermsHandler = new UserPermisionHandler();
            _funcchanelsHandler = new FuncChanelsHandler();

            try
            {
                var procName = "PKG_SA_FUNCTIONS.Add_SA_FUNCTIONS";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PApplicationID", model.ApplicationId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PParentID", model.ParentId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("Plevel_fun", model.Level_Fun, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PName", model.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PCode", model.Code, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PIcon", model.Icon, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PNavigationMobile", model.NavigationMobile, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PURL", model.Url, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("POrderView", model.Orderview, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PIsPublic", model.Ispublic, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PStatus", model.Status, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PMaker", baseModel.Maker, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pcssClass", model.CssClass, OracleMappingType.Varchar2, ParameterDirection.Input);

                var res = await _employeeRepositoryHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

                if (res != null && res.Data.Status.Equals("00"))
                {
                    if (model.ListFuncPerms != null && model.ListFuncPerms.Count > 0)
                    {
                        List<decimal> ListDefault = new List<decimal>();

                        foreach (var item in model.ListFuncPerms)
                        {
                            FuncPermsCreateModel createModel = item;
                            createModel.FunctionId = res.Data.Id;

                            var resAddFuncPerms = await _funcPermsHandler.CreateAsync(createModel) as ResponseObject<ResponseModel>;

                            if (resAddFuncPerms != null 
                                && resAddFuncPerms.Data.Status.Equals("00") && createModel.IsDefault == 1)
                            {
                                ListDefault.Add(resAddFuncPerms.Data.Id);
                            }
                        }

                        if(ListDefault != null && ListDefault.Count > 0)
                        {
                            foreach (var item in ListDefault)
                            {
                                var addUserPerms = await _userpermsHandler.CreateAllUserIdWithFuncPerms(model.ApplicationId, item);
                            }
                        }
                    }

                    if (model.ListChanels != null && model.ListChanels.Count > 0)
                    {
                        foreach (var item in model.ListChanels)
                        {
                            FuncChanelsCreateModel addFuncChanelModel = new FuncChanelsCreateModel()
                            {
                                FunctionId = res.Data.Id,
                                ChanelId = item
                            };

                            var addChanels = await _funcchanelsHandler.CreateAsync(addFuncChanelModel);
                        }
                    }

                    return new ResponseObject<ResponseModel>(res.Data, "Thành công", StatusCode.Success);
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
        public async Task<Response> FindAsync(decimal id)
        {
            try
            {
                var procName = "PKG_SA_FUNCTIONS.GET_BY_ID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _employeeRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, false);
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

        public Task<Response> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Response> GetTreeFunctionsByUserAsync(decimal userId, decimal appId, string channelCode)
        {
            try
            {
                var procName = "PKG_SA_FUNCTIONS.GET_TREE_FUNCTIONS_BY_USER";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PUSERID", userId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PAPPID", appId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PCHANNELCODE", channelCode, OracleMappingType.Varchar2, ParameterDirection.Input);

                var dataResult = await _employeeRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam) as ResponseObject<List<FunctionsBaseModel>>;

                if(dataResult != null)
                {
                    var result = GetMenu(dataResult.Data);

                    if (result != null && result.Count > 0)
                    {
                        return new ResponseObject<List<FunctionsTreeModel>>(result, "Thành công", StatusCode.Success);
                    }
                }
                
                return new ResponseObject<List<FunctionsTreeModel>>(null, "Không thành công", StatusCode.Fail);
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

        public async Task<Response> GetTreeFunctionsWithoutAppAsync(decimal userId, string channelCode)
        {
            try
            {
                var procName = "PKG_SA_FUNCTIONS.GET_TREE_FUNCTIONS_WITHOUT_APP";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PUSERID", userId, OracleMappingType.Decimal, ParameterDirection.Input);               
                dyParam.Add("PCHANNELCODE", channelCode, OracleMappingType.Varchar2, ParameterDirection.Input);

                var dataResult = await _employeeRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam) as ResponseObject<List<FunctionsBaseModel>>;

                if (dataResult != null)
                {
                    var result = GetMenu(dataResult.Data);

                    if (result != null && result.Count > 0)
                    {
                        return new ResponseObject<List<FunctionsTreeModel>>(result, "Thành công", StatusCode.Success);
                    }
                }

                return new ResponseObject<List<FunctionsTreeModel>>(null, "Không thành công", StatusCode.Fail);
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
        public List<FunctionsTreeModel> GetMenu(List<FunctionsBaseModel> dataResult)
        {
            var result = new List<FunctionsTreeModel>();
            List<FunctionsBaseModel> listParent = dataResult.Where(sp=>sp.ParentId == 0).OrderBy(sp=>sp.Orderview).ToList();
            int icount = 0;
            foreach (FunctionsBaseModel item in listParent)
            {
                var itemTree = new FunctionsTreeModel();
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
                itemTree.CssClass = item.CssClass;

                List<FunctionsBaseModel> listSub = dataResult.Where(sp => sp.ParentId == item.Id).OrderBy(sp => sp.Orderview).ToList();
                if(listSub.Count > 0)
                {
                    itemTree.Childs = GetMenuSub(dataResult, listSub);
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
        public List<FunctionsTreeModel> GetMenuSub(List<FunctionsBaseModel> dataResult,
                                    List<FunctionsBaseModel> listParent)
        {
            var result = new List<FunctionsTreeModel>();
            int icount = 0;
            foreach (FunctionsBaseModel item in listParent)
            {
                var itemTree = new FunctionsTreeModel();

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
                itemTree.CssClass = item.CssClass;

                var resultChilds = new List<FunctionsTreeModel>();
                List<FunctionsBaseModel> listSub = dataResult.Where(sp => sp.ParentId == item.Id).OrderBy(sp => sp.Orderview).ToList();
                if (listSub.Count > 0)
                {
                    itemTree.Childs = GetMenuSub(dataResult, listSub);
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
        public async Task<Response> GetAllFunctionByAppIdAsync(decimal appId, string type = null)
        {
            try
            {
                if (type != null)
                {
                    var res = await GetAllFunctionByAppIdAsync_1(appId) as ResponseObject<List<FunctionsBaseModel>>;
                    if (res != null && res.Data.Count > 0)
                    {
                        List<FunctionsBaseModel> result = GetParentSelect(res.Data);
                        return new ResponseObject<List<FunctionsBaseModel>>(result, "Thành công", StatusCode.Success);
                    }
                    else
                        return new ResponseObject<List<FunctionsBaseModel>>(null, "Không thành công", StatusCode.Fail);
                }
                else
                {
                    var res = await GetAllFunctionByAppIdAsync_1(appId) as ResponseObject<List<FunctionsBaseModel>>;
                    if(res != null)
                        return new ResponseObject<List<FunctionsBaseModel>>(res.Data, "Thành công",StatusCode.Success);
                    else
                        return new ResponseObject<List<FunctionsBaseModel>>(null, "Không thành công", StatusCode.Fail);
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
        public async Task<Response> GetAllFunctionByAppIdAsync_1(decimal appId)
        {
            try
            {
                var procName = "PKG_SA_FUNCTIONS.GET_ALL_FUNC_BY_APPID_1";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PAPPLICATIONID", appId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _employeeRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
        public async Task<Response> GetFunctionByRoleId(decimal roleId)
        {
            try
            {
                var procName = "PKG_SA_FUNCTIONS.GET_FUNC_BY_ROLEID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PROLEID", roleId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _employeeRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
        public List<FunctionsBaseModel> GetParentSelect(List<FunctionsBaseModel> dataResult)
        {
            var result = new List<FunctionsBaseModel>();
            result.Add(new FunctionsBaseModel() {
                Id = 0,
                NameSelect = "--Chọn chức năng cha--"
            });
            List<FunctionsBaseModel> listParent = dataResult.Where(sp => sp.ParentId == 0).OrderBy(sp => sp.Orderview).ToList();
            int icount = 0;
            foreach (FunctionsBaseModel item in listParent)
            {
                string stringTitle = "-";
                var itemTree = item;
                itemTree = item;
                itemTree.NameSelect = stringTitle + " " + item.Name;
                result.Add(itemTree);

                List<FunctionsBaseModel> listSub = dataResult.Where(sp => sp.ParentId == item.Id).OrderBy(sp => sp.Orderview).ToList();
                if (listSub.Count > 0)
                {
                    List<FunctionsBaseModel> list = GetChildSelect(dataResult, listSub, stringTitle.ToString());
                    result.AddRange(list);
                }

                icount++;
            }
            return result;
        }
        public List<FunctionsBaseModel> GetChildSelect(List<FunctionsBaseModel> dataResult,
                                    List<FunctionsBaseModel> listParent, string stringTitle)
        {
            var result = new List<FunctionsBaseModel>();
            int icount = 0;
            foreach (FunctionsBaseModel item in listParent)
            {
                string titleSub = stringTitle + "-";
                var itemTree = item;
                itemTree.NameSelect = titleSub + " "  + item.Name;
                result.Add(itemTree);

                var resultChilds = new List<FunctionsBaseModel>();
                List<FunctionsBaseModel> listSub = dataResult.Where(sp => sp.ParentId == item.Id).OrderBy(sp => sp.Orderview).ToList();

                if (listSub.Count > 0)
                {
                    List<FunctionsBaseModel> list = GetChildSelect(dataResult, listSub, titleSub);
                    result.AddRange(list);
                }

                icount++;
                
            }
            return result;
        }

        public async Task<Response> UpdateAsync(decimal id, FunctionsUpdateModel model)
        {
            _funcPermsHandler = new FuncPermsHandler();
            _userpermsHandler = new UserPermisionHandler();
            _funcchanelsHandler = new FuncChanelsHandler();

            var funcQuery = await FindAsync(id) as ResponseObject<FunctionsBaseModel>;

            if (funcQuery == null || funcQuery.Data == null)
                return new ResponseError(StatusCode.Fail, "Không thành công");
            
            try
            {
                var procName = "PKG_SA_FUNCTIONS.Update_SA_FUNCTIONS";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PParentID", model.ParentId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("Plevel_fun", model.Level_Fun, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PName", model.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PIcon", model.Icon, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PNavigationMobile", model.NavigationMobile, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PURL", model.Url, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("POrderView", model.Orderview, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PIsPublic", model.Ispublic, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PStatus", model.Status, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pcssClass", model.CssClass, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                var res = await _employeeRepositoryHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

                if (res != null && res.Data.Status.Equals("00"))
                {
                    if (model.ListFuncPerms != null && model.ListFuncPerms.Count > 0)
                    {
                        string idFuncPermsDisable = "";
                        List<decimal> ListDefault = new List<decimal>();
                        string idFuncPerms = "";

                        foreach (var item in model.ListFuncPerms)
                        {
                            if (item.IsDisable == 1)
                            {
                                idFuncPerms += item.Id + "|";

                                if (item.IsDefault == 1) ListDefault.Add(item.Id);

                                idFuncPermsDisable += item.Id + "|";

                                FuncPermsUpdateModel updateFuncPerModel = new FuncPermsUpdateModel()
                                {
                                    Name = item.Name,
                                    OrderView = item.OrderView,
                                    Description = item.Description,
                                    IsDefault = item.IsDefault
                                };

                                var resupdateFuncPerms = await _funcPermsHandler.UpdateAsync(item.Id, updateFuncPerModel);
                            }
                        }

                        var delFunPerms = await _funcPermsHandler.DeleteManyByListFuncPermsAsync(id,idFuncPermsDisable) as ResponseObject<ResponseModel>;
                        
                        if (delFunPerms != null && delFunPerms.Data.Status.Equals("00"))
                        {
                            foreach (var item in model.ListFuncPerms)
                            {
                                if (item.IsDisable != 1)
                                {
                                    FuncPermsCreateModel createModel = item;
                                    createModel.FunctionId = res.Data.Id;

                                    var resAddFuncPerms = await _funcPermsHandler.CreateAsync(createModel) as ResponseObject<ResponseModel>;

                                    if (resAddFuncPerms != null
                                        && resAddFuncPerms.Data.Status.Equals("00"))
                                    {
                                        if (createModel.IsDefault == 1)
                                        {
                                            ListDefault.Add(resAddFuncPerms.Data.Id);
                                        }

                                        idFuncPerms += item.Id + "|";
                                    }
                                }
                            }

                            var delUserPerms = await _userpermsHandler.DeleteUserPermsByListFuncPerms(idFuncPerms) as ResponseObject<ResponseModel>;
                            if (delUserPerms != null && delUserPerms.Data.Status.Equals("00"))
                            {
                                if (ListDefault != null && ListDefault.Count > 0)
                                {
                                    foreach (var item in ListDefault)
                                    {
                                        var addUserPerms = await _userpermsHandler.CreateAllUserIdWithFuncPerms(model.ApplicationId, item);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var delAllFuncPerms = await _funcPermsHandler.DeleteAllByFuncIdAsync(id);
                    }

                    var listChanel = await _funcchanelsHandler.DeleteByFuncIdAsync(id) as ResponseObject<ResponseModel>;
                    
                    if (listChanel != null && listChanel.Data.Status.Equals("00") 
                        && model.ListChanels != null && model.ListChanels.Count > 0)
                    {
                        foreach (var item in model.ListChanels)
                        {
                            FuncChanelsCreateModel addFuncChanelModel = new FuncChanelsCreateModel()
                            {
                                FunctionId = res.Data.Id,
                                ChanelId = item
                            };

                            var addChanels = await _funcchanelsHandler.CreateAsync(addFuncChanelModel);
                        }
                    }

                    return new ResponseObject<ResponseModel>(res.Data, "Thành công", StatusCode.Success);
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
        public async Task<Response> GetFunctionsByFilterAsync(FunctionsQueryModel filterModel)
        {
            try
            {
                var procName = "PKG_SA_FUNCTIONS.GET_SA_FUNCTIONS_BY_FILTER";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pPageSize", filterModel.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("pPageIndex", filterModel.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("pFullTextSearch", filterModel.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pApplicationId", (!filterModel.ApplicationId.HasValue || filterModel.ApplicationId == -1) ? null : filterModel.ApplicationId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("pStatus", (filterModel.Status == "ALL" || string.IsNullOrEmpty(filterModel.Status)) ? null : filterModel.Status, OracleMappingType.Varchar2, ParameterDirection.Input);

                return await _employeeRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
            _funcPermsHandler = new FuncPermsHandler();
            _funcchanelsHandler = new FuncChanelsHandler();

            try
            {
                var getFunc = await _funcPermsHandler.GetFuncPermsByFuncId(id) as ResponseObject<List<FuncPermsBaseModel>>;

                if (getFunc != null && getFunc.Data.Count > 0)
                {
                    if(getFunc.Data.Count(i => i.IsDisable == 1) > 0)
                    {
                        return new ResponseError(StatusCode.Fail, "Không thành công");
                    }
                }

                var delChanel = await _funcchanelsHandler.DeleteByFuncIdAsync(id) as ResponseObject<ResponseModel>;

                var delFuncPerms = await _funcPermsHandler.DeleteAllByFuncIdAsync(id) as ResponseObject<ResponseModel>;

                if (delChanel != null && delFuncPerms != null && delChanel.Data.Status.Equals("00") && delFuncPerms.Data.Status.Equals("00"))
                {
                    var procName = "PKG_SA_FUNCTIONS.DELETE_SA_FUNCTIONS";
                    var dyParam = new OracleDynamicParameters();
                    dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam.Add("PID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                    return await _employeeRepositoryHandler.ExecuteProcOracle(procName, dyParam);
                }

                return new ResponseError(StatusCode.Fail, "Không thành công");
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

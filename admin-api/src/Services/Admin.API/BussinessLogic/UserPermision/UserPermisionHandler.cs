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
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
//Chuyenlt1 05/03/2019
namespace API.BussinessLogic
{
    public class UserPermisionHandler : IUserPermisionHandler
    {
        private readonly RepositoryHandler<SaUserPerms, UserPermisionBaseModel, UserPermisionQueryModel> _userpermisionRepositoryHandler = new RepositoryHandler<SaUserPerms, UserPermisionBaseModel, UserPermisionQueryModel>();
        private ApplicationHandler _applicationHandler;
        private UserPermisionHandler _userpermsHandler;
        private RoleHandler _rolesHandler;
        private FunctionsHandler _funcHandler;
        private FuncPermsHandler _funcpermsHandler;
        private PermissionsHandler _permissionHandler;
        private UserRolesHandler _userroleHandler;
        private LogActionsHandler _logActionHandler;
        private UserGroupHandler _userGroupHandler;
        private readonly ILogger<UserPermisionHandler> _logger;
        public UserPermisionHandler(ILogger<UserPermisionHandler> logger = null)
        {
            _logger = logger;
        }
        public async Task<Response> CreateAsync(UserPermisionCreateModel model)
        {
            try
            {
                var procName = "PKG_SA_USER_PERMS.AddFunctionPermissionOfUser";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PUSERID", model.Userid, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PAPPLICATIONID", model.Applicationid, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PFUNCPERMID", model.Funcpermid, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PROLEID", model.RoleId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _userpermisionRepositoryHandler.ExecuteProcOracle(procName, dyParam);
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

        public async Task<Response> UpdateAsync(decimal userId, UserPermisionUpdateModel model)
        {
            _userroleHandler = new UserRolesHandler();
            _logActionHandler = new LogActionsHandler();
            try
            {

                var deleteByUserId = await DeleteUserPermsOfUser(userId) as ResponseObject<ResponseModel>;

                if (deleteByUserId == null || !deleteByUserId.Data.Status.Equals("00"))
                    return new ResponseError(StatusCode.Fail, "Không thành công");

                var delUserRolesByUserId = await _userroleHandler.DeleteRolesByUserId(userId) as ResponseObject<ResponseModel>;
                if (delUserRolesByUserId == null || !delUserRolesByUserId.Data.Status.Equals("00"))
                    return new ResponseObject<UserPermisionResponseModel>(null, "Không thành công", StatusCode.Fail);


                if (model.ListFuncPerms != null && model.ListFuncPerms.Count > 0)
                {
                    // Lay id Nhom nguoi dung
                    List<decimal> lstRoleId = model.ListFuncPerms.Select(sp => sp.RoleId.Value).Distinct().ToList();

                    if (lstRoleId != null && lstRoleId.Count > 0)
                    {
                        foreach (var item in lstRoleId)
                        {

                            var insUserRole = await _userroleHandler.CreateAsync(new UserRolesCreateModel()
                            {
                                UserId = userId,
                                RoleId = item
                            }) as ResponseObject<UserRolesResponseModel>;
                        }
                    }

                    foreach (var item in model.ListFuncPerms)
                    {
                        UserPermisionCreateModel modelCreate = new UserPermisionCreateModel()
                        {
                            Applicationid = item.Applicationid,
                            Funcpermid = item.Funcpermid,
                            RoleId = item.RoleId,
                            Userid = userId
                        };

                        var insUserPerm = await CreateAsync(modelCreate);
                    }

                    var res = new ResponseModel
                    {
                        Id = 0,
                        Name = "",
                        Status = "00",
                        Message = "Thành công"
                    };

                    // Log Action
                    var logActionsModel = new LogActionsModel
                    {
                        FunctionCode = "UPDATE_USER_PERMISSION",
                        FunctionName = "Phân quyền người dùng",
                        ActionByUser = model.Maker,
                        ActionName = "Cập nhật",
                        ActionType = "UPDATE",
                        ActionTime = DateTime.Now,
                        ObjectContentNew = JsonConvert.SerializeObject(model)
                    };

                    await _logActionHandler.AddLog(logActionsModel);
                    return new ResponseObject<ResponseModel>(res, "Thành công", StatusCode.Success);
                }
                else if (model.ListFuncPerms == null || model.ListFuncPerms.Count < 1)
                {
                    var res = new ResponseModel
                    {
                        Id = 0,
                        Name = "",
                        Status = "00",
                        Message = "Thành công"
                    };

                    // Log Action
                    var logActionsModel = new LogActionsModel
                    {
                        FunctionCode = "UPDATE_USER_PERMISSION",
                        FunctionName = "Phân quyền người dùng",
                        ActionByUser = model.Maker,
                        ActionName = "Cập nhật",
                        ActionType = "UPDATE",
                        ActionTime = DateTime.Now,
                        ObjectContentNew = JsonConvert.SerializeObject(model)
                    };

                    await _logActionHandler.AddLog(logActionsModel);
                    return new ResponseObject<ResponseModel>(res, "Thành công", StatusCode.Success);
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
        public async Task<Response> UpdateFunctionPermissionOfUserAsync(UserPermisioUpdateModel model)
        {
            try
            {
                var lstUpdateResult = new List<UserPermisionResponseModel>();
                using (var unitOfWorkOracle = new UnitOfWorkOracle())
                {
                    if (model.UpdateModel != null && model.UpdateModel.Count > 0)
                    {
                        for (int i = 0; i < model.UpdateModel.Count; i++)
                        {
                            var procName = "PKG_SA_USER_PERMS.UpdateFunctionPermissionOfUser";
                            var dyParam = new OracleDynamicParameters();
                            dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                            //dyParam.Add("pID", model.updateModel[i].Permissionid, OracleMappingType.Decimal, ParameterDirection.Input);
                            dyParam.Add("PUSERID", model.UpdateModel[i].Userid, OracleMappingType.Decimal, ParameterDirection.Input);
                            dyParam.Add("pApplicationID", model.UpdateModel[i].Applicationid, OracleMappingType.Decimal, ParameterDirection.Input);
                            dyParam.Add("pFuncPermID", model.UpdateModel[i].Funcpermid, OracleMappingType.Decimal, ParameterDirection.Input);
                            var updateResult = await _userpermisionRepositoryHandler.ExecuteProcOracle(procName, unitOfWorkOracle, dyParam) as ResponseObject<ResponseModel>;

                            if (updateResult != null)
                            {
                                lstUpdateResult.Add(new UserPermisionResponseModel
                                {
                                    Id = updateResult.Data.Id,
                                    Name = updateResult.Data.Name,
                                    Status = updateResult.Data.Status,
                                    Message = (updateResult.Data.Status.Equals("00") ? "Thành công" : "Không thành công")
                                });
                            }
                        }

                        return new ResponseObject<List<UserPermisionResponseModel>>(lstUpdateResult, "Thành công", StatusCode.Success);
                    }
                    else
                    {
                        return new ResponseObject<List<UserPermisionResponseModel>>(lstUpdateResult, "Không thành công", StatusCode.Fail);
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
        public async Task<Response> DeleteUserPermsOfUser(decimal userId)
        {
            try
            {
                var procName = "PKG_SA_USER_PERMS.DELETEUserPermsOfUser";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PUSERID", userId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _userpermisionRepositoryHandler.ExecuteProcOracle(procName, dyParam);
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
        public async Task<Response> DeleteUserPermsByRoleId(decimal roleId)
        {
            try
            {
                var procName = "PKG_SA_USER_PERMS.DELETE_FUNC_PERMS_BY_ROLEID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PROLEID", roleId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _userpermisionRepositoryHandler.ExecuteProcOracle(procName, dyParam);
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
        public async Task<Response> GetUserPermsByUserId(decimal? userId)
        {
            try
            {
                var procName = "PKG_SA_USER_PERMS.GET_USER_PERMS_BY_USERID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PUSERID", userId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _userpermisionRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
        public async Task<Response> GetTreeFuncPermByUserIdGroupByFunc(decimal? userId = null)
        {
            _applicationHandler = new ApplicationHandler();
            _userpermsHandler = new UserPermisionHandler();
            _rolesHandler = new RoleHandler();
            _funcHandler = new FunctionsHandler();


            try
            {
                List<TreeUserPermModel> listUserPerms = new List<TreeUserPermModel>();
                var resApplication = await _applicationHandler.GetAllAsync() as ResponseObject<List<ApplicationBaseModel>>;
                List<UserPermisionBaseModel> listUserPermsByUserId = new List<UserPermisionBaseModel>();

                if (userId != null)
                {
                    var resList = await _userpermsHandler.GetUserPermsByUserId(userId) as ResponseObject<List<UserPermisionBaseModel>>;
                    if (resList != null && resList.Data.Count > 0)
                        listUserPermsByUserId = resList.Data;
                }

                if (resApplication != null && resApplication.Data.Count > 0)
                {
                    foreach (ApplicationBaseModel item in resApplication.Data)
                    {
                        TreeUserPermModel itemTree = new TreeUserPermModel();
                        List<RoleByAppIdModel> listRoles = new List<RoleByAppIdModel>();

                        itemTree.ApplicationId = item.Id;
                        itemTree.ApplicationName = item.Name;
                        itemTree.IsChecked = false;

                        var resRolesByAppId = await _rolesHandler.GetRolesByAppId(item.Id) as ResponseObject<List<RolesViewModel>>;


                        if (resRolesByAppId != null && resRolesByAppId.Data.Count > 0)
                        {
                            foreach (RolesViewModel itm in resRolesByAppId.Data)
                            {
                                RoleByAppIdModel itemRole = new RoleByAppIdModel();
                                var serialized = JsonConvert.SerializeObject(itm);
                                itemRole = JsonConvert.DeserializeObject<RoleByAppIdModel>(serialized);
                                itemRole.IsChecked = false;

                                var resFuncByRole = await _funcHandler.GetFunctionByRoleId(itm.Id) as ResponseObject<List<FunctionsBaseModel>>;

                                if (resFuncByRole != null)
                                {
                                    var result = await GetParent(resFuncByRole.Data, listUserPermsByUserId, itm.Id);

                                    if (result != null && result.Count > 0)
                                    {
                                        itemRole.ListFunctions = result;

                                        itemRole.IsChecked = (result.Count(i => i.IsChecked == true) == result.Count) ? true : false;

                                    }
                                }

                                listRoles.Add(itemRole);
                            }
                        }

                        if (listRoles != null)
                            itemTree.IsChecked = (listRoles.Count(i => i.IsChecked == true) == listRoles.Count) ? true : false;

                        itemTree.ListRole = listRoles;

                        listUserPerms.Add(itemTree);
                    }
                }

                return new ResponseObject<List<TreeUserPermModel>>(listUserPerms, "Thành công", StatusCode.Success);
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
        public async Task<List<TreeFuncPermsModel>> GetParent(List<FunctionsBaseModel> dataResult, List<UserPermisionBaseModel> listCheck, decimal roleId)
        {
            _funcpermsHandler = new FuncPermsHandler();
            _permissionHandler = new PermissionsHandler();
            var result = new List<TreeFuncPermsModel>();
            List<FunctionsBaseModel> listParent = dataResult.Where(sp => sp.ParentId == 0).OrderBy(sp => sp.Orderview).ToList();
            int icount = 0;
            foreach (FunctionsBaseModel item in listParent)
            {
                var itemTree = new TreeFuncPermsModel();
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

                var resFuncPerms = await _funcpermsHandler.GetFuncPermsByFuncIdRoleId(roleId, item.Id) as ResponseObject<List<FuncPermsBaseModel>>;
                int countCheckFuncPerms = 0;

                if (resFuncPerms != null && resFuncPerms.Data.Count > 0)
                {
                    List<FuncPermsBaseModel> listFuncPerms = resFuncPerms.Data;
                    foreach (FuncPermsBaseModel i in listFuncPerms)
                    {
                        if (listCheck.Any(c => c.FuncpermId == i.Id))
                        {
                            i.IsChecked = true;
                            countCheckFuncPerms++;
                        }

                        else i.IsChecked = false;
                    }

                    itemTree.ListFuncPerms = listFuncPerms;
                }

                if (listSub.Count > 0)
                {
                    itemTree.Childs = await GetChild(dataResult, listSub, listCheck, roleId);

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
        public async Task<List<TreeFuncPermsModel>> GetChild(List<FunctionsBaseModel> dataResult,
                                    List<FunctionsBaseModel> listParent, List<UserPermisionBaseModel> listCheck, decimal roleId)
        {
            _funcpermsHandler = new FuncPermsHandler();
            _permissionHandler = new PermissionsHandler();
            var result = new List<TreeFuncPermsModel>();
            int icount = 0;
            foreach (FunctionsBaseModel item in listParent)
            {
                var itemTree = new TreeFuncPermsModel();

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

                var resultChilds = new List<FunctionsTreeModel>();
                List<FunctionsBaseModel> listSub = dataResult.Where(sp => sp.ParentId == item.Id).OrderBy(sp => sp.Orderview).ToList();

                var resFuncPerms = await _funcpermsHandler.GetFuncPermsByFuncIdRoleId(roleId, item.Id) as ResponseObject<List<FuncPermsBaseModel>>;
                int countCheckFuncPerms = 0;

                if (resFuncPerms != null && resFuncPerms.Data.Count > 0)
                {
                    List<FuncPermsBaseModel> listFuncPerms = resFuncPerms.Data;
                    foreach (FuncPermsBaseModel i in listFuncPerms)
                    {
                        if (listCheck.Any(c => c.FuncpermId == i.Id))
                        {
                            i.IsChecked = true;
                            countCheckFuncPerms++;
                        }
                        else i.IsChecked = false;
                    }

                    itemTree.ListFuncPerms = listFuncPerms;
                }

                if (listSub.Count > 0)
                {
                    itemTree.Childs = await GetChild(dataResult, listSub, listCheck, roleId);

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
        public async Task<Response> GetTreeFuncPermByUserIdGroupByPerms(decimal? userId = null)
        {
            _applicationHandler = new ApplicationHandler();
            _userpermsHandler = new UserPermisionHandler();
            _rolesHandler = new RoleHandler();
            _funcpermsHandler = new FuncPermsHandler();

            try
            {
                List<TreeUserPerByPerms> listUserPerms = new List<TreeUserPerByPerms>();
                var resApplication = await _applicationHandler.GetAllAsync() as ResponseObject<List<ApplicationBaseModel>>;
                List<UserPermisionBaseModel> listUserPermsByUserId = new List<UserPermisionBaseModel>();

                if (userId != null)
                {
                    var resList = await _userpermsHandler.GetUserPermsByUserId(userId) as ResponseObject<List<UserPermisionBaseModel>>;
                    if (resList != null && resList.Data.Count > 0)
                        listUserPermsByUserId = resList.Data;
                }

                if (resApplication != null && resApplication.Data.Count > 0)
                {
                    foreach (ApplicationBaseModel item in resApplication.Data)
                    {
                        TreeUserPerByPerms itemTree = new TreeUserPerByPerms();
                        List<RoleByAppIdGroupByPermsModel> listRoles = new List<RoleByAppIdGroupByPermsModel>();

                        itemTree.ApplicationId = item.Id;
                        itemTree.ApplicationName = item.Name;
                        itemTree.IsChecked = false;

                        var resRolesByAppId = await _rolesHandler.GetRolesByAppId(item.Id) as ResponseObject<List<RolesViewModel>>;

                        if (resRolesByAppId != null && resRolesByAppId.Data.Count > 0)
                        {
                            foreach (RolesViewModel itm in resRolesByAppId.Data)
                            {
                                RoleByAppIdGroupByPermsModel itemRole = new RoleByAppIdGroupByPermsModel();
                                var serialized = JsonConvert.SerializeObject(itm);
                                itemRole = JsonConvert.DeserializeObject<RoleByAppIdGroupByPermsModel>(serialized);
                                itemRole.IsChecked = false;

                                var resFuncPermsByRole = await _funcpermsHandler.GetFuncPermsByRoleId(itm.Id) as ResponseObject<List<FuncPermsBaseModel>>;

                                if (resFuncPermsByRole != null)
                                {
                                    var result = await GetParentGroupByPerms(itm.Id, resFuncPermsByRole.Data, listUserPermsByUserId);

                                    if (result != null && result.Count > 0)
                                    {
                                        itemRole.ListPerms = result;
                                        itemRole.IsChecked = (result.Count(i => i.IsChecked == true) == result.Count) ? true : false;
                                    }
                                }

                                listRoles.Add(itemRole);
                            }
                        }

                        if (listRoles != null)
                            itemTree.IsChecked = (listRoles.Count(i => i.IsChecked == true) == listRoles.Count) ? true : false;

                        itemTree.ListRole = listRoles;

                        listUserPerms.Add(itemTree);
                    }
                }

                return new ResponseObject<List<TreeUserPerByPerms>>(listUserPerms, "Thành công", StatusCode.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception Error");
                return new ResponseError(StatusCode.Fail, ex.ToString());
            }
        }
        public async Task<List<TreeFuncPermsGroupByPermsModel>> GetParentGroupByPerms(decimal? roleId, List<FuncPermsBaseModel> dataResult, List<UserPermisionBaseModel> listCheck)
        {
            _permissionHandler = new PermissionsHandler();
            var result = new List<TreeFuncPermsGroupByPermsModel>();
            var resPermis = await _permissionHandler.GetPermsByRoleAsync(roleId) as ResponseObject<List<PermissionsBaseModel>>;

            if (resPermis != null && resPermis.Data.Count > 0)
            {
                foreach (PermissionsBaseModel item in resPermis.Data)
                {
                    var itemTree = new TreeFuncPermsGroupByPermsModel();
                    var serialized = JsonConvert.SerializeObject(item);
                    itemTree = JsonConvert.DeserializeObject<TreeFuncPermsGroupByPermsModel>(serialized);
                    itemTree.IsChecked = false;

                    List<FuncPermsBaseModel> listPerms = dataResult.Where(sp => sp.PermissionId == item.Id).OrderBy(sp => sp.OrderView).ToList();

                    int countCheckFuncPerms = 0;

                    foreach (FuncPermsBaseModel i in listPerms)
                    {
                        if (listCheck.Any(c => c.FuncpermId == i.Id))
                        {
                            i.IsChecked = true;
                            countCheckFuncPerms++;
                        }
                        else i.IsChecked = false;
                    }

                    if (listPerms.Count() > 0)
                    {
                        itemTree.ListFuncPerms = listPerms;
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
        public async Task<Response> CreateAllUserIdWithFuncPerms(decimal? appId, decimal funcpermsId)
        {
            try
            {
                var procName = "PKG_SA_USER_PERMS.ADD_ALL_USERID_WITH_FUNCPERMID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pApplicationId", appId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("pFuncPermsId", funcpermsId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _userpermisionRepositoryHandler.ExecuteProcOracle(procName, dyParam);
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
        public async Task<Response> DeleteUserPermsByListFuncPerms(string listFuncPerms)
        {
            try
            {
                var procName = "PKG_SA_USER_PERMS.DELETE_IN_LIST_FUNC_PERMS";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pListFuncPerms", listFuncPerms, OracleMappingType.Varchar2, ParameterDirection.Input);

                return await _userpermisionRepositoryHandler.ExecuteProcOracle(procName, dyParam);
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
        #region Application - Roles
        public async Task<Response> GetTreeAllAppThenRole(decimal? userGroupId = null)
        {
            _applicationHandler = new ApplicationHandler();
            _rolesHandler = new RoleHandler();
            _userGroupHandler = new UserGroupHandler();
            try
            {
                List<TreeUserPermModel> listUserPerms = new List<TreeUserPermModel>();
                var listRoleIdByUserGroup = new List<decimal>();
                var resApplication = await _applicationHandler.GetApplicationActive() as ResponseObject<List<ApplicationBaseModel>>;

                if (userGroupId.HasValue)
                {
                    var getUserGroupById = await _userGroupHandler.GetById(userGroupId.Value) as ResponseObject<UserGroupModel>;
                    if (getUserGroupById != null && getUserGroupById.StatusCode == StatusCode.Success && getUserGroupById.Data.ListRoleId.Count > 0)
                        listRoleIdByUserGroup = getUserGroupById.Data.ListRoleId;
                }

                if (resApplication != null && resApplication.Data.Count > 0)
                {
                    foreach (ApplicationBaseModel item in resApplication.Data)
                    {
                        TreeUserPermModel itemTree = new TreeUserPermModel();
                        List<RoleByAppIdModel> listRoles = new List<RoleByAppIdModel>();

                        itemTree.ApplicationId = item.Id;
                        itemTree.ApplicationName = item.Name;
                        itemTree.IsChecked = false;

                        var resRolesByAppId = await _rolesHandler.GetRolesByAppId(item.Id) as ResponseObject<List<RolesViewModel>>;

                        if (resRolesByAppId != null && resRolesByAppId.Data.Count > 0)
                        {
                            foreach (RolesViewModel itm in resRolesByAppId.Data)
                            {
                                RoleByAppIdModel itemRole = new RoleByAppIdModel();
                                var serialized = JsonConvert.SerializeObject(itm);
                                itemRole = JsonConvert.DeserializeObject<RoleByAppIdModel>(serialized);
                                itemRole.IsChecked = false;
                                if (listRoleIdByUserGroup != null && listRoleIdByUserGroup.Count > 0)
                                    itemRole.IsChecked = listRoleIdByUserGroup.Contains(itemRole.Id);
                                listRoles.Add(itemRole);
                            }
                        }

                        if (listRoles != null)
                            itemTree.IsChecked = (listRoles.Count(i => i.IsChecked == true) == listRoles.Count) ? true : false;

                        itemTree.ListRole = listRoles;

                        listUserPerms.Add(itemTree);
                    }
                }

                return new ResponseObject<List<TreeUserPermModel>>(listUserPerms, "Thành công", StatusCode.Success);
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
        #endregion
    }
}

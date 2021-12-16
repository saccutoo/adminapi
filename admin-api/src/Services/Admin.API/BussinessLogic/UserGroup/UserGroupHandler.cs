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
using Newtonsoft.Json;

namespace API.BussinessLogic
{
    public class UserGroupHandler : IUserGroupHandler
    {
        private readonly RepositoryHandler<SaUserGroup, UserGroupModel, UserGroupQueryModel> _repositoryHandler
            = new RepositoryHandler<SaUserGroup, UserGroupModel, UserGroupQueryModel>();

        private readonly RepositoryHandler<SaRoles, RolesViewModel, RolesQueryModel> _rolesRepositoryHandler
            = new RepositoryHandler<SaRoles, RolesViewModel, RolesQueryModel>();

        private ApplicationHandler _applicationHandler;
        private RoleHandler _rolesHandler;
        private readonly ILogger<UserGroupHandler> _logger;
        public UserGroupHandler(ILogger<UserGroupHandler> logger = null)
        {
            _logger = logger;
        }
        public async Task<Response> CreateAsync(UserGroupCreateModel model)
        {
            try
            {
                using (var unitOfWorkOracle = new UnitOfWorkOracle())
                {
                    var iConn = unitOfWorkOracle.GetConnection();
                    var iTrans = iConn.BeginTransaction();

                    var procName = "PKG_SA_USER_GROUP.CREATE_RECORD";
                    var dyParam = new OracleDynamicParameters();
                    dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam.Add("P_NAME", model.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_DESCRIPTION", model.Description, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_ORDERVIEW", model.OrderView, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("P_CREATEDBY", model.CreatedBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                    var createRes = await _repositoryHandler.ExecuteProcOracle(procName, iConn, iTrans, dyParam) as ResponseObject<ResponseModel>;
                    if (createRes != null && createRes.StatusCode == StatusCode.Success)
                    {
                        var userGroupId = createRes.Data.Id;
                        if (model.ListRoleId.Count > 0)
                        {
                            foreach (var roleId in model.ListRoleId)
                            {
                                try
                                {
                                    var procName2 = "PKG_SA_USER_GROUP.CREATE_USERGROUP_ROLES";
                                    var dyParam2 = new OracleDynamicParameters();
                                    dyParam2.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                                    dyParam2.Add("P_USERGROUPID", userGroupId, OracleMappingType.Decimal, ParameterDirection.Input);
                                    dyParam2.Add("P_ROLEID", roleId, OracleMappingType.Decimal, ParameterDirection.Input);

                                    var executeResult1 = await _repositoryHandler.ExecuteProcOracle(procName2, iConn, iTrans, dyParam2) as ResponseObject<ResponseModel>;
                                    if (executeResult1 == null || executeResult1.StatusCode == StatusCode.Fail || executeResult1.Data == null)
                                    {
                                        iTrans.Rollback();
                                        return new ResponseError(StatusCode.Fail, executeResult1.Message);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    iTrans.Rollback();
                                    if (_logger != null) _logger.LogError(ex, "Exception Error");
                                    return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ");
                                }

                            }
                        }
                    }
                    iTrans.Commit();
                    return createRes;
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
        public async Task<Response> UpdateAsync(decimal id, UserGroupUpdateModel model)
        {
            try
            {
                using (var unitOfWorkOracle = new UnitOfWorkOracle())
                {
                    var iConn = unitOfWorkOracle.GetConnection();
                    var iTrans = iConn.BeginTransaction();

                    var procName = "PKG_SA_USER_GROUP.UPDATE_RECORD";
                    var dyParam = new OracleDynamicParameters();
                    dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam.Add("P_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("P_NAME", model.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_DESCRIPTION", model.Description, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_STATUS", model.Status, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_ORDERVIEW", model.OrderView, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("P_LASTMODIFIEDBY", model.LastModifiedBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                    var updateRes = await _repositoryHandler.ExecuteProcOracle(procName, iConn, iTrans, dyParam) as ResponseObject<ResponseModel>;
                    if (updateRes != null && updateRes.StatusCode == StatusCode.Success)
                    {
                        //2. Xóa toàn bộ phân quyền cũ: nhóm người dùng, nhóm quyền, quyền kế thừa ( không bao gồm quyền mặc định )
                        var procName1 = "PKG_SA_USER_GROUP.DROP_USERPER_BY_USERGROUP";
                        var dyParam1 = new OracleDynamicParameters();
                        dyParam1.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                        dyParam1.Add("P_USERGROUPID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                        var dropUserPerRes = await _repositoryHandler.ExecuteProcOracle(procName1, iConn, iTrans, dyParam1) as ResponseObject<ResponseModel>;
                        if (dropUserPerRes != null && dropUserPerRes.StatusCode == StatusCode.Success)
                        {
                            if (model.ListRoleId.Count > 0)
                            {
                                foreach (var roleId in model.ListRoleId)
                                {
                                    var procName2 = "PKG_SA_USER_GROUP.CREATE_USERGROUP_ROLES";
                                    var dyParam2 = new OracleDynamicParameters();
                                    dyParam2.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                                    dyParam2.Add("P_USERGROUPID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                                    dyParam2.Add("P_ROLEID", roleId, OracleMappingType.Decimal, ParameterDirection.Input);

                                    var executeResult1 = await _repositoryHandler.ExecuteProcOracle(procName2, iConn, iTrans, dyParam2) as ResponseObject<ResponseModel>;
                                    if (executeResult1.StatusCode == StatusCode.Fail && executeResult1.Data != null)
                                    {
                                        iTrans.Rollback();
                                        return new ResponseError(StatusCode.Fail, executeResult1.Message);
                                    }
                                }
                            }
                            //3. Khởi tạo lại dữ liệu phân quyền cho user thuộc nhóm
                            var procName3 = "PKG_SA_USER_GROUP.RECREATE_USER_USERGROUP";
                            var dyParam3 = new OracleDynamicParameters();
                            dyParam3.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                            dyParam3.Add("P_USERGROUPID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                            var reCreateUserPerRes = await _repositoryHandler.ExecuteProcOracle(procName3, iConn, iTrans, dyParam3) as ResponseObject<ResponseModel>;
                            if (reCreateUserPerRes == null || reCreateUserPerRes.StatusCode == StatusCode.Fail || reCreateUserPerRes.Data == null)
                            {
                                iTrans.Rollback();
                                return new ResponseError(StatusCode.Fail, reCreateUserPerRes.Message);
                            }
                        }
                        else
                        {
                            iTrans.Rollback();
                            return dropUserPerRes;
                        }
                    }
                    iTrans.Commit();
                    return updateRes;
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
        public async Task<Response> DeleteByIdAsync(decimal id)
        {
            try
            {
                var procName = "PKG_SA_USER_GROUP.DELETE_BY_ID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                var deleteRes = await _repositoryHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;
                if (deleteRes != null && deleteRes.StatusCode == StatusCode.Fail)
                {
                    if (deleteRes.Data.Name == "HAS_REFERENCE") return new ResponseError(StatusCode.Fail, "Nhóm người dùng đang được gán user!");
                }
                return deleteRes;
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
        public async Task<Response> GetByFilterAsync(UserGroupQueryModel queryModel)
        {
            try
            {
                var procName = "PKG_SA_USER_GROUP.GET_BY_FILTER";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_PAGE_SIZE", queryModel.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_PAGE_INDEX", queryModel.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_FULL_TEXT_SEARCH", queryModel.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_STATUS", (queryModel.Status == "ALL" || string.IsNullOrEmpty(queryModel.Status)) ? null : queryModel.Status, OracleMappingType.Varchar2, ParameterDirection.Input);

                return await _repositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
        public async Task<Response> GetById(decimal id)
        {
            try
            {
                var procName = "PKG_SA_USER_GROUP.GET_BY_ID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                var getByIdRes = await _repositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, false) as ResponseObject<UserGroupModel>;
                if (getByIdRes != null && getByIdRes.StatusCode == StatusCode.Success)
                {
                    var procName1 = "PKG_SA_USER_GROUP.GET_ROLES_BY_USERGROUP";
                    var dyParam1 = new OracleDynamicParameters();
                    dyParam1.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam1.Add("P_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                    var getRolesByUserGroup = await _rolesRepositoryHandler.ExecuteProcOracleReturnRow(procName1, dyParam1, true) as ResponseObject<List<RolesViewModel>>;
                    if (getRolesByUserGroup != null && getRolesByUserGroup.StatusCode == StatusCode.Success)
                    {
                        getByIdRes.Data.ListRoleId = new List<decimal>();
                        getByIdRes.Data.ListRoleId.AddRange(getRolesByUserGroup.Data.Select(sp => sp.Id));
                    }
                }
                return getByIdRes;
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
        public async Task<Response> GetAllByUser(decimal? userId = null)
        {
            try
            {
                var procName = "PKG_SA_USER_GROUP.GET_ALL_ACTIVE";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);

                var getAllActiveRes = await _repositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true) as ResponseObject<List<UserGroupModel>>;
                if (getAllActiveRes != null && getAllActiveRes.StatusCode == StatusCode.Success)
                {
                    if (userId.HasValue)
                    {
                        var procName1 = "PKG_SA_USER_GROUP.GET_BY_USERID";
                        var dyParam1 = new OracleDynamicParameters();
                        dyParam1.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                        dyParam1.Add("P_USERID", userId.Value, OracleMappingType.Decimal, ParameterDirection.Input);

                        var getAllByUser = await _repositoryHandler.ExecuteProcOracleReturnRow(procName1, dyParam1, true) as ResponseObject<List<UserGroupModel>>;
                        if (getAllByUser != null && getAllByUser.StatusCode == StatusCode.Success && getAllByUser.Data.ToList().Count > 0)
                        {
                            var lstAllGroupByUser = getAllByUser.Data;
                            foreach (var item in getAllActiveRes.Data)
                            {
                                item.IsChecked = getAllByUser.Data.Exists(sp => sp.Id == item.Id);
                            }
                        }
                    }
                }
                return getAllActiveRes;
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

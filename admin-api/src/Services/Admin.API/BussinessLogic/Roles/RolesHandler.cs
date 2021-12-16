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
    public class RoleHandler : IRolesHandler
    {
        private readonly RepositoryHandler<SaRoles, RolesViewModel, RolesQueryModel> _rolesRepositoryHandler = new RepositoryHandler<SaRoles, RolesViewModel, RolesQueryModel>();

        private RolePermsHandler _rolepermsHandler;
        private UserRolesHandler _userrolesHandler;
        private UserPermisionHandler _userpermsHandler;
        private ApplicationHandler _applicationHandler;
        private readonly ILogger<RoleHandler> _logger;
        public RoleHandler(ILogger<RoleHandler> logger = null)
        {
            _logger = logger;
        }
        public async Task<Response> CreateAsync(RolesCreateModel model, BaseModel baseModel)
        {
            _rolepermsHandler = new RolePermsHandler();
            _userrolesHandler = new UserRolesHandler();
            _userpermsHandler = new UserPermisionHandler();
            _applicationHandler = new ApplicationHandler();

            try
            {
                var appCode = string.Empty;
                var application = await _applicationHandler.FindAsync(model.ApplicationId.Value) as ResponseObject<ApplicationBaseModel>;
                if (application.Data != null) appCode = application.Data.Code;

                var procName = "PKG_SA_ROLES.ADD_ROLES";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PAPPLICATIONID", model.ApplicationId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PAPPLICATIONCODE", appCode, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PNAME", model.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PDESCRIPTION", model.Description, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PSTATUS", model.Status, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PMaker", baseModel.Maker, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PMAKERONDATE", DateTime.Now, OracleMappingType.TimeStamp, ParameterDirection.Input);

                var res = await _rolesRepositoryHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

                if (res != null && res.Data.Status.Equals("00"))
                {
                    if (model.ListFuncPermsId != null && model.ListFuncPermsId.Count > 0)
                    {
                        foreach (decimal i in model.ListFuncPermsId)
                        {
                            RolePermsCreateModel modelRolePerm = new RolePermsCreateModel()
                            {
                                FuncpermId = (long)i,
                                RoleId = (long)res.Data.Id
                            };
                            var inserRolePerm = await _rolepermsHandler.CreateAsync(modelRolePerm);
                        }
                    }

                    return new ResponseObject<ResponseModel>(res.Data, "Thành công", StatusCode.Success);
                }
                else
                {
                    return new ResponseObject<ResponseModel>(res.Data, "Không thành công", StatusCode.Fail);
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
        public async Task<Response> UpdateAsync(decimal id, RolesUpdateModel model)
        {
            _rolepermsHandler = new RolePermsHandler();
            _userrolesHandler = new UserRolesHandler();
            _userpermsHandler = new UserPermisionHandler();

            try
            {
                var roleQuery = await FindAsync(id) as ResponseObject<RolesViewModel>;

                if (roleQuery.Data == null)
                {
                    return new ResponseError(StatusCode.Fail, "Role không tồn tại");
                }
                else
                {
                    var procNam = "PKG_SA_ROLES.UPDATE_ROLES";
                    var dyParm = new OracleDynamicParameters();
                    dyParm.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParm.Add("PID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParm.Add("PNAME", model.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParm.Add("PDESCRIPTION", model.Description, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParm.Add("PSTATUS", model.Status, OracleMappingType.Varchar2, ParameterDirection.Input);

                    var res = await _rolesRepositoryHandler.ExecuteProcOracle(procNam, dyParm) as ResponseObject<ResponseModel>;

                    if (res != null && res.Data.Status.Equals("00"))
                    {
                        if (model.ListFuncPermsId != null && model.ListFuncPermsId.Count > 0)
                        {
                            var delRolePerm = await _rolepermsHandler.DeleteByIdAsync(id) as ResponseObject<ResponseModel>;

                            if (delRolePerm != null && delRolePerm.Data.Status.Equals("00"))
                            {
                                foreach (decimal i in model.ListFuncPermsId)
                                {
                                    RolePermsCreateModel modelRolePerm = new RolePermsCreateModel()
                                    {
                                        FuncpermId = (long)i,
                                        RoleId = (long)id
                                    };
                                    var inserRolePerm = await _rolepermsHandler.CreateAsync(modelRolePerm);
                                }
                            }

                            List<UserRolesViewModel> listUserId = new List<UserRolesViewModel>();
                            var getUserByRoleId = await _userrolesHandler.GetUserByRoleIdAsync(id) as ResponseObject<List<UserRolesViewModel>>;

                            if (getUserByRoleId != null && getUserByRoleId.Data.Count > 0)
                                listUserId = getUserByRoleId.Data;

                            var deluserperm = await _userpermsHandler.DeleteUserPermsByRoleId(id) as ResponseObject<ResponseModel>;

                            if (deluserperm != null && deluserperm.Data.Status.Equals("00"))
                            {
                                foreach (UserRolesBaseModel item in listUserId)
                                {
                                    foreach (decimal i in model.ListFuncPermsId)
                                    {
                                        UserPermisionCreateModel userpermCreate = new UserPermisionCreateModel()
                                        {
                                            Userid = item.UserId,
                                            Applicationid = roleQuery.Data.ApplicationId,
                                            Funcpermid = i,
                                            RoleId = id
                                        };

                                        var insuserperm = await _userpermsHandler.CreateAsync(userpermCreate);
                                    }
                                }
                            }
                        }

                        return new ResponseObject<ResponseModel>(res.Data, "Thành công", StatusCode.Success);
                    }
                    else
                    {
                        return new ResponseObject<ResponseModel>(res.Data, "Không thành công", StatusCode.Fail);
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
        public async Task<Response> DeleteAsync(decimal id)
        {
            _userrolesHandler = new UserRolesHandler();
            _rolepermsHandler = new RolePermsHandler();

            try
            {
                var getByUserId = await _userrolesHandler.GetRolesOfUserAsync(id) as ResponseObject<List<UserRolesBaseModel>>;

                if (getByUserId != null && getByUserId.Data.Count > 0)
                {
                    return new ResponseError(StatusCode.Fail, "Không thành công");
                }

                var delRolePerms = await _rolepermsHandler.DeleteByIdAsync(id) as ResponseObject<ResponseModel>;

                if (delRolePerms != null && delRolePerms.Data.Status.Equals("00"))
                {
                    var procName = "PKG_SA_ROLES.DELETE_ROLES";
                    var dyParam = new OracleDynamicParameters();
                    dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam.Add("PID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                    return await _rolesRepositoryHandler.ExecuteProcOracle(procName, dyParam);
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
        public async Task<Response> DeleteRoleMany(List<decimal> listRoleId)
        {
            var listResult = new List<RoleResponseModel>();
            using (var unitOfWorkOracle = new UnitOfWorkOracle())
            {
                if (listRoleId != null && listRoleId.Count > 0)
                {
                    foreach (var id in listRoleId)
                    {
                        var procName = "PKG_SA_ROLES.DELETE_ROLES";
                        var dyParam = new OracleDynamicParameters();
                        dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                        dyParam.Add("PID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                        var deleteResult = await _rolesRepositoryHandler.ExecuteProcOracle(procName, unitOfWorkOracle, dyParam) as ResponseObject<ResponseModel>;

                        if (deleteResult != null)
                        {
                            listResult.Add(new RoleResponseModel
                            {
                                Id = deleteResult.Data.Id,
                                Name = deleteResult.Data.Name,
                                Status = deleteResult.Data.Status,
                                Message = (deleteResult.Data.Status.Equals("00") ? "Thanh cong" : "Khong thanh cong")
                            }
                                );
                        }
                        //return await _rolesRepositoryHandler.ExecuteProcOracle(procName, dyParam);

                    }
                    return new ResponseObject<List<RoleResponseModel>>(listResult, "Thanh cong");
                }
                else
                {
                    return new ResponseObject<List<RoleResponseModel>>(listResult, "Khong Thanh cong");
                }

            }

        }
        public async Task<Response> FindAsync(decimal id)
        {
            try
            {
                var procNam = "PKG_SA_ROLES.GET_BY_ID";
                var dyParm = new OracleDynamicParameters();
                dyParm.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParm.Add("PID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                return await _rolesRepositoryHandler.ExecuteProcOracleReturnRow(procNam, dyParm, false);
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
        public async Task<Response> GetRoleByFuncPermAsync(string listFuncPerms)
        {
            try
            {
                var procNam = "PKG_SA_ROLES.GET_ROLES_BY_FUNCPERMID";
                var dyParm = new OracleDynamicParameters();
                dyParm.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParm.Add("PLISTFUNCPERMID", listFuncPerms, OracleMappingType.Varchar2, ParameterDirection.Input);
                return await _rolesRepositoryHandler.ExecuteProcOracleReturnRow(procNam, dyParm, false);
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
        public async Task<Response> GetAllAsync()
        {
            try
            {
                var procName = "PKG_SA_ROLES.GET_ALL";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);

                return await _rolesRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
        public async Task<Response> GetRolesByFilterAsync(RolesQueryModel filterModel)
        {
            try
            {
                var procName = "PKG_SA_ROLES.GET_ROLES_BY_FILTER";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pPageSize", filterModel.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("pPageIndex", filterModel.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("pFullTextSearch", filterModel.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pApplicationId", (!filterModel.ApplicationId.HasValue || filterModel.ApplicationId == -1) ? null : filterModel.ApplicationId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("pStatus", (filterModel.Status == "ALL" || string.IsNullOrEmpty(filterModel.Status)) ? null : filterModel.Status, OracleMappingType.Varchar2, ParameterDirection.Input);

                return await _rolesRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
        public async Task<Response> GetRoleById(decimal id)
        {
            try
            {
                var procName = "PKG_SA_ROLES.GET_BY_ID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                return await _rolesRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, false);
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
        public async Task<Response> GetRolesByAppId(decimal appId)
        {
            try
            {
                var procName = "PKG_SA_ROLES.GET_ROLES_BY_APPID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PAPPID", appId, OracleMappingType.Decimal, ParameterDirection.Input);
                return await _rolesRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
        public RoleResponseModel ReturnResponse(ResponseOracle model)
        {
            RoleResponseModel res = new RoleResponseModel();
            res.Id = model.ID;
            res.Name = model.NAME;
            res.Status = model.STATUS_CODE;
            res.Message = model.STATUS_CODE.Equals("00") ? "Thành công" : "Không thành công";
            return res;
        }
    }
}

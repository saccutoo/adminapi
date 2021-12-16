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
    public class UserRolesHandler : IUserRolesHandler
    {
        private readonly RepositoryHandler<SaUserRoles, UserRolesViewModel, UserQueryModel> _userRolesRepositoryHandler = new RepositoryHandler<SaUserRoles, UserRolesViewModel, UserQueryModel>();
        private readonly ILogger<UserRolesHandler> _logger;
        public UserRolesHandler(ILogger<UserRolesHandler> logger = null)
        {
            _logger = logger;
        }
        public async Task<Response> CreateAsync(UserRolesCreateModel model)
        {
            try
            {
                var procName = "PKG_SA_USER_ROLES.ADD_USER_ROLES";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PUSERID", model.UserId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PROLEID", model.RoleId, OracleMappingType.Varchar2, ParameterDirection.Input);

                return await _userRolesRepositoryHandler.ExecuteProcOracle(procName, dyParam);
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
        public async Task<Response> UpdateAsync(decimal id, UserRolesBaseModel model)
        {
            try
            {
                var existUserRoles = await FindAsync(id) as ResponseObject<UserRolesBaseModel>;

                if (existUserRoles.Data == null)
                {
                    return new ResponseError(StatusCode.Fail, "Không thành công");
                }

                var procName = "PKG_SA_USER_ROLES.UPDATE_USER_ROLES";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PUSERID", (!string.IsNullOrEmpty(model.UserId.ToString())) ? model.UserId : existUserRoles.Data.UserId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PROLEID", (!string.IsNullOrEmpty(model.RoleId.ToString())) ? model.RoleId : existUserRoles.Data.RoleId, OracleMappingType.Varchar2, ParameterDirection.Input);

                return await _userRolesRepositoryHandler.ExecuteProcOracle(procName, dyParam);
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
                var procName = "PKG_SA_USER_ROLES.GET_ALL";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);

                return await _userRolesRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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

        public async Task<Response> GetRolesOfUserAsync(decimal userId)
        {
            try
            {
                var procName = "PKG_SA_USER_ROLES.GET_ROLES_OF_USER";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PUSERID", userId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _userRolesRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
        public async Task<Response> GetUserByRoleIdAsync(decimal roleId)
        {
            try
            {
                var procName = "PKG_SA_USER_ROLES.GET_USER_BY_ROLEID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PROLEID", roleId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _userRolesRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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

        public async Task<Response> GetRolesNotOfUserAsync(decimal userId)
        {
            try
            {
                var procName = "PKG_SA_USER_ROLES.GET_ROLES_NOT_OF_USER";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PUSERID", userId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _userRolesRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
                var procName = "PKG_SA_USER_ROLES.GET_BY_ID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _userRolesRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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

        public async Task<Response> DeleteRolesByUserId(decimal userId)
        {
            try
            {
                var procName = "PKG_SA_USER_ROLES.DELETE_USER_ROLES_BY_USERID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PUSERID", userId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _userRolesRepositoryHandler.ExecuteProcOracle(procName, dyParam);
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

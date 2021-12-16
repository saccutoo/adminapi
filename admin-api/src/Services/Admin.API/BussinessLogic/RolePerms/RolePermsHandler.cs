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
    public class RolePermsHandler : IRolePermsHandler
    {
        private readonly RepositoryHandler<SaRolePerms, RolePermsBaseModel, RolePermsQueryModel> _rolesPermsRepositoryHandler = new RepositoryHandler<SaRolePerms, RolePermsBaseModel, RolePermsQueryModel>();
        private readonly ILogger<RolePermsHandler> _logger;
        public RolePermsHandler(ILogger<RolePermsHandler> logger = null)
        {
            _logger = logger;
        }
        public async Task<Response> CreateAsync(RolePermsCreateModel model)
        {
            try
            {
                var procName = "PKG_SA_ROLE_PERMS.ADD_ROLE_PERMS";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PFUNCPERMID", model.FuncpermId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PROLEID", model.RoleId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _rolesPermsRepositoryHandler.ExecuteProcOracle(procName, dyParam);
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
        public async Task<Response> DeleteByIdAsync(decimal roleId)
        {
            try
            {
                var procName = "PKG_SA_ROLE_PERMS.DELETE_ROLE_PERMS_BY_ROLEID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PROLEID", roleId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _rolesPermsRepositoryHandler.ExecuteProcOracle(procName, dyParam);
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

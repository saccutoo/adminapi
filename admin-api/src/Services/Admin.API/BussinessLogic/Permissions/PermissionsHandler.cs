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
    public class PermissionsHandler : IPermissionsHandler
    {
        private readonly RepositoryHandler<SaPermissions, PermissionsBaseModel, PermissonsQueryModel> _permissionRepositoryHandler = new RepositoryHandler<SaPermissions, PermissionsBaseModel, PermissonsQueryModel>();
        private readonly ILogger<PermissionsHandler> _logger;
        public PermissionsHandler(ILogger<PermissionsHandler> logger = null)
        {
            _logger = logger;
        }
        public async Task<Response> CreateAsync(PermissionsCreateModel model, BaseModel baseModel)
        {
            try
            {
                var procName = "PKG_SA_APPLICATIONS.INSERT_ROW";
                var dyParam = new OracleDynamicParameters();
                //dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                //dyParam.Add("PNAME", model.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                //dyParam.Add("PCODE", model.Code, OracleMappingType.Varchar2, ParameterDirection.Input);
                //dyParam.Add("PICON", model.Icon, OracleMappingType.Varchar2, ParameterDirection.Input);
                //dyParam.Add("PCSSCLASS", model.Cssclass, OracleMappingType.Varchar2, ParameterDirection.Input);
                //dyParam.Add("PURL", model.Url, OracleMappingType.Varchar2, ParameterDirection.Input);
                //dyParam.Add("PORDERVIEW", model.OrderView, OracleMappingType.Decimal, ParameterDirection.Input);
                //dyParam.Add("PISPUBLIC", model.IsPublic, OracleMappingType.Decimal, ParameterDirection.Input);
                //dyParam.Add("PSTATUS", model.Status, OracleMappingType.Varchar2, ParameterDirection.Input);
                //dyParam.Add("POPENDATE", DateTime.Now, OracleMappingType.TimeStamp, ParameterDirection.Input);
                //dyParam.Add("PMAKER", baseModel.Maker, OracleMappingType.Varchar2, ParameterDirection.Input);
                //dyParam.Add("PMAKERONDATE", baseModel.MakerOnDate, OracleMappingType.TimeStamp, ParameterDirection.Input);

                return await _permissionRepositoryHandler.ExecuteProcOracle(procName, dyParam);
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

        public async Task<Response> GetAllPermsAsync(decimal? funcId = null)
        {
            try
            {
                if (funcId != null)
                {
                    List<PermissionsBaseModel> listResult = new List<PermissionsBaseModel>();
                    var resByFuncId = await GetPermsByFuncAsync(funcId) as ResponseObject<List<PermissionsBaseModel>>;
                    var resAll = await GetAllPermsAsync() as ResponseObject<List<PermissionsBaseModel>>;

                    if (resByFuncId != null && resByFuncId.Data.Count > 0 && resAll != null && resAll.Data.Count > 0)
                    {
                        foreach (var item in resAll.Data)
                        {
                            item.IsDisable = 0;

                            if (resByFuncId.Data.Any(itm => itm.Id == item.Id))
                                item.IsChecked = true;

                            if (resByFuncId.Data.Any(itm => itm.Id == item.Id && itm.IsDisable == 1))
                                item.IsDisable = 1;
                        }
                    }

                    if(resByFuncId == null && resAll != null && resAll.Data.Count > 0)
                    {
                        foreach (var item in resAll.Data)
                        {
                            item.IsDisable = 0;
                        }
                    }

                    return new ResponseObject<List<PermissionsBaseModel>>(resAll.Data, "Thành công", StatusCode.Success);
                }
                else
                {
                    var procName = "PKG_SA_PERMISSIONS.GET_ALL";
                    var dyParam = new OracleDynamicParameters();
                    dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);

                    return await _permissionRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
        public async Task<Response> GetPermsByAppIdAsync(decimal appId)
        {
            try
            {
                var procName = "PKG_SA_PERMISSIONS.GET_PERMS_BY_APPID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PAPPID", appId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _permissionRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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

        public async Task<Response> GetPermsByFuncAsync(decimal? funcId)
        {
            try
            {
                var procName = "PKG_SA_PERMISSIONS.GET_PERMS_BY_FUNCID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PFUNCID", funcId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _permissionRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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

        public async Task<Response> GetPermsByRoleAsync(decimal? roleId)
        {
            try
            {
                var procName = "PKG_SA_PERMISSIONS.GET_PERMS_BY_ROLEID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PROLEID", roleId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _permissionRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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

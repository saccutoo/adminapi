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
using Microsoft.Extensions.Logging;

namespace API.BussinessLogic
{
    public class ApplicationHandler : IApplicationHandler
    {
        private readonly RepositoryHandler<SaApplications, ApplicationBaseModel, ApplicationQueryModel> _employeeRepositoryHandler = new RepositoryHandler<SaApplications, ApplicationBaseModel, ApplicationQueryModel>();
        private readonly ILogger<ApplicationHandler> _logger;
        public ApplicationHandler(ILogger<ApplicationHandler> logger = null)
        {
            _logger = logger;
        }
        public async Task<Response> CreateAsync(ApplicationCreateModel model, BaseModel baseModel)
        {
            try
            {
                var procName = "PKG_SA_APPLICATIONS.INSERT_ROW";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PNAME", model.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PCODE", model.Code, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PICON", model.Icon, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PCSSCLASS", model.Cssclass, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PURL", model.Url, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PORDERVIEW", model.OrderView, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PISPUBLIC", model.IsPublic, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PSTATUS", model.Status, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("POPENDATE", DateTime.Now, OracleMappingType.TimeStamp, ParameterDirection.Input);
                dyParam.Add("PMAKER", baseModel.Maker, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PMAKERONDATE", baseModel.MakerOnDate, OracleMappingType.TimeStamp, ParameterDirection.Input);

                return await _employeeRepositoryHandler.ExecuteProcOracle(procName, dyParam);
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

        public async Task<Response> UpdateAsync(decimal id, ApplicationCreateModel model)
        {
            try
            {
                var procName = "PKG_SA_APPLICATIONS.UPDATE_ROW";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PNAME", model.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PCODE", model.Code, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PICON", model.Icon, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PCSSCLASS", model.Cssclass, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PURL", model.Url, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PORDERVIEW", model.OrderView, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PISPUBLIC", model.IsPublic, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PSTATUS", model.Status, OracleMappingType.Varchar2, ParameterDirection.Input);

                return await _employeeRepositoryHandler.ExecuteProcOracle(procName, dyParam);
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
                var procName = "PKG_SA_APPLICATIONS.GET_BY_ID";
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

        public async Task<Response> GetAllAsync()
        {
            try
            {
                var procName = "PKG_SA_APPLICATIONS.GET_ALL";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PTYPE", "ALL", OracleMappingType.Varchar2, ParameterDirection.Input);

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

        public async Task<Response> GetApplicationActive()
        {
            try
            {
                var procName = "PKG_SA_APPLICATIONS.GET_ALL";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PTYPE", "ACTIVE", OracleMappingType.Varchar2, ParameterDirection.Input);

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

        public async Task<Response> GetApplicationByUserAsync(decimal userId, string channelCode)
        {
            try
            {
                var procName = "PKG_SA_APPLICATIONS.GET_APPLICATION_BY_USER";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PUSERID", userId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PCHANNELCODE", channelCode, OracleMappingType.Varchar2, ParameterDirection.Input);

                //var dataResult = await _employeeRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam) as ResponseObject<List<ApplicationBaseModel>>;

                return await _employeeRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam);
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

        public async Task<Response> GetApplicationByRolesIdAsync(decimal roleId)
        {
            try
            {
                var procName = "PKG_SA_APPLICATIONS.GET_APPLICATION_BY_ROLE_ID";
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

        public async Task<Response> DeleteAsync(decimal id)
        {
            try
            {
                var procName = "PKG_SA_APPLICATIONS.DELETE_ROW";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _employeeRepositoryHandler.ExecuteProcOracle(procName, dyParam);
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

        public async Task<Response> GetApplicationByFilterAsync(ApplicationQueryModel filterModel)
        {
            try
            {
                var procName = "PKG_SA_APPLICATIONS.GET_APPLICATION_BY_FILTER";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pPageSize", filterModel.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("pPageIndex", filterModel.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("pFullTextSearch", filterModel.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);
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
    }
}

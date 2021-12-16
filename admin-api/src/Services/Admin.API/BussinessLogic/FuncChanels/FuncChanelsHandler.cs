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
    public class FuncChanelsHandler : IFuncChanelsHandler
    {
        private readonly RepositoryHandler<SaFuncChanels, FuncChanelsBaseModel, FuncChanelsQueryModel> _funcchanelssRepositoryHandler = new RepositoryHandler<SaFuncChanels, FuncChanelsBaseModel, FuncChanelsQueryModel>();
        private readonly ILogger<FuncChanelsHandler> _logger;
        public FuncChanelsHandler(ILogger<FuncChanelsHandler> logger = null)
        {
            _logger = logger;
        }
        public async Task<Response> CreateAsync(FuncChanelsCreateModel model)
        {
            try
            {
                var procName = "PKG_SA_FUNC_CHANELS.ADD_FUNCTION_CHANEL";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pFunctionId", model.FunctionId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("pChanelId", model.ChanelId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _funcchanelssRepositoryHandler.ExecuteProcOracle(procName, dyParam);
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

        public async Task<Response> DeleteByFuncIdAsync(decimal funcId)
        {
            try
            {
                var procName = "PKG_SA_FUNC_CHANELS.DELETE_BY_FUNCID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pFunctionId", funcId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _funcchanelssRepositoryHandler.ExecuteProcOracle(procName, dyParam);
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

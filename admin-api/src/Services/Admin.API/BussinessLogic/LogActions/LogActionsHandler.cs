using System.Threading.Tasks;
using Utils;
using API.Infrastructure.Repositories;
using Admin.API.Infrastructure.Migrations;
using Dapper.Oracle;
using System.Data;
using System;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace API.BussinessLogic
{
    public class LogActionsHandler : ILogActionsHandler
    {
        private readonly RepositoryHandler<SaLogActions, LogActionsModel, LogActionsQueryModel> _logRepositoryHandler = new RepositoryHandler<SaLogActions, LogActionsModel, LogActionsQueryModel>();
        private readonly ILogger<LogActionsHandler> _logger;

        public LogActionsHandler(ILogger<LogActionsHandler> logger = null)
        {            
            _logger = logger;
        }

        public async Task<Response> AddLog(LogActionsModel model)
        {
            try
            {              
                var procName = "PKG_SA_LOG_ACTIONS.ADD_LOG_ACTION";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pFUNCTIONCODE", model.FunctionCode, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pFUNCTIONNAME", model.FunctionName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pACTIONTYPE",  model.ActionType, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pACTIONNAME", model.ActionName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pACTIONTIME", model.ActionTime, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("pACTIONBYUSER", model.ActionByUser, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pOBJECTCONTENTNEW", model.ObjectContentNew, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pOBJECTCONTENTOLD", model.ObjectContentOld, OracleMappingType.Varchar2, ParameterDirection.Input);

                return await _logRepositoryHandler.ExecuteProcOracle(procName, dyParam);

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

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
    public class ChanelsHandler : IChanelsHandler
    {
        private readonly RepositoryHandler<SaChanels, ChanelsBaseModel, ChanelsQueryModel> _chanelsRepositoryHandler = new RepositoryHandler<SaChanels, ChanelsBaseModel, ChanelsQueryModel>();
        private readonly ILogger<ChanelsHandler> _logger;
        public ChanelsHandler(ILogger<ChanelsHandler> logger = null)
        {
            _logger = logger;
        }
        public async Task<Response> GetAllChanels(decimal? funcId = null)
        {
            try
            {
                if (funcId != null)
                {
                    List<PermissionsBaseModel> listResult = new List<PermissionsBaseModel>();
                    var resByFuncId = await GetChanelsByFuncId(funcId) as ResponseObject<List<ChanelsBaseModel>>;
                    var resAll = await GetAllChanels() as ResponseObject<List<ChanelsBaseModel>>;

                    if (resByFuncId != null && resByFuncId.Data.Count > 0 && resAll != null && resAll.Data.Count > 0)
                    {
                        foreach (var item in resAll.Data)
                        {
                            if (resByFuncId.Data.Any(itm => itm.Id == item.Id))
                                item.IsChecked = true;
                        }
                    }

                    return new ResponseObject<List<ChanelsBaseModel>>(resAll.Data, "Thành công", StatusCode.Success);
                }
                else
                {
                    var procName = "PKG_SA_CHANNELS.GET_ALL";
                    var dyParam = new OracleDynamicParameters();
                    dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);

                    return await _chanelsRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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

        public async Task<Response> GetChanelsByFuncId(decimal? funcId)
        {
            try
            {
                var procName = "PKG_SA_CHANNELS.GET_BY_FUNCID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PFUNCID", funcId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _chanelsRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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

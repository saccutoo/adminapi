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
    public class ATM_UserPermisionHandler : IATM_UserPermisionHandler
    {
        private readonly RepositoryHandler<SaATMUserPerms, ATM_UserPermisionBaseModel, ATM_UserPermisionQueryModel> _callDB = new RepositoryHandler<SaATMUserPerms, ATM_UserPermisionBaseModel, ATM_UserPermisionQueryModel>();
        
        private readonly ILogger<UserPermisionHandler> _logger;
        public ATM_UserPermisionHandler(ILogger<UserPermisionHandler> logger = null)
        {
            _logger = logger;
        }
        public async Task<Response> GetATMUserPermsByFillter(ATM_UserPermisionQueryModel model)
        {
            try
            {
                var procName = "PKG_SA_ATM_USER_PERMS.GET_ATM_USER_PERMS_BY_FILTER";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PPAGESIZE", model.PageSize, OracleMappingType.Int32, ParameterDirection.Input);
                dyParam.Add("PPAGEINDEX", model.PageIndex, OracleMappingType.Int32, ParameterDirection.Input);
                dyParam.Add("PIDUSER", model.Id_User, OracleMappingType.Int32, ParameterDirection.Input);
                dyParam.Add("PPOSCD", model.PosCD, OracleMappingType.Varchar2, ParameterDirection.Input);

                return await _callDB.ExecuteProcOracleReturnRow(procName, dyParam,true);
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

        public async Task<Response> GetUserTTDVCNTT()
        {
            try
            {
                var procName = "PKG_SA_ATM_USER_PERMS.GET_USERS_TTDVCNTT";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);

                return await _callDB.ExecuteProcOracleReturnRow(procName, dyParam, true);
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

        public async Task<Response> GetChiNhanh()
        {
            try
            {
                var procName = "PKG_SA_ATM_USER_PERMS.GET_CHI_NHANH";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);

                return await _callDB.ExecuteProcOracleReturnRow(procName, dyParam, true);
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

        public async Task<Response> GetChiNhanhByUserName(string UserName)
        {
            try
            {
                var procName = "PKG_SA_ATM_USER_PERMS.GET_CHI_NHANH_BY_USERNAME";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("pUserName", UserName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);

                return await _callDB.ExecuteProcOracleReturnRow(procName, dyParam, true);
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

        public async Task<Response> GetListPosCDById(int idUser)
        {
            try
            {
                var procName = "PKG_SA_ATM_USER_PERMS.GET_LIST_POSCD_BY_ID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("PIDUSER", idUser, OracleMappingType.Int32, ParameterDirection.Input);
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);

                return await _callDB.ExecuteProcOracleReturnRow(procName, dyParam,false);
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

        public async Task<Response> GrandATMUserPerms(string listPosCD,int idUser )
        {
            try
            {
                var procName = "PKG_SA_ATM_USER_PERMS.GRAND_ATM_USER_PERMS";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pListPosCD", listPosCD, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pIdUser", idUser, OracleMappingType.Int32, ParameterDirection.Input);

                return await _callDB.ExecuteProcOracle(procName, dyParam);
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

        public async Task<Response> GetInfoFromOOS(string Data)
        {
            try
            {
                var procName = "PKG_UTIL_KPI_ACCOUNTANT.GET_INFO_FROM_OOS";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("pData", Data, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);

                return await _callDB.ExecuteProcOracleReturnRow(procName, dyParam, false);
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

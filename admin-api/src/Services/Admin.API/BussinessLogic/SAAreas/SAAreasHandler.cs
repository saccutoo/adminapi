using System.Threading.Tasks;
using Utils;
using API.Infrastructure.Repositories;
using UTILS.API.Infrastructure.Migrations;
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
    public class SAAreasHandler : ISAAreasHandler
    {
        private string strPackages = "PKG_SA_AREAS.";
        private string strWebURL = Helpers.GetConfig("Web:BackEnd");
        private readonly RepositoryHandler<SAAreasBaseModel, SAAreasViewModel, SAAreasQueryModel> _SAAreasRepositoryHandler = new RepositoryHandler<SAAreasBaseModel, SAAreasViewModel, SAAreasQueryModel>();
        private readonly RepositoryHandler<SAAreasBaseModel, SAAreasViewModel, SAAreasQueryModel> _SAAreasVViewRepositoryHandler = new RepositoryHandler<SAAreasBaseModel, SAAreasViewModel, SAAreasQueryModel>();


        private readonly ILogger<SAAreasHandler> _logger;
        public SAAreasHandler(ILogger<SAAreasHandler> logger = null)
        {
            _logger = logger;
        }

        public async Task<Response> GetAllAsync(string Type)
        {
            try
            {
                var procName = strPackages + "GET_ALL";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("PTYPE", Type, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);

                return await _SAAreasRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
                var procName = strPackages + "GET_BY_ID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                return await _SAAreasRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, false);
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

        public async Task<Response> GetByCode(string Code)
        {
            try
            {
                var procName = strPackages + "GET_BY_CODE";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PCODE", Code, OracleMappingType.Varchar2, ParameterDirection.Input);
                return await _SAAreasRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, false);
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

        public async Task<Response> CreateAsync(SAAreasCreateModel model, IDbConnection iCon = null, IDbTransaction iTran = null)
        {
            try
            {
                var procName = strPackages + "INSERT_ROW";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PPARENTID", model.ParentId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PCODE", model.Code, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PNAME", model.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PORDERVIEW", model.OrderView, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PSTATUS", model.Status, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PMAKER", model.CreateBy, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PMAKERONDATE", DateTime.Now, OracleMappingType.Date, ParameterDirection.Input);
                if (iCon == null)
                {
                    return await _SAAreasRepositoryHandler.ExecuteProcOracle(procName, dyParam);
                }
                else
                {
                    return await _SAAreasRepositoryHandler.ExecuteProcOracle(procName, iCon, iTran, dyParam);
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



        public async Task<Response> UpdateAsync(decimal id, SAAreasUpdateModel model, IDbConnection iCon = null, IDbTransaction iTran = null)
        {
            try
            {
                var objQuery = await GetById(id) as ResponseObject<SAAreasViewModel>;

                if (objQuery.Data == null)
                {
                    return new ResponseError(StatusCode.Fail, "Bản ghi không tồn tại");
                }
                else
                {
                    var procNam = strPackages + "UPDATE_ROW";
                    var dyParam = new OracleDynamicParameters();
                    dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam.Add("PID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("PPARENTID", model.ParentId, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("PCODE", model.Code, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("PNAME", model.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("PORDERVIEW", model.OrderView, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("PSTATUS", model.Status, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("PMAKER", model.LastModifiedBy, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("PMAKERONDATE", DateTime.Now, OracleMappingType.Date, ParameterDirection.Input);
                    if (iCon == null)
                    {
                        return await _SAAreasRepositoryHandler.ExecuteProcOracle(procNam, dyParam);
                    }
                    else
                    {
                        return await _SAAreasRepositoryHandler.ExecuteProcOracle(procNam, iCon, iTran, dyParam);
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
        public async Task<Response> ApprovedAsync(SAAreasApprovedModel model, IDbConnection iCon = null, IDbTransaction iTran = null)
        {
            try
            {
                var objQuery = await GetById(model.Id) as ResponseObject<SAAreasViewModel>;

                if (objQuery.Data == null)
                {
                    return new ResponseError(StatusCode.Fail, "Bản ghi không tồn tại");
                }
                else
                {
                    var procNam = strPackages + "APPROVE_ROW";
                    var dyParam = new OracleDynamicParameters();
                    dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam.Add("PID", model.Id, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("PSTATUS", model.Status, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("PAPPROVER", model.ApproverBy, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("PAPPROVERONDATE", DateTime.Now, OracleMappingType.TimeStamp, ParameterDirection.Input);
                    if (iCon == null)
                    {
                        return await _SAAreasRepositoryHandler.ExecuteProcOracle(procNam, dyParam);
                    }
                    else
                    {
                        return await _SAAreasRepositoryHandler.ExecuteProcOracle(procNam, iCon, iTran, dyParam);
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
        public async Task<Response> DeleteAsync(decimal id, string LastModifiedBy, IDbConnection iCon = null, IDbTransaction iTran = null)
        {
            try
            {
                var procName = strPackages + "DELETE_ROW";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PMAKER", LastModifiedBy, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PMAKERONDATE", DateTime.Now, OracleMappingType.TimeStamp, ParameterDirection.Input);
                if (iCon == null)
                {
                    return await _SAAreasRepositoryHandler.ExecuteProcOracle(procName, dyParam);
                }
                else
                {
                    return await _SAAreasRepositoryHandler.ExecuteProcOracle(procName, iCon, iTran, dyParam);
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
        public async Task<Response> DeleteManyAsync(List<decimal> listRoleId, string LastModifiedBy, IDbConnection iCon = null, IDbTransaction iTran = null)
        {
            var listResult = new List<ResponseModel>();
            using (var unitOfWorkOracle = new UnitOfWorkOracle())
            {
                if (listRoleId != null && listRoleId.Count > 0)
                {
                    foreach (var id in listRoleId)
                    {
                        var deleteResult = await DeleteAsync(id, LastModifiedBy, iCon, iTran) as ResponseObject<ResponseModel>;

                        if (deleteResult != null)
                        {
                            listResult.Add(new ResponseModel
                            {
                                Id = deleteResult.Data.Id,
                                Name = deleteResult.Data.Name,
                                Status = deleteResult.Data.Status,
                                Message = (deleteResult.Data.Status.Equals("00") ? "Thanh cong" : "Khong thanh cong")
                            });
                        }
                    }
                    return new ResponseObject<List<ResponseModel>>(listResult, "Thanh cong");
                }
                else
                {
                    return new ResponseObject<List<ResponseModel>>(listResult, "Khong Thanh cong");
                }

            }

        }

        public async Task<Response> GetByFilterAsync(SAAreasQueryModel filterModel)
        {
            try
            {
                var procName = strPackages + "GET_BY_FILTER";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pPageSize", filterModel.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("pPageIndex", filterModel.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("pFullTextSearch", filterModel.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pStatus", (filterModel.Status == "ALL" || string.IsNullOrEmpty(filterModel.Status)) ? null : filterModel.Status, OracleMappingType.Varchar2, ParameterDirection.Input);

                return await _SAAreasRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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

        public async Task<Response> CheckCodeAsync(decimal id, string code)
        {
            try
            {
                var procName = strPackages + "CHECK_BY_CODE";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PCODE", code, OracleMappingType.Varchar2, ParameterDirection.Input);
                return await _SAAreasRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, false);
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

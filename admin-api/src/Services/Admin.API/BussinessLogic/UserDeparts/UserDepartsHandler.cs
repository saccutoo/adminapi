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
//Chuyenlt1 06/03/2019
namespace API.BussinessLogic
{
    public class UserDepartsHandler : IUserDepartsHandler
    {
        private readonly RepositoryHandler<SaUserPerms, UserDepartsBaseModel, UserDepartsQueryModel> _userDepartsRepositoryHandler = new RepositoryHandler<SaUserPerms, UserDepartsBaseModel, UserDepartsQueryModel>();
        private readonly ILogger<UserDepartsHandler> _logger;
        public UserDepartsHandler(ILogger<UserDepartsHandler> logger = null)
        {
            _logger = logger;
        }
        public async Task<Response> CreateAsync(UserDepartsCreateModel model)
        {
            try
            {
                var procName = "PKG_SA_USER_DEPARTS.AddUserInDepartment";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PUSERID", model.Userid, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PDEPARTMENTID", model.Departmentid, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PISHOST", model.IsHost, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _userDepartsRepositoryHandler.ExecuteProcOracle(procName, dyParam);
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

        public async Task<Response> UpdateDepartmentByUserId(UserDepartsBaseModel model)
        {
            try
            {
                var procName = "PKG_SA_USER_DEPARTS.UpdateDepartmentByUserId";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PUSERID", model.Userid, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PDEPARTMENTID", model.Departmentid, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PISHOST", model.IsHost, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _userDepartsRepositoryHandler.ExecuteProcOracle(procName, dyParam);
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

        public async Task<Response> UpdateUserInDepartment(decimal id, UserDepartsUpdateModel model)
        {
    
            try
            {
                var lstUpdateResult = new List<UserDepartsResponseModel>();
                using (var unitOfWorkOracle = new UnitOfWorkOracle())
                {
                    if (model.updateModel != null && model.updateModel.Count > 0)
                    {
                        for (int i = 0; i < model.updateModel.Count; i++)
                        {
                            var procName = "PKG_SA_USER_DEPARTS.UpdateUserInDepartment";
                            var dyParam = new OracleDynamicParameters();
                            dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                            dyParam.Add("pID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                            dyParam.Add("PUSERID", model.updateModel[i].Userid, OracleMappingType.Decimal, ParameterDirection.Input);
                            dyParam.Add("pDEPARTMENTID", model.updateModel[i].Departmentid, OracleMappingType.Decimal, ParameterDirection.Input);
                           dyParam.Add("pIsHost", model.updateModel[i].IsHost, OracleMappingType.Decimal, ParameterDirection.Input);
                            var updateResult = await _userDepartsRepositoryHandler.ExecuteProcOracle(procName, unitOfWorkOracle, dyParam) as ResponseObject<ResponseModel>;

                            if (updateResult != null)
                            {
                                lstUpdateResult.Add(new UserDepartsResponseModel
                                {
                                    Id = updateResult.Data.Id,
                                    Name = updateResult.Data.Name,
                                    Status = updateResult.Data.Status,
                                    Message = (updateResult.Data.Status.Equals("00") ? "Thành công" : "Không thành công")
                                });
                            }
                        }

                        return new ResponseObject<List<UserDepartsResponseModel>>(lstUpdateResult, "Thành công", StatusCode.Success);
                    }
                    else
                    {
                        return new ResponseObject<List<UserDepartsResponseModel>>(lstUpdateResult, "Không thành công", StatusCode.Fail);
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
    }
}

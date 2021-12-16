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

namespace API.BussinessLogic
{
    public class UserBiometricHandler : IUserBiometricHandler
    {
        private readonly RepositoryHandler<SaUsersBiometric, UserBiometricModel, UserBiometricQueryModel> _userBiometricRepositoryHandler = new RepositoryHandler<SaUsersBiometric, UserBiometricModel, UserBiometricQueryModel>();
        private readonly UserHandler _userHandler = new UserHandler();
        private readonly ILogger<UserBiometricHandler> _logger;
        public UserBiometricHandler(ILogger<UserBiometricHandler> logger = null)
        {
            _logger = logger;
        }

        public async Task<Response> SetBiometric(UserBiometricCreateModel model)
        {
            try
            {
                // Kiem tra username co ton tai trong he thong
                var getUser = await _userHandler.GetByUserName(model.UserName) as ResponseObject<UserViewModel>;
                if (getUser.StatusCode == StatusCode.Fail || (getUser.StatusCode == StatusCode.Success && getUser.Data == null)) return new ResponseError(StatusCode.Fail, "Tên đăng nhập không hợp lệ");

                var procName = "PKG_SA_BIOMETRIC.SET_BIOMETRIC";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pUSERNAME", model.UserName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pDEVICE_ID", model.DeviceId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pDEVICE_INFO", model.DeviceInfo, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pIP", model.Ip, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pSTATUS", model.Status, OracleMappingType.Varchar2, ParameterDirection.Input);

                var setBiometricResult = await _userBiometricRepositoryHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;
                if (setBiometricResult.StatusCode == StatusCode.Success && model.Status == "REGISTER")
                    return new ResponseObject<UserBiometricBaseModel>(new UserBiometricBaseModel
                    {
                        BiometricToken = setBiometricResult.Data.Name
                    }, string.Empty, StatusCode.Success);
                else return setBiometricResult;

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
        public async Task<Response> LoginBiometric(UserBiometricBaseModel baseModel)
        {
            try
            {               
                var procName = "PKG_SA_BIOMETRIC.LOGIN_BIOMETRIC";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pUSERNAME", baseModel.UserName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pDEVICE_ID", baseModel.DeviceId, OracleMappingType.Varchar2, ParameterDirection.Input);               
                dyParam.Add("pIP", baseModel.Ip, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pTOKEN_BIOMETRIC", baseModel.BiometricToken, OracleMappingType.Varchar2, ParameterDirection.Input);

                return await _userBiometricRepositoryHandler.ExecuteProcOracle(procName, dyParam);               
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

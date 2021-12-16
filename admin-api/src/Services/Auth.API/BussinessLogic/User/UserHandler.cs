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

namespace API.BussinessLogic
{
    public class UserHandler : IUserHandler
    {
        private readonly RepositoryHandler<SaUsers, UserViewModel, UserQueryModel> _userRepositoryHandler = new RepositoryHandler<SaUsers, UserViewModel, UserQueryModel>();

        public async Task<Response> UpdateLoginAsync(decimal id, UserLoginModel model)
        {
            try
            {
                var existUser = await FindAsync(id) as ResponseObject<UserViewModel>;

                if (existUser == null)
                {
                    return new ResponseError(StatusCode.Fail, "Không thành công");
                }

                var procName = "PKG_SA_USERS.UPDATE_USER";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PDEPARTMENTID", existUser.Data.DepartmentId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PLASTNAME", existUser.Data.LastName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PFIRSTNAME", existUser.Data.FirstName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PUSERNAME", existUser.Data.UserName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PPASSWORD", existUser.Data.Password, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PPASSWORDSALT", existUser.Data.PasswordSalt, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PGENDER", existUser.Data.Gender, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PPHONE", existUser.Data.Phone, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PBIRTHDAY", existUser.Data.Birthday, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("PADDRESS", existUser.Data.Address, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PEMAIL", existUser.Data.Email, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PSTATUS", !string.IsNullOrEmpty(model.Status) ? model.Status : existUser.Data.Status.ToString(), OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PLASTLOGINDATE", model.LastLoginDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("PLASTLOGOUTDATE", model.LastLogoutDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("PFALLEDPASSCOUNT", model.FailedPassCount, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PLOCKTIME", model.LockTime, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("PPOSTINGCODE", existUser.Data.PostingCode, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                var res = await _userRepositoryHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

                if (res != null && res.Data.Status.Equals("00"))
                {
                    return new ResponseObject<ResponseModel>(res.Data, "Thành công", StatusCode.Success);
                }
                else
                {
                    return new ResponseError(StatusCode.Fail, "Không thành công");
                }

            }
            catch (Exception ex)
            {
                return new ResponseError(StatusCode.Fail, ex.Message);
            }
        }
        public async Task<Response> FindAsync(decimal id)
        {
            try
            {
                var procName = "PKG_SA_USERS.GET_BY_ID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _userRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, false);
            }
            catch (Exception ex)
            {
                return new ResponseError(StatusCode.Fail, ex.Message);
            }
        }
        public async Task<Response> GetByUserName(string userName)
        {
            try
            {
                var procName = "PKG_SA_USERS.GET_BY_USERNAME";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PUSERNAME", userName, OracleMappingType.Varchar2, ParameterDirection.Input);

                return await _userRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, false);
            }
            catch (Exception ex)
            {
                return new ResponseError(StatusCode.Fail, ex.Message);
            }
        }     
    }
}

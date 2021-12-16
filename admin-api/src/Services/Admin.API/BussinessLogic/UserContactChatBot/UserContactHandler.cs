using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;
using Dapper;
using Dapper.Oracle;
using API.Infrastructure.Repositories;
using System.Data;
using Microsoft.Extensions.Logging;

namespace API.BussinessLogic
{
    public class UserContactHandler : IUserContactHandler
    {

        private readonly RepositoryHandler<UserContactCreatingModel, UserContactResponseModel, UserContactQueryModel> _userContactRepositoryHandler = new RepositoryHandler<UserContactCreatingModel, UserContactResponseModel, UserContactQueryModel>();
        private readonly ILogger<UserContactHandler> _logger;
        public UserContactHandler(ILogger<UserContactHandler> logger = null)
        {
            _logger = logger;
        }
        public async Task<Response> GetContactByNameAndDepartment(string userName, string deptName)
        {
            try
            {
                var procName = "AI_CHATBOT_SHB.GET_CONTACT";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_CURSOR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_NAME", userName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_CENTER", deptName, OracleMappingType.Varchar2, ParameterDirection.Input);


                return await _userContactRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
            

            //if (res.Data.STATUS_CODE.Equals("00"))
            //{
            //    return new ResponseObject<UserContactResponseModel>(ReturnResponse(res.Data), "Thành công", StatusCode.Success);
            //}
            //else
            //{
            //    return new ResponseObject<UserContactResponseModel>(ReturnResponse(res.Data), "Không thành công", StatusCode.Fail);
            //}
        }

        public UserContactResponseModel ReturnResponse(ResponseOracle model)
        {
            UserContactResponseModel res = new UserContactResponseModel();
            /*res.Id = model.ID;
            res.Name = model.NAME;
            res.Status = model.STATUS_CODE;
            res.Message = model.STATUS_CODE.Equals("00") ? "Thành công" : "Không thành công";*/
            return res;
        }

    }
}

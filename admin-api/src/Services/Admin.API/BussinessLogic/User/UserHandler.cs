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
    public class UserHandler : IUserHandler
    {
        private readonly RepositoryHandler<SaUsers, UserViewModel, UserQueryModel> _userRepositoryHandler
            = new RepositoryHandler<SaUsers, UserViewModel, UserQueryModel>();
        private readonly RepositoryHandler<PhongBanIasModel, PhongBanIasModel, UserQueryModel> _phongBanIasHandler
            = new RepositoryHandler<PhongBanIasModel, PhongBanIasModel, UserQueryModel>();
        private readonly RepositoryHandler<PosPhamViModel, PosPhamViModel, UserQueryModel> _posPhamViHandler
           = new RepositoryHandler<PosPhamViModel, PosPhamViModel, UserQueryModel>();
        private UserRolesHandler _userRolesHandler = new UserRolesHandler();
        private UserDepartsHandler _userDepartHandler = new UserDepartsHandler();
        private ApplicationHandler _applicationHandler = new ApplicationHandler();
        private UserPermisionHandler _userpermsHandler = new UserPermisionHandler();
        private LogActionsHandler _logActionHandler;
        private readonly ILogger<UserHandler> _logger;
        private readonly string _avatarDefault;
        public UserHandler(ILogger<UserHandler> logger = null)
        {
            _logger = logger;
            _avatarDefault = Helpers.GetConfig("StaticFiles:AvatarDefault");
        }

        public async Task<Response> CreateAsync(UserCreateModel model, BaseModel baseModel)
        {
            try
            {
                var passwordDecrype = EncryptedString.DecryptString(model.Password, Helpers.GetConfig("Encrypt:Key"));
                var salt = Helpers.PasswordCreateSalt512();
                var passEncrypt = Helpers.PasswordGenerateHmac(passwordDecrype, salt);

                var procName = "PKG_SA_USERS.ADD_USER";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PDEPARTMENTID", model.DepartmentId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PLASTNAME", model.LastName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PFIRSTNAME", model.FirstName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PUSERNAME", model.UserName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PPASSWORD", passEncrypt, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PPASSWORDSALT", salt, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PGENDER", model.Gender, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PPHONE", model.Phone, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PBIRTHDAY", model.Birthday, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("PADDRESS", model.Address, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PEMAIL", model.Email, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PSTATUS", model.Status, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PMAKER", baseModel.Maker, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PMAKERONDATE", DateTime.Now, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("PIP", model.Ip, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PAGENT", model.Agent, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PPOSTINGCODE", model.PostingCode, OracleMappingType.Varchar2, ParameterDirection.Input);


                var res = await _userRepositoryHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

                if (res != null && res.Data.Status.Equals("00"))
                {
                    var id_new = res.Data.Id;
                    UserDepartsCreateModel userDepartCreateModel = new UserDepartsCreateModel()
                    {
                        Userid = id_new,
                        Departmentid = model.DepartmentId,
                        IsHost = 1
                    };

                    var insertUserDepart = await _userDepartHandler.CreateAsync(userDepartCreateModel) as ResponseObject<UserDepartsResponseModel>;

                    if (insertUserDepart == null || !insertUserDepart.Data.Status.Equals("00"))
                        return new ResponseError(StatusCode.Fail, "Không thành công");

                    if (model.RolesId != null)
                    {
                        foreach (decimal id in model.RolesId)
                        {
                            var resInsert = await InsertRolesAndApplication(id_new, id);
                            if (!resInsert)
                            {
                                return new ResponseError(StatusCode.Fail, "Không thành công");
                            }
                        }
                    }

                    return new ResponseObject<ResponseModel>(res.Data, "Thành công", StatusCode.Success);
                }

                return new ResponseError(StatusCode.Fail, "Không thành công");
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
        public async Task<Response> UpdateAsync(decimal id, UserUpdateModel model)
        {
            try
            {
                var existUser = await FindAsync(id) as ResponseObject<UserViewModel>;

                var salt = "";
                var passEncrypt = "";

                if (existUser == null)
                {
                    return new ResponseError(StatusCode.Fail, "Không thành công");
                }

                if (!string.IsNullOrEmpty(model.Password))
                {
                    var passwordDecrype = EncryptedString.DecryptString(model.Password, Helpers.GetConfig("Encrypt:Key"));
                    salt = Helpers.PasswordCreateSalt512();
                    passEncrypt = Helpers.PasswordGenerateHmac(passwordDecrype, salt);
                }

                var procName = "PKG_SA_USERS.UPDATE_USER";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PDEPARTMENTID", model.DepartmentId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PLASTNAME", model.LastName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PFIRSTNAME", model.FirstName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PUSERNAME", existUser.Data.UserName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PPASSWORD", (!string.IsNullOrEmpty(model.Password)) ? passEncrypt : existUser.Data.Password, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PPASSWORDSALT", (!string.IsNullOrEmpty(model.Password)) ? salt : existUser.Data.PasswordSalt, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PGENDER", model.Gender, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PPHONE", model.Phone, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PBIRTHDAY", model.Birthday, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("PADDRESS", model.Address, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PEMAIL", model.Email, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PSTATUS", model.Status, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PLASTLOGINDATE", existUser.Data.LastLoginDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("PLASTLOGOUTDATE", existUser.Data.LastLogoutDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("PFALLEDPASSCOUNT", existUser.Data.FailedPassCount, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PLOCKTIME", existUser.Data.LockTime, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("PPOSTINGCODE", model.PostingCode, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                var res = await _userRepositoryHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

                if (res != null && res.Data.Status.Equals("00"))
                {
                    UserDepartsBaseModel userDepartModel = new UserDepartsBaseModel()
                    {
                        Userid = id,
                        Departmentid = model.DepartmentId,
                        IsHost = 1
                    };

                    var resUserDepart = await _userDepartHandler.UpdateDepartmentByUserId(userDepartModel) as ResponseObject<UserDepartsResponseModel>;

                    if (resUserDepart == null || !resUserDepart.Data.Status.Equals("00"))
                        return new ResponseError(StatusCode.Fail, "Không thành công");


                    if (model.RolesId != null)
                    {
                        var resDelete = await DeleteRolesAndApplication(id);
                        if (resDelete)
                        {
                            foreach (decimal item in model.RolesId)
                            {
                                var resInsert = await InsertRolesAndApplication(id, item);
                                if (!resInsert)
                                {
                                    return new ResponseError(StatusCode.Fail, "Không thành công");
                                }
                            }
                        }
                    }
                    return new ResponseObject<ResponseModel>(res.Data, "Thành công", StatusCode.Success);
                }
                else
                {
                    return new ResponseError(StatusCode.Fail, "Không thành công");
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

        public async Task<Response> UpdateFcmTokenAsync(UserUpdateFCMModel model)
        {
            try
            {
                var existUser = await FindAsync(model.UserId) as ResponseObject<UserViewModel>;

                if (existUser == null)
                {
                    return new ResponseError(StatusCode.Fail, "Không thành công");
                }
                
                var procName = "PKG_SA_USERS.UPDATE_USER_TOKEN";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pUSERID", model.UserId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("pFCMTOKEN", model.FcmToken, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pDEVICEID", model.DeviceId, OracleMappingType.Varchar2, ParameterDirection.Input);

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
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ");
                }
                else throw ex;
            }
        }

        public async Task<Response> UpdateUserGroupAsync(decimal id, UserUserGroupUpdateModel model)
        {
            try
            {
                _logActionHandler = new LogActionsHandler();
                using (var unitOfWorkOracle = new UnitOfWorkOracle())
                {
                    var iConn = unitOfWorkOracle.GetConnection();
                    var iTrans = iConn.BeginTransaction();

                    //1. Cập nhật thông tin user
                    var existUser = await FindAsync(id) as ResponseObject<UserViewModel>;
                    if (existUser == null) return new ResponseError(StatusCode.Fail, "Không thành công");

                    var procName = "PKG_SA_USERS.UPDATE_USER_SHORTCUT";
                    var dyParam = new OracleDynamicParameters();
                    dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam.Add("P_USERID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("P_BM2", model.MaPhongBanIas, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_BM3", model.PhongBanIas, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_BM4", model.PhamViDuLieu, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_BM5", model.MaPos, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_BM6", model.TenPos, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_BM7", model.TrangThaiNguoiDung, OracleMappingType.Varchar2, ParameterDirection.Input);


                    var updateUserRes = await _userRepositoryHandler.ExecuteProcOracle(procName, iConn, iTrans, dyParam) as ResponseObject<ResponseModel>;

                    if (updateUserRes != null && updateUserRes.StatusCode == StatusCode.Success)
                    {
                        //2. Xóa toàn bộ phân quyền cũ: nhóm người dùng, nhóm quyền, quyền kế thừa ( không bao gồm quyền mặc định )
                        var procName1 = "PKG_SA_USER_GROUP.DROP_USERPER_BY_USERID";
                        var dyParam1 = new OracleDynamicParameters();
                        dyParam1.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                        dyParam1.Add("P_USERID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                        var dropUserPerRes = await _userRepositoryHandler.ExecuteProcOracle(procName1, iConn, iTrans, dyParam1) as ResponseObject<ResponseModel>;
                        if (dropUserPerRes != null && dropUserPerRes.StatusCode == StatusCode.Success)
                        {
                            //3. Thêm mới lại toàn bộ thông tin phân quyền: nhóm người dùng, nhóm quyền thuộc nhóm ng dùng, quyền kế thừa nhóm quyền
                            if (model.ListUserGroup != null && model.ListUserGroup.Count > 0)
                            {
                                foreach (var userGroupId in model.ListUserGroup)
                                {
                                    try
                                    {
                                        var procName2 = "PKG_SA_USER_GROUP.CREATE_USER_USERGROUP";
                                        var dyParam2 = new OracleDynamicParameters();
                                        dyParam2.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                                        dyParam2.Add("P_USERGROUPID", userGroupId, OracleMappingType.Decimal, ParameterDirection.Input);
                                        dyParam2.Add("P_USERID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                                        var createUserUserGroup = await _userRepositoryHandler.ExecuteProcOracle(procName2, iConn, iTrans, dyParam2) as ResponseObject<ResponseModel>;
                                        if (createUserUserGroup == null || createUserUserGroup.StatusCode == StatusCode.Fail || createUserUserGroup.Data == null)
                                        {
                                            iTrans.Rollback();
                                            return new ResponseError(StatusCode.Fail, createUserUserGroup.Message);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        iTrans.Rollback();
                                        if (_logger != null) _logger.LogError(ex, "Exception Error");
                                        return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ");
                                    }
                                }
                            }
                        }
                        else
                        {
                            iTrans.Rollback();
                            return dropUserPerRes;
                        }
                    }
                    iTrans.Commit();

                    // Log Action
                    var logActionsModel = new LogActionsModel
                    {
                        FunctionCode = "UPDATE_USER",
                        FunctionName = "Cập nhật người dùng",
                        ActionByUser = model.ModifiedBy,
                        ActionName = "Cập nhật",
                        ActionType = "UPDATE",
                        ActionTime = DateTime.Now,
                        ObjectContentNew = JsonConvert.SerializeObject(model)
                    };

                    await _logActionHandler.AddLog(logActionsModel);

                    return updateUserRes;
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
                var procName = "PKG_SA_USERS.DELETE_USER";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PUSERID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _userRepositoryHandler.ExecuteProcOracle(procName, dyParam);
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
        public async Task<Response> DeleteManyAsync(UserDeleteModel model)
        {
            try
            {
                var lstDeleteResult = new List<ResponseOracleModel>();
                using (var unitOfWorkOracle = new UnitOfWorkOracle())
                {
                    if (model.ListId != null && model.ListId.Count > 0)
                    {
                        foreach (var id in model.ListId)
                        {
                            var procName = "PKG_SA_USERS.DELETE_USER";
                            var dyParam = new OracleDynamicParameters();
                            dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                            dyParam.Add("PUSERID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                            var deleteResult = await _userRepositoryHandler.ExecuteProcOracle(procName, unitOfWorkOracle, dyParam) as ResponseObject<ResponseModel>;

                            if (deleteResult != null)
                            {
                                lstDeleteResult.Add(new ResponseOracleModel
                                {
                                    Id = deleteResult.Data.Id,
                                    Name = deleteResult.Data.Name,
                                    Status = deleteResult.Data.Status,
                                    Message = (deleteResult.Data.Status.Equals("00") ? "Thành công" : "Không thành công")
                                });
                            }
                        }

                        return new ResponseObject<List<ResponseOracleModel>>(lstDeleteResult, "Thành công", StatusCode.Success);
                    }
                    else
                    {
                        return new ResponseObject<List<ResponseOracleModel>>(lstDeleteResult, "Không thành công", StatusCode.Fail);
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
        public async Task<Response> GetUserByBirthdayMonnthAsync(int month)
        {
            try
            {
                var procName = "PKG_SA_USERS.GET_BIRTHDAY_MONTH";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PMONTH", month, OracleMappingType.Decimal, ParameterDirection.Input);

                var userBirthDays = await _userRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true) as ResponseObject<List<UserViewModel>>;
                if (userBirthDays?.Data?.Count > 0)
                {
                    var rootPath = Helpers.GetConfig("StaticFiles:RootPath");
                    var defaultAvt = Helpers.GetConfig("StaticFiles:AvatarDefault");
                    foreach (var item in userBirthDays.Data)
                    {
                        item.Password = "";
                        item.PasswordSalt = "";
                        item.Avatar = string.IsNullOrEmpty(item.Avatar) ? (rootPath + defaultAvt) : (rootPath + item.Avatar);
                    }
                }
                return userBirthDays;
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

        public async Task<Response> GetUserByBirthdayDayAsync(int day, int month)
        {
            try
            {
                var procName = "PKG_SA_USERS.GET_BIRTHDAY_MONTH_DAY";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PDAY", day, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PMONTH", month, OracleMappingType.Decimal, ParameterDirection.Input);
                
                var userBirthDays = await _userRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true) as ResponseObject<List<UserViewModel>>;
                if(userBirthDays?.Data?.Count > 0)
                {
                    var rootPath = Helpers.GetConfig("StaticFiles:RootPath");
                    var defaultAvt = Helpers.GetConfig("StaticFiles:AvatarDefault");
                    foreach (var item in userBirthDays.Data)
                    {
                        item.Password = "";
                        item.PasswordSalt = "";
                        item.Avatar = string.IsNullOrEmpty(item.Avatar) ? (rootPath + defaultAvt) : (rootPath + item.Avatar);
                    }
                }
                return userBirthDays;
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
                var procName = "PKG_SA_USERS.GET_BY_ID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                var existUser = await _userRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, false) as ResponseObject<UserViewModel>;

                if (existUser.StatusCode == StatusCode.Success)
                {
                    if (string.IsNullOrEmpty(existUser.Data.Avatar)) existUser.Data.Avatar = _avatarDefault;
                    return new ResponseObject<UserViewModel>(existUser.Data, "Thành công", StatusCode.Success);
                }
                else return new ResponseObject<UserViewModel>(null, "Không thành công", StatusCode.Fail);
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
        // Lay ban ghi theo Ma nhan su
        public async Task<Response> GetByEmpIDAsync(string Emp_ID)
        {
            try
            {
                var procName = "PKG_SA_USERS.GET_BY_EMP_ID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PEMP_ID", Emp_ID, OracleMappingType.Varchar2, ParameterDirection.Input);
                var existUser = await _userRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, false) as ResponseObject<UserViewModel>;
                if (existUser.StatusCode == StatusCode.Success)
                {
                    if (string.IsNullOrEmpty(existUser.Data.Avatar)) existUser.Data.Avatar = _avatarDefault;
                    return new ResponseObject<UserViewModel>(existUser.Data, "Thành công", StatusCode.Success);
                }
                else return new ResponseObject<UserViewModel>(null, "Không thành công", StatusCode.Fail);
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

        public async Task<Response> GetByEmailAsync(string Email)
        {
            try
            {
                var procName = "PKG_SA_USERS.GET_BY_EMAIL";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PEMAIL", Email, OracleMappingType.Varchar2, ParameterDirection.Input);
                var existUser = await _userRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, false) as ResponseObject<UserViewModel>;
                if (existUser.StatusCode == StatusCode.Success)
                {
                    if (string.IsNullOrEmpty(existUser.Data.Avatar)) existUser.Data.Avatar = _avatarDefault;
                    return new ResponseObject<UserViewModel>(existUser.Data, "Thành công", StatusCode.Success);
                }
                else return new ResponseObject<UserViewModel>(null, "Không thành công", StatusCode.Fail);
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
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ");
                }
                else throw ex;
            }
        }
        public async Task<Response> GetAllPhongBanIas()
        {
            try
            {
                var procName = "PKG_SA_USERS.GET_ALL_PHONGBAN_IAS";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);

                return await _phongBanIasHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
        public async Task<Response> GetPosTheoPhamVi(string maPhamVi)
        {
            try
            {
                var procName = "PKG_SA_USERS.GET_ALL_POS";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_DATA_RANGE", maPhamVi, OracleMappingType.Varchar2, ParameterDirection.Input);

                return await _posPhamViHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
        public async Task<Response> GetUserByFilterAsync(UserQueryModel model)
        {
            try
            {
                var procName = "PKG_SA_USERS.GET_USER_BY_FILTER";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PPAGESIZE", model.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PPAGEINDEX", model.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PFULLTEXTSEARCH", !string.IsNullOrEmpty(model.FullTextSearch) ? model.FullTextSearch : null, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PDEPARTMENTID", (!model.DepartmentId.HasValue || model.DepartmentId == -1) ? null : model.DepartmentId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PSTATUS", (model.Status == "ALL" || string.IsNullOrEmpty(model.Status)) ? null : model.Status, OracleMappingType.Varchar2, ParameterDirection.Input);

                return await _userRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
        public async Task<Response> GetStaffByManagerAsync(string userName)
        {
            try
            {
                var procName = "PKG_SA_USERS.GET_STAFF_BY_MANAGER_NAME";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pUserName", userName, OracleMappingType.Varchar2, ParameterDirection.Input);

                return await _userRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
                var procName = "PKG_SA_USERS.GET_ALL";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);

                return await _userRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
        public ResponseOracleModel ReturnResponse(ResponseOracle model)
        {
            ResponseOracleModel res = new ResponseOracleModel();
            res.Id = model.ID;
            res.Name = model.NAME;
            res.Status = model.STATUS_CODE;
            res.Message = model.STATUS_CODE.Equals("00") ? "Thành công" : "Không thành công";
            return res;
        }
        public async Task<bool> InsertRolesAndApplication(decimal userId, decimal id)
        {
            try
            {
                var resApplication = await _applicationHandler.GetApplicationByRolesIdAsync(id) as ResponseObject<List<ApplicationBaseModel>>;

                if (resApplication != null)
                {
                    foreach (var item in resApplication.Data)
                    {
                        UserPermisionCreateModel userPermsModel = new UserPermisionCreateModel()
                        {
                            Userid = userId,
                            Applicationid = item.Id,
                            Funcpermid = item.FuncPermsId
                        };
                        var resUserPerms = await _userpermsHandler.CreateAsync(userPermsModel) as ResponseObject<UserPermisionResponseModel>;

                        if (resUserPerms == null || !resUserPerms.Data.Status.Equals("00")) return false;
                    }
                }

                UserRolesCreateModel userrolesModel = new UserRolesCreateModel()
                {
                    UserId = userId,
                    RoleId = id
                };

                var resUserRoles = await _userRolesHandler.CreateAsync(userrolesModel) as ResponseObject<UserRolesResponseModel>;

                if (resUserRoles == null || !resUserRoles.Data.Status.Equals("00")) return false;

                return true;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return false;
                }
                else throw ex;
            }
        }
        public async Task<bool> DeleteRolesAndApplication(decimal userId)
        {
            try
            {
                var resUserperms = await _userpermsHandler.DeleteUserPermsOfUser(userId) as ResponseObject<ResponseModel>;

                if (resUserperms == null || !resUserperms.Data.Status.Equals("00"))
                    return false;

                var resUserroles = await _userRolesHandler.DeleteRolesByUserId(userId) as ResponseObject<ResponseModel>;

                if (resUserroles == null || !resUserroles.Data.Status.Equals("00"))
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return false;
                }
                else throw ex;
            }
        }
    }
}

using System.Threading.Tasks;
using Utils;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Dapper.Oracle;
using System.Data;
using Admin.API.Infrastructure.Migrations;
using API.Infrastructure.Repositories;

namespace API.BussinessLogic
{
    public class AuthHandler : IAuthHandler
    {
        private readonly RepositoryHandler<SaKeyManager, KeyManagerModel, KeyManagerQueryModel> _keyManagerHandler = new RepositoryHandler<SaKeyManager, KeyManagerModel, KeyManagerQueryModel>();
        private UserHandler _userHandler;
        private UserBiometricHandler _userBiometricHandler;
        private FuncPermsHandler _funcpermsHandler;
        private readonly string _baseToken;
        private string _apiGatewayRootUrl;
        private string _clientId;
        private string _clientSecret;
        private readonly decimal? _maxFailedCount;
        private readonly decimal? _lockTimeFailed;
        private readonly decimal? _visibleCaptchaCount;
        private readonly decimal? _sessionTime;
        private readonly decimal? _isAuthenLdap;
        private readonly string _adPath;
        private readonly string _adDomain;
        private readonly string _avatarDefault;
        private readonly ILogger<AuthHandler> _logger;

        public AuthHandler(ILogger<AuthHandler> logger)
        {
            _maxFailedCount = decimal.Parse(Helpers.GetConfig("LoginConfig:MaxFailedCount"));
            _lockTimeFailed = decimal.Parse(Helpers.GetConfig("LoginConfig:LockTimeFailed"));
            _visibleCaptchaCount = decimal.Parse(Helpers.GetConfig("LoginConfig:VisibleCaptchaCount"));
            _sessionTime = decimal.Parse(Helpers.GetConfig("LoginConfig:SessionTime"));
            _baseToken = Helpers.GetConfig("Authenticate:BaseToken");
            _isAuthenLdap = decimal.Parse(Helpers.GetConfig("LDAP:IsAuthenLdap"));
            _adPath = Helpers.GetConfig("LDAP:ADHost");
            _adDomain = Helpers.GetConfig("LDAP:ADDomain");
            _avatarDefault = Helpers.GetConfig("StaticFiles:AvatarDefault");
            _logger = logger;
        }
        public async Task<Response> Authenticate(string userName, string passWord, string zone, bool? isGetToken = true)
        {
            _funcpermsHandler = new FuncPermsHandler();
            try
            {
                if (!(string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passWord)))
                {
                    var passwordPlaintext = EncryptedString.DecryptString(passWord, Helpers.GetConfig("Encrypt:Key"));
                    
                    // Kiểm tra thông tin tên đăng nhập
                    _userHandler = new UserHandler();
                    UserLoginModel userLoginUpdate;

                    var existUser = await _userHandler.GetByUserName(userName) as ResponseObject<UserViewModel>;
                    if (existUser.Data != null)
                    {
                        var isValid = false;
                        // Kiểm tra mật khẩu
                        var userSalt = existUser.Data.PasswordSalt;
                        var userPass = existUser.Data.Password;
                        string inputHmac = string.Empty;
                        if (!string.IsNullOrEmpty(userSalt)) inputHmac = Helpers.PasswordGenerateHmac(passwordPlaintext, userSalt);

                        decimal? coutLoginFall = existUser.Data.FailedPassCount + 1;

                        decimal totalminutes = existUser.Data.LockTime.HasValue ? (decimal)(DateTime.Now - existUser.Data.LockTime.Value).TotalMinutes : _lockTimeFailed.Value + 1;

                        if (existUser.Data.Status.Equals("LOCKED") && totalminutes <= _lockTimeFailed)
                        {
                            return new ResponseObject<UserLoginModel>(null, "Tài khoản hiện đang bị khóa", StatusCode.Fail);
                        }

                        // Kiểm tra mk trong DB
                        if (_isAuthenLdap.Value != 1 || existUser.Data.Id <= 0) isValid = string.Equals(userPass, inputHmac);
                        // Kiểm tra mk trong AD 
                        //TODO: Tam fix cho user=admin
                        else if (_isAuthenLdap.Value == 1 && existUser.Data.Id > 0)
                        {
                            var checkAD = CheckUserAD(existUser.Data.UserName, passwordPlaintext);
                            isValid = checkAD.Data;
                        }

                        if (isValid)
                        {
                            userLoginUpdate = new UserLoginModel();

                            var data = existUser?.Data;

                            if (isGetToken.HasValue && isGetToken.Value)
                            {
                                var getAccessTokenResult = GetAccessToken(zone).Result;
                                if (getAccessTokenResult != null)
                                {
                                    userLoginUpdate.AccessToken = getAccessTokenResult.access_token;
                                    userLoginUpdate.ExpiresIn = getAccessTokenResult.expires_in;
                                }
                            }

                            userLoginUpdate.Id = existUser.Data.Id;
                            userLoginUpdate.UserName = existUser.Data.UserName;
                            userLoginUpdate.Status = "ACTIVE";
                            userLoginUpdate.LastLoginDate = DateTime.Now;
                            userLoginUpdate.LastLogoutDate = existUser.Data.LastLogoutDate;
                            userLoginUpdate.FailedPassCount = 0;
                            userLoginUpdate.LockTime = null;
                            userLoginUpdate.IsVisibleCaptcha = false;
                            userLoginUpdate.FullName = existUser.Data.FullName;
                            userLoginUpdate.Phone = existUser.Data.Phone;
                            userLoginUpdate.Email = existUser.Data.Email;
                            userLoginUpdate.Address = existUser.Data.Address;
                            userLoginUpdate.Title = existUser.Data.Title;
                            userLoginUpdate.SessionTime = _sessionTime ?? 20;
                            userLoginUpdate.PosCD = existUser.Data.PosCD;
                            userLoginUpdate.DeptId = existUser.Data.DeptId;
                            if (string.IsNullOrEmpty(existUser.Data.Avatar)) userLoginUpdate.Avatar = _avatarDefault;
                            else userLoginUpdate.Avatar = existUser.Data.Avatar;

                            List<FuncLoginModel> funPerm = new List<FuncLoginModel>();
                            if (await _funcpermsHandler.GetFuncPermsByUserId(existUser.Data.Id) is ResponseObject<List<FuncPermsBaseModel>> getFuncPerms && getFuncPerms.Data.Count > 0)
                            {
                                foreach (var item in getFuncPerms.Data)
                                {
                                    FuncLoginModel i = new FuncLoginModel();
                                    if (item.Code != null)
                                    {
                                        i.FuncPermCode = item.Code;
                                        i.FunctionId = item.FunctionId;
                                        funPerm.Add(i);
                                    }
                                }

                                userLoginUpdate.ListFuncPerms = funPerm;
                            }

                            // Tao token chua thong tin quyen han cua nguoi dung: ten dang nhap, danh sach quyen han,
                            // thoi gian session,...
                            var PermissionTokenId = Guid.NewGuid();
                            var permissionTokenModel = new PermissionTokenModel
                            {
                                Id = PermissionTokenId,
                                UserName = userLoginUpdate.UserName,
                                UserId = userLoginUpdate.Id,
                                ExpiredAfter = userLoginUpdate.SessionTime,
                                ListFuncPerms = userLoginUpdate.ListFuncPerms
                            };
                            userLoginUpdate.PermissionToken = GeneratePermissionToken(permissionTokenModel);
                            userLoginUpdate.PermissionTokenId = PermissionTokenId;
                            // Thêm thông tin token vào bảng key manager để quản lý phiên 
                            var procName = "PKG_SA_KEY_MANAGER.ADD_KEY";
                            var dyParam = new OracleDynamicParameters();
                            dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                            dyParam.Add("pUSERNAME", existUser.Data.UserName, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam.Add("pIDPERMISSIONTOKEN", permissionTokenModel.Id, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam.Add("pBEARERTOKEN", userLoginUpdate.AccessToken, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam.Add("pCREATEDONDATE", DateTime.Now, OracleMappingType.Date, ParameterDirection.Input);
                            var addKeyManager = await _keyManagerHandler.ExecuteProcOracle(procName, dyParam);

                            var updateUserSuccess = await _userHandler.UpdateLoginAsync(data.Id, userLoginUpdate);
                            return new ResponseObject<UserLoginModel>(userLoginUpdate, "Đăng nhập thành công");
                        }

                        else
                        {
                            string mess = "Tên đăng nhập hoặc mật khẩu không đúng";

                            userLoginUpdate = new UserLoginModel
                            {
                                Id = existUser.Data.Id,
                                LastLoginDate = existUser.Data.LastLoginDate,
                                LastLogoutDate = existUser.Data.LastLogoutDate,
                                FailedPassCount = coutLoginFall,
                                Status = "ACTIVE"
                            };

                            if (coutLoginFall >= _visibleCaptchaCount)
                            {
                                userLoginUpdate.IsVisibleCaptcha = true;
                            }
                            else
                            {
                                userLoginUpdate.IsVisibleCaptcha = false;
                            }

                            if (coutLoginFall >= _maxFailedCount)
                            {
                                userLoginUpdate.LockTime = DateTime.Now;
                                userLoginUpdate.Status = "LOCKED";
                                userLoginUpdate.FailedPassCount = 0;
                                userLoginUpdate.IsVisibleCaptcha = false;
                                mess = "Tài khoản hiện đang bị khóa";
                            }
                            else
                            {
                                userLoginUpdate.LockTime = existUser.Data.LockTime;
                            }

                            var updateUserFail = await _userHandler.UpdateLoginAsync(existUser.Data.Id, userLoginUpdate);

                            return new ResponseObject<UserLoginModel>(userLoginUpdate, mess, StatusCode.Fail);
                        }
                    }
                    else return new ResponseObject<UserLoginModel>(null, "Tên đăng nhập hoặc mật khẩu không đúng", StatusCode.Fail);
                }
                else return new ResponseObject<UserLoginModel>(null, "Tên đăng nhập hoặc mật khẩu không đúng", StatusCode.Fail);
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
        public async Task<Response> AuthenticateBiometric(string userName, string deviceId, string biometricToken)
        {
            _funcpermsHandler = new FuncPermsHandler();
            try
            {
                if (!(string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(deviceId) || string.IsNullOrEmpty(biometricToken)))
                {
                    // Kiểm tra thông tin tên đăng nhập
                    _userHandler = new UserHandler();
                    UserLoginModel userLoginUpdate;

                    var existUser = await _userHandler.GetByUserName(userName) as ResponseObject<UserViewModel>;
                    if (existUser.Data != null)
                    {
                        var isValid = false;

                        decimal? coutLoginFall = existUser.Data.FailedPassCount + 1;
                        decimal totalminutes = existUser.Data.LockTime.HasValue ? (decimal)(DateTime.Now - existUser.Data.LockTime.Value).TotalMinutes : _lockTimeFailed.Value + 1;

                        if (existUser.Data.Status.Equals("LOCKED") && totalminutes <= _lockTimeFailed)
                        {
                            return new ResponseObject<UserLoginModel>(null, "Tài khoản hiện đang bị khóa", StatusCode.Fail);
                        }

                        // Xac thuc sinh trac hoc
                        _userBiometricHandler = new UserBiometricHandler();
                        var loginBiometricModel = new UserBiometricBaseModel
                        {
                            UserName = userName,
                            BiometricToken = biometricToken,
                            DeviceId = deviceId
                        };
                        var loginBiometricResult = await _userBiometricHandler.LoginBiometric(loginBiometricModel) as ResponseObject<ResponseModel>;
                        if (loginBiometricResult.StatusCode == StatusCode.Success)
                        {
                            isValid = !isValid;
                            biometricToken = loginBiometricResult.Data.Name;
                        }

                        if (isValid)
                        {
                            userLoginUpdate = new UserLoginModel();

                            var data = existUser?.Data;

                            var getAccessTokenResult = GetAccessToken("EXTERNAL").Result;
                            if (getAccessTokenResult != null) userLoginUpdate.AccessToken = getAccessTokenResult.access_token;


                            userLoginUpdate.Id = existUser.Data.Id;
                            userLoginUpdate.UserName = existUser.Data.UserName;
                            userLoginUpdate.Status = "ACTIVE";
                            userLoginUpdate.LastLoginDate = DateTime.Now;
                            userLoginUpdate.LastLogoutDate = existUser.Data.LastLogoutDate;
                            userLoginUpdate.FailedPassCount = 0;
                            userLoginUpdate.LockTime = null;
                            userLoginUpdate.IsVisibleCaptcha = false;
                            userLoginUpdate.FullName = existUser.Data.FullName;
                            userLoginUpdate.Phone = existUser.Data.Phone;
                            userLoginUpdate.Email = existUser.Data.Email;
                            userLoginUpdate.Address = existUser.Data.Address;
                            userLoginUpdate.Title = existUser.Data.Title;
                            userLoginUpdate.SessionTime = _sessionTime ?? 20;
                            userLoginUpdate.PosCD = existUser.Data.PosCD;
                            userLoginUpdate.DeptId = existUser.Data.DeptId;
                            userLoginUpdate.BiometricToken = biometricToken;
                            if (string.IsNullOrEmpty(existUser.Data.Avatar)) userLoginUpdate.Avatar = _avatarDefault;
                            else userLoginUpdate.Avatar = existUser.Data.Avatar;

                            List<FuncLoginModel> funPerm = new List<FuncLoginModel>();
                            if (await _funcpermsHandler.GetFuncPermsByUserId(existUser.Data.Id) is ResponseObject<List<FuncPermsBaseModel>> getFuncPerms && getFuncPerms.Data.Count > 0)
                            {
                                foreach (var item in getFuncPerms.Data)
                                {
                                    FuncLoginModel i = new FuncLoginModel();
                                    if (item.Code != null)
                                    {
                                        i.FuncPermCode = item.Code;
                                        i.FunctionId = item.FunctionId;
                                        funPerm.Add(i);
                                    }
                                }

                                userLoginUpdate.ListFuncPerms = funPerm;
                            }

                            // Tao token chua thong tin quyen han cua nguoi dung: ten dang nhap, danh sach quyen han,
                            // thoi gian session,...
                            var permissionTokenModel = new PermissionTokenModel
                            {
                                Id = Guid.NewGuid(),
                                UserName = userLoginUpdate.UserName,
                                ExpiredAfter = userLoginUpdate.SessionTime,
                                ListFuncPerms = userLoginUpdate.ListFuncPerms
                            };
                            userLoginUpdate.PermissionToken = GeneratePermissionToken(permissionTokenModel);

                            // Thêm thông tin token vào bảng key manager để quản lý phiên 
                            var procName = "PKG_SA_KEY_MANAGER.ADD_KEY";
                            var dyParam = new OracleDynamicParameters();
                            dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                            dyParam.Add("pUSERNAME", existUser.Data.UserName, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam.Add("pIDPERMISSIONTOKEN", permissionTokenModel.Id, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam.Add("pBEARERTOKEN", userLoginUpdate.AccessToken, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam.Add("pCREATEDONDATE", DateTime.Now, OracleMappingType.Date, ParameterDirection.Input);
                            var addKeyManager = await _keyManagerHandler.ExecuteProcOracle(procName, dyParam);

                            var updateUserSuccess = await _userHandler.UpdateLoginAsync(data.Id, userLoginUpdate);
                            return new ResponseObject<UserLoginModel>(userLoginUpdate, "Đăng nhập thành công");
                        }

                        else
                        {
                            string mess = "Tên đăng nhập hoặc mật khẩu không đúng";

                            userLoginUpdate = new UserLoginModel
                            {
                                Id = existUser.Data.Id,
                                LastLoginDate = existUser.Data.LastLoginDate,
                                LastLogoutDate = existUser.Data.LastLogoutDate,
                                FailedPassCount = coutLoginFall,
                                Status = "ACTIVE"
                            };

                            if (coutLoginFall >= _visibleCaptchaCount)
                            {
                                userLoginUpdate.IsVisibleCaptcha = true;
                            }
                            else
                            {
                                userLoginUpdate.IsVisibleCaptcha = false;
                            }

                            if (coutLoginFall >= _maxFailedCount)
                            {
                                userLoginUpdate.LockTime = DateTime.Now;
                                userLoginUpdate.Status = "LOCKED";
                                userLoginUpdate.FailedPassCount = 0;
                                userLoginUpdate.IsVisibleCaptcha = false;
                                mess = "Tài khoản hiện đang bị khóa";
                            }
                            else
                            {
                                userLoginUpdate.LockTime = existUser.Data.LockTime;
                            }

                            var updateUserFail = await _userHandler.UpdateLoginAsync(existUser.Data.Id, userLoginUpdate);

                            return new ResponseObject<UserLoginModel>(userLoginUpdate, mess, StatusCode.Fail);
                        }
                    }
                    else return new ResponseObject<UserLoginModel>(null, "Tên đăng nhập hoặc mật khẩu không đúng", StatusCode.Fail);
                }
                else return new ResponseObject<UserLoginModel>(null, "Thông tin xác thực không đủ", StatusCode.Fail);
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
        public async Task<Response> RevokeKey(string idPermissionToken)
        {
            try
            {
                var procName = "PKG_SA_KEY_MANAGER.REVOKE_KEY";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pIDPERMISSIONTOKEN", idPermissionToken, OracleMappingType.Varchar2, ParameterDirection.Input);
                return await _keyManagerHandler.ExecuteProcOracle(procName, dyParam);
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return null;
                }
                else throw ex;
            }
        }
        public async Task<AuthModel> GetAccessToken(string zone)
        {
            try
            {
                // Neu zone = EXTERNAL goi APIGW DMZ de sinh token
                if (string.Compare(zone, "EXTERNAL") == 0)
                {
                    _apiGatewayRootUrl = Helpers.GetConfig("AuthenticateExternal:APIGatewayRootUrl");
                    _clientId = Helpers.GetConfig("AuthenticateExternal:ClientId");
                    _clientSecret = Helpers.GetConfig("AuthenticateExternal:ClientSecret");
                }
                else
                {
                    _apiGatewayRootUrl = Helpers.GetConfig("Authenticate:APIGatewayRootUrl");
                    _clientId = Helpers.GetConfig("Authenticate:ClientId");
                    _clientSecret = Helpers.GetConfig("Authenticate:ClientSecret");
                }

                Uri getAccessTokenUri = new Uri(_apiGatewayRootUrl + "/token")
                    .AddQuery("grant_type", "client_credentials");

                var getAccessTokenRequest = (HttpWebRequest)WebRequest.Create(getAccessTokenUri);
                getAccessTokenRequest.Method = "POST";
                string basicToken = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(_clientId + ":" + _clientSecret));
                getAccessTokenRequest.ContentType = "application/x-www-form-urlencoded";
                getAccessTokenRequest.Accept = "application/json, text/javascript, */*";
                getAccessTokenRequest.Headers.Add("Authorization", "Basic " + basicToken);

                using (var getAccessTokenResponse = (HttpWebResponse)getAccessTokenRequest.GetResponse())
                {
                    var streamResponse = getAccessTokenResponse.GetResponseStream();
                    MemoryStream ms = new MemoryStream();
                    streamResponse.CopyTo(ms);
                    ms.Position = 0;
                    HttpResponseMessage resultResponse = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StreamContent(ms)
                    };
                    resultResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    string resultResponseStringData = await resultResponse.Content.ReadAsStringAsync();

                    var resultResponseJsonData = JsonConvert.DeserializeObject<AuthModel>(resultResponseStringData);
                    return resultResponseJsonData;
                }

            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return null;
                }
                else throw ex;
            }
        }
        private string GeneratePermissionToken(PermissionTokenModel permissionTokenModel)
        {
            string permissionToken = string.Empty;
            if (permissionTokenModel != null)
            {
                permissionTokenModel.ExpiredIn = DateTime.Now.AddMinutes((double)permissionTokenModel.ExpiredAfter);
                var tokenString = JsonConvert.SerializeObject(permissionTokenModel);
                // Ma hoa token
                permissionToken = EncryptedString.EncryptString(tokenString, Helpers.GetConfig("Encrypt:Key"));
            }
            return permissionToken;
        }
        private ResponseObject<bool> CheckUserAD(string userName, string passWord)
        {
            try
            {
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passWord))
                {
                    return new ResponseObject<bool>(false);
                }
                using (var cn = new LdapConnection())
                {
                    var adUserName = $"{_adDomain}\\{userName}";
                    // connect
                    cn.Connect(_adPath, LdapConnection.DEFAULT_PORT);
                    // bind with an username and password
                    // this how you can verify the password of an user
                    cn.Bind(LdapConnection.Ldap_V3, adUserName, passWord);
                    // call ldap op
                    // cn.Delete("<<userdn>>")
                    // cn.Add(<<ldapEntryInstance>>)
                    return new ResponseObject<bool>(true);
                }
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<bool>(false, "Lỗi ngoại lệ");
                }
                else throw ex;
            }
        }
        public async Task<Response> CheckKeyManager(string userName, string idPermissionToken, string bearerToken)
        {
            try
            {
                var procName = "PKG_SA_KEY_MANAGER.CHECK_KEY_VALID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pUSERNAME", userName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pIDPERMISSIONTOKEN", idPermissionToken, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pBEARERTOKEN", bearerToken, OracleMappingType.Varchar2, ParameterDirection.Input);
                var checkKeyManager = await _keyManagerHandler.ExecuteProcOracleReturnRow(procName, dyParam) as ResponseObject<List<KeyManagerModel>>;

                if (checkKeyManager != null && checkKeyManager.StatusCode == StatusCode.Success && checkKeyManager.Data != null && checkKeyManager.Data.Count > 0) return new ResponseObject<KeyManagerModel>(new KeyManagerModel() { Isvalid = 1 }, string.Empty, StatusCode.Success);
                else return new ResponseObject<KeyManagerModel>(new KeyManagerModel() { Isvalid = 0 }, string.Empty, StatusCode.Success);
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return null;
                }
                else throw ex;
            }
        }
    }
}

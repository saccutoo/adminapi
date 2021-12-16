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

namespace API.BussinessLogic
{
    public class AuthHandler : IAuthHandler
    {
        private UserHandler _userHandler;
        private readonly string _baseToken;
        private readonly string _apiGatewayRootUrl;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly decimal? _maxFailedCount;
        private readonly decimal? _lockTimeFailed;
        private readonly decimal? _visibleCaptchaCount;
        private readonly decimal? _isAuthenLdap;
        private readonly string _adPath;
        private readonly string _adDomain;
        public AuthHandler()
        {
            _maxFailedCount = decimal.Parse(Helpers.GetConfig("LoginConfig:MaxFailedCount"));
            _lockTimeFailed = decimal.Parse(Helpers.GetConfig("LoginConfig:LockTimeFailed"));
            _visibleCaptchaCount = decimal.Parse(Helpers.GetConfig("LoginConfig:VisibleCaptchaCount"));
            _baseToken = Helpers.GetConfig("Authenticate:BaseToken");
            _apiGatewayRootUrl = Helpers.GetConfig("Authenticate:APIGatewayRootUrl");
            _clientId = Helpers.GetConfig("Authenticate:ClientId");
            _clientSecret = Helpers.GetConfig("Authenticate:ClientSecret");
            _isAuthenLdap = decimal.Parse(Helpers.GetConfig("LDAP:IsAuthenLdap"));
            _adPath = Helpers.GetConfig("LDAP:ADHost");
            _adDomain = Helpers.GetConfig("LDAP:ADDomain");
        }
        public async Task<Response> Authenticate(string userName, string passWord, bool? isGetToken = null)
        {
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
                        if (_isAuthenLdap.Value != 1 || existUser.Data.Id == 0) isValid = string.Equals(userPass, inputHmac);
                        // Kiểm tra mk trong AD 
                        //TODO: Tam fix cho user=admin
                        else if (_isAuthenLdap.Value == 1 && existUser.Data.Id != 0)
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
                                var getAccessTokenResult = GetAccessToken().Result;
                                if (getAccessTokenResult != null) userLoginUpdate.AccessToken = getAccessTokenResult.access_token;
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
                return new ResponseObject<UserLoginModel>(null, ex.Message, StatusCode.Fail);
            }
        }
        public async Task<AuthModel> GetAccessToken()
        {
            try
            {
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
            catch (Exception)
            {
                return null;
            }
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
                return new ResponseObject<bool>(false, ex.Message);
            }
        }
    }
}

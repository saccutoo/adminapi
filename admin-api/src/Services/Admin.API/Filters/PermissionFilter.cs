using API.BussinessLogic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Linq;
using System.Net;
using Utils;

namespace API.Filters
{
    public class PermissionAttribute : TypeFilterAttribute
    {
        public PermissionAttribute(TypeFilter typeFilter, string value) : base(typeof(PermissionFilter))
        {
            Arguments = new object[] { typeFilter, value };
        }
    }
    public class PermissionFilter : IAuthorizationFilter
    {
        private readonly TypeFilter _typeFilter;
        private readonly string _value;
        private readonly ILogger<PermissionFilter> _logger;
        static RestClient _client;
        static RestRequest _request;
        public PermissionFilter(TypeFilter typeFilter, string value, ILogger<PermissionFilter> logger = null)
        {
            _typeFilter = typeFilter;
            _value = value;
            _logger = logger;
            _request = new RestRequest();
        }

        private string[] GetListPermissionFilter()
        {
            if (!string.IsNullOrEmpty(_value)) return _value.Split(",");
            return null;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                context.HttpContext.Request.Headers.TryGetValue("X-UserName", out StringValues userName);
                context.HttpContext.Request.Headers.TryGetValue("X-PermissionToken", out StringValues permissionTokenEncrypt);
                context.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues bearerToken);

                bearerToken = bearerToken.ToString().Replace("Bearer", "").Trim();
                // Cau hinh endpoint Key manager
                var isValidateKey = Helpers.GetConfig("KeyManagerEndpoint:IsValidateKey") == "1" ? true : false;
                var keyManagerEndpoint = Helpers.GetConfig("KeyManagerEndpoint:EndPoint");

                // Lay tat ca quyen duoc thuc thi controller
                var lstPermissionOfController = GetListPermissionFilter();

                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(permissionTokenEncrypt))
                {
                    // Giai ma token theo khoa 
                    var permissionTokenDecrypt = EncryptedString.DecryptString(permissionTokenEncrypt, Helpers.GetConfig("Encrypt:Key"));
                    // Chuyen doi thanh model
                    PermissionTokenModel permissionTokenModel = JsonConvert.DeserializeObject<PermissionTokenModel>(permissionTokenDecrypt);
                    if (permissionTokenModel != null)
                    {
                        if (isValidateKey)
                        {
                            // Kiem tra key hop le duoc luu trong DB      
                            string url = "/api/auth/check-key-manager?userName=" + userName + "&idPermissionToken=" + permissionTokenModel.Id.ToString() + "&bearerToken=" + bearerToken;
                            _client = new RestClient(keyManagerEndpoint);
                            _request.Resource = url;
                            _request.Method = Method.GET;
                            IRestResponse response = _client.Execute(_request);
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                var result = JsonConvert.DeserializeObject<ResponseObjectKeyManager>(response.Content);
                                if (result.Data.Isvalid == 0) context.Result = new BadRequestObjectResult("Key Manager NotFound");
                                else
                                {
                                    // Neu the loc quyen la "ALL" thi bo qua check quyen va nguoc lai
                                    if (!lstPermissionOfController.Select(sp => sp == "ALL").FirstOrDefault())
                                    {
                                        // Kiem tra thong tin userName tren header va trong permission token
                                        if (permissionTokenModel.UserName != userName) context.Result = new BadRequestObjectResult("Invalid Header");
                                        // Kiem tra thoi gian het han cua token
                                        // else if (DateTime.Compare(DateTime.Now, permissionTokenModel.ExpiredIn) == 1) context.Result = new BadRequestObjectResult("Token Expired");
                                        else
                                        {
                                            if (permissionTokenModel.ListFuncPerms != null && permissionTokenModel.ListFuncPerms.Count > 0)
                                            {
                                                // Lay tat ca quyen cua user
                                                var lstPermissionOfUser = permissionTokenModel.ListFuncPerms.Select(sp => sp.FuncPermCode).ToList();

                                                var checkPer = lstPermissionOfUser.Exists(sp => lstPermissionOfController.Contains(sp));
                                                if (!checkPer) context.Result = new BadRequestObjectResult("Permission Denied");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Neu the loc quyen la "ALL" thi bo qua check quyen va nguoc lai
                            if (!lstPermissionOfController.Select(sp => sp == "ALL").FirstOrDefault())
                            {
                                // Kiem tra thong tin userName tren header va trong permission token
                                if (permissionTokenModel.UserName != userName) context.Result = new BadRequestObjectResult("Invalid Header");
                                // Kiem tra thoi gian het han cua token
                                // else if (DateTime.Compare(DateTime.Now, permissionTokenModel.ExpiredIn) == 1) context.Result = new BadRequestObjectResult("Token Expired");
                                else
                                {
                                    if (permissionTokenModel.ListFuncPerms != null && permissionTokenModel.ListFuncPerms.Count > 0)
                                    {
                                        // Lay tat ca quyen cua user
                                        var lstPermissionOfUser = permissionTokenModel.ListFuncPerms.Select(sp => sp.FuncPermCode).ToList();

                                        var checkPer = lstPermissionOfUser.Exists(sp => lstPermissionOfController.Contains(sp));
                                        if (!checkPer) context.Result = new BadRequestObjectResult("Permission Denied");
                                    }
                                }
                            }
                        }
                    }
                    else context.Result = new BadRequestObjectResult("Invalid Header");
                }
                else context.Result = new BadRequestObjectResult("Invalid Header");
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                }
                context.Result = new BadRequestObjectResult("Invalid Header");
            }
        }
    }
    public enum TypeFilter
    {
        CheckPermission
    }
    public class ResponseObjectKeyManager
    {
        public KeyManagerModel Data { get; set; }
    }
}

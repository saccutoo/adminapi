using System;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public interface IAuthHandler
    {
        Task<Response> Authenticate(string userName, string passWord, string zone, bool? isGetToken = true);
        Task<Response> AuthenticateBiometric(string userName, string deviceId, string biometricToken);
        Task<Response> CheckKeyManager(string userName, string idPermissionToken, string bearerToken);
        Task<Response> RevokeKey(string idPermissionToken);
    }
}

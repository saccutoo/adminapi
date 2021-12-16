using System;
using Utils;

namespace API.BussinessLogic
{
    public class AuthModel 
    {
        public string access_token { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
        public decimal expires_in { get; set; }
        public string refresh_token { get; set; }       
    }   
    public class LoginModel
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
    }
}

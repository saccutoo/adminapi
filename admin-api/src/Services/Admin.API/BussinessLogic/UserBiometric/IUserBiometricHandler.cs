using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public interface IUserBiometricHandler
    {
        Task<Response> SetBiometric(UserBiometricCreateModel model);        
    }
}

using System;
using System.Collections.Generic;
using Utils;

namespace API.BussinessLogic
{
    public class UserBiometricBaseModel
    {
        public string UserName { get; set; }
        public string DeviceId { get; set; }
        
        public string BiometricToken { get; set; }
        public string Ip { get; set; }
    }
    public class UserBiometricModel: UserBiometricBaseModel
    {
        public decimal Id { get; set; }
        public string DeviceInfo { get; set; }
        public DateTime? LastLoginDate { get; set; }     
        public string Bm1 { get; set; }
        public string Bm2 { get; set; }
        public string Bm3 { get; set; }
        public string Bm4 { get; set; }
        public string Bm5 { get; set; }
        public string Bm6 { get; set; }
        public string Bm7 { get; set; }
        public string Bm8 { get; set; }
        public string Bm9 { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }      
    }    
    public class UserBiometricCreateModel
    {
        public string UserName { get; set; }
        public string DeviceId { get; set; }
        public string DeviceInfo { get; set; }
        public string Status { get; set; }
        public string Ip { get; set; }
        public string CreatedBy { get; set; }
    }
    public class UserBiometricQueryModel : PaginationRequest
    {
        public string Status { get; set; }
    }
}

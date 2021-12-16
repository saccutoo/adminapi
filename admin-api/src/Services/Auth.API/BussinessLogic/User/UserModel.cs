using System;
using System.Collections.Generic;
using Utils;

namespace API.BussinessLogic
{
    public class UserBaseModel : BaseModel
    {
        public decimal Id { get; set; }
        public decimal? DepartmentId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }    
        public string UserName { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public DateTime? Birthday { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastLogoutDate { get; set; }
        public decimal? FailedPassCount { get; set; }
        public DateTime? LockTime { get; set; }
        public bool? ForceChangePass { get; set; }
        public string Status { get; set; }
        public string Ip { get; set; }
        public string Agent { get; set; }
        public string PostingCode { get; set; }
    }

    public class UserViewModel: UserBaseModel
    {      
        public string DepartmentName { get; set; }
        public string FullName { get; set; }
    }
    public class UserLoginModel 
    {
        public decimal Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string AccessToken { get; set; }
        public bool IsVisibleCaptcha { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastLogoutDate { get; set; }
        public decimal? FailedPassCount { get; set; }
        public DateTime? LockTime { get; set; }
        public string Status { get; set; }

    }
    public class UserCreateModel
    {
        public decimal? DepartmentId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public DateTime? Birthday { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Ip { get; set; }
        public string Agent { get; set; }
        public string Status { get; set; }
        public string PostingCode { get; set; }
        public List<decimal> RolesId { get; set; }
    }

    public class UserUpdateModel
    {
        public decimal? DepartmentId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public DateTime? Birthday { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string Ip { get; set; }
        public string Agent { get; set; }
        public string PostingCode { get; set; }
        public List<decimal> RolesId { get; set; }
        public List<decimal> ListFuncPerms { get; set; }
    }

    public class UserDeleteModel
    {
        public List<decimal> ListId { get; set; }
    }
    public class ResponseOracleModel
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
    public class UserQueryModel : PaginationRequest
    {
        public decimal? IsPublic { get; set; }
        public string Status { get; set; }
        public decimal? DepartmentId { get; set; }
    }
}

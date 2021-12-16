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
        public string Emp_Id { get; set; }
        public string Ext { get; set; }
        public string Name_No_Sign { get; set; }
        public string Title { get; set; }
        public string Avatar { get; set; }
        public string Bm2 { get; set; }
        public string Bm3 { get; set; }
        public string Bm4 { get; set; }
        public string Bm5 { get; set; }
        public string Bm6 { get; set; }
        public string DeviceId { get; set; }
        public string FcmToken { get; set; }
    }
    public class UserViewModel : UserBaseModel
    {
        public string DepartmentName { get; set; }
        public string FullName { get; set; }
        public string PosCD { get; set; }
        public string DeptId { get; set; }
    }
    public class UserLoginModel
    {
        public decimal Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Avatar{ get; set; }
        public string Title { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string AccessToken { get; set; }
        public decimal ExpiresIn { get; set; }
        public string PermissionToken { get; set; }
        public bool IsVisibleCaptcha { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastLogoutDate { get; set; }
        public decimal? FailedPassCount { get; set; }
        public DateTime? LockTime { get; set; }
        public decimal SessionTime { get; set; }
        public string Status { get; set; }
        public string PosCD { get; set; }
        public string DeptId { get; set; }
        public string BiometricToken { get; set; }
        public List<FuncLoginModel> ListFuncPerms { get; set; }
        public Guid PermissionTokenId { get;  set; }
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


    /// <summary>
    /// Model chứa thông tin cập nhật
    /// người dùng, bao gồm thông tin thêm từ hệ thống khác, danh sách nhóm người dùng đc gán
    /// </summary>
    public class UserUserGroupUpdateModel
    {
        public string MaPhongBanIas { get; set; }
        public string PhongBanIas { get; set; }
        public string PhamViDuLieu { get; set; }
        public string MaPos { get; set; }
        public string TenPos { get; set; }
        public string TrangThaiNguoiDung { get; set; }
        public List<decimal> ListUserGroup { get; set; }
        public string ModifiedBy { get; set; }
    }

    #region Thuộc tính mở rộng
    public class PhongBanIasModel
    {
        public string MaPhongBanIas { get; set; }
        public string PhongBanIas { get; set; }
    }
    public class PosPhamViModel
    {
        public string MaPos { get; set; }
        public string TenPos { get; set; }
    }


    public class UserUpdateFCMModel
    {
        public decimal UserId { get; set; }
        public string DeviceId { get; set; }
        public string FcmToken { get; set; }
    }
    #endregion
}

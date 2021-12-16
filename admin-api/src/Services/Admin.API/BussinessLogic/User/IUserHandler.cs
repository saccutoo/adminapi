using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public interface IUserHandler
    {
        Task<Response> GetUserByBirthdayMonnthAsync(int month);
        Task<Response> GetUserByBirthdayDayAsync(int day,int month);
        Task<Response> FindAsync(decimal id);
        Task<Response> GetByEmpIDAsync(string Emp_ID);
        Task<Response> GetByEmailAsync(string Email);
        Task<Response> GetAllAsync();
        Task<Response> GetByUserName(string userName);
        Task<Response> CreateAsync(UserCreateModel model, BaseModel baseModel);
        Task<Response> UpdateAsync(decimal id, UserUpdateModel model);
        Task<Response> UpdateUserGroupAsync(decimal id, UserUserGroupUpdateModel model);
        Task<Response> UpdateLoginAsync(decimal id, UserLoginModel model);
        Task<Response> DeleteAsync(decimal id);
        Task<Response> DeleteManyAsync(UserDeleteModel model);
        Task<Response> GetUserByFilterAsync(UserQueryModel model);
        Task<Response> GetStaffByManagerAsync(string userName);
        Task<Response> UpdateFcmTokenAsync(UserUpdateFCMModel model);

        #region Hàm mở rộng
        Task<Response> GetAllPhongBanIas();
        Task<Response> GetPosTheoPhamVi(string maPhamVi);
        #endregion
    }
}

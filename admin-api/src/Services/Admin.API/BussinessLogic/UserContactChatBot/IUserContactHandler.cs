using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;
namespace API.BussinessLogic
{
    public interface IUserContactHandler
    {
        Task<Response> GetContactByNameAndDepartment(string userName,string deptName);
    }
}

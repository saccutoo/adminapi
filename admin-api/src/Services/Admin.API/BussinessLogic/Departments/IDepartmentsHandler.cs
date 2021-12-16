using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public interface IDepartmentsHandler
    {
        Task<Response> GetDepartByBranchsAsync(decimal branchId);
        Task<Response> GetTreeDepartAsync();
        Task<Response> GetFlatDepartByParentIdAsync(decimal id);
        Task<Response> GetTreeDepartSelectAsync();
        Task<Response> GetDepartmentTreeView();
        Task<Response> UpdateTel(decimal id, string tel);
        Task<Response> GetAllAncestorAsync(decimal currentDepartmentId);
    }
}

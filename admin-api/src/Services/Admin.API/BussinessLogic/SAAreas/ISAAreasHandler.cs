using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public interface ISAAreasHandler
    {
        Task<Response> GetAllAsync(string Type);
        Task<Response> GetByCode(string Code);
        Task<Response> GetById(decimal id);
        Task<Response> CreateAsync(SAAreasCreateModel model, IDbConnection iCon = null, IDbTransaction iTran = null);
        Task<Response> UpdateAsync(decimal id, SAAreasUpdateModel model, IDbConnection iCon = null, IDbTransaction iTran = null);
        Task<Response> ApprovedAsync(SAAreasApprovedModel model, IDbConnection iCon = null, IDbTransaction iTran = null);
        Task<Response> DeleteAsync(decimal id, string LastModifiedBy, IDbConnection iCon = null, IDbTransaction iTran = null);
        Task<Response> DeleteManyAsync(List<decimal> listRoleId, string LastModifiedBy, IDbConnection iCon = null, IDbTransaction iTran = null);
        Task<Response> GetByFilterAsync(SAAreasQueryModel model);
        Task<Response> CheckCodeAsync(decimal id, string code);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public interface IContactsHandler
    {
        Task<Response> FindAsync(decimal id);
        Task<Response> UpdateAsync(decimal id, ContactUpdateModel model);
        Task<Response> GetContactByFilterAsync(ContactQueryModel model);
        Task<Response> GetContactGroupByFilterAsync(ContactQueryModel model);
    }
}

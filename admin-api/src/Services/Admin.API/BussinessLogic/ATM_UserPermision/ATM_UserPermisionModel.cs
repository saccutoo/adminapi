using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public class ATM_UserPermisionBaseModel 
    {
        public decimal Id { get; set; }
        public string FullName { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
        public string PosCD { get; set; }
        public string Pos_CD { get; set; }      //dùng để get info từ oos cho chấm điểm kpi kế toán
        public string PosName { get; set; }
        public string ListPosCD { set; get; }
    }
    
    public class ATM_UserPermisionQueryModel : PaginationRequest
    {
        public decimal? Id_User { get; set; }

        public string PosCD { get; set; }
    }
    
}

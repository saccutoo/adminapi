using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public class DepartmentsBaseModel : BaseModel
    {
        public decimal Id { get; set; }
        public decimal? Parent_Dept_Id { get; set; }
        public string Name { get; set; }
        public string NameSelect { get; set; }
        public string Dept_Id { get; set; }
        public string Icon { get; set; }
        public string Tel { get; set; }
        public int? OrderView { get; set; }
        public string Status { get; set; }
        public string MainCd { get; set; }
        public string PosCd { get; set; }
        public string PostingCode { get; set; }
    }

    public class DepartmentsCreateModel
    {
        
    }

    public class DepartmentsUpdateModel
    {
        
    }

    public class DepartmentsQueryModel : PaginationRequest
    {
        public decimal? IsPublic { get; set; }
        public string Status { get; set; }
    }

    public class DepartmentsTreeModel : DepartmentsBaseModel
    {
        public bool? HasChild { get; set; }
        public bool? HasSeperate { get; set; }
        public List<DepartmentsTreeModel> Childs { get; set; }
    }

    public class TreeDepartmentModel
    {
        public decimal Id  { get; set; }
        public decimal? Parent_Dept_Id { get; set; }
        public string Name { get; set; }
        public string Tel { get; set; }
        public int? OrderView { get; set; }
        public bool IsHasChild { get; set; }
        public List<TreeDepartmentModel> ListChilds { get; set; }
    }
}

using Utils;

namespace API.BussinessLogic
{
    public class ChanelsBaseModel : BaseModel
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Decription { get; set; }
        public bool IsChecked { get; set; }
    }

    public class ChanelsQueryModel : PaginationRequest
    {
        public decimal? IsPublic { get; set; }
        public string Status { get; set; }
    }
}

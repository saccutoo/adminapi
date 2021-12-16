using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Utils;
namespace API.BussinessLogic
{
    public class UserContactCreatingModel
    {
        public string FullName;
        public string Dien_Thoai;
        public string Tel;
        public string Title;
        public string Trung_Tam;
        public string Ext;
        public string Email;
    }

    public class UserContactQueryModel : PaginationRequest
    {
        public decimal? IsPublic { get; set; }
        public string Status { get; set; }
    }

    public class UserContactResponseModel
    {
        [JsonProperty(PropertyName = "FullName")]
        public string FullName;
        [JsonProperty(PropertyName = "MobilePhone")]
        public string Dien_Thoai;
        [JsonProperty(PropertyName = "TrungTam")]
        public string Trung_Tam;
        public string Title;
        public string Tel;
        public string Ext;
        public string Email;
    }

    public class ContactFptAiInput
    {
        public string input_phongban { get; set; }
        public string input_ten_nv { get; set; }
    }

    public partial class FptResponse
    {
        public string type { get; set; }
        public FptContent content { get; set; }
    }
    public partial class FptContent
    {
        public string text { get; set; }
        public List<Object> buttons { get; set; }
    }
    public partial class ContactFptMessage
    {
        public List<FptResponse> messages { get; set; }

    }
}

using System;
using System.Runtime.Serialization;

namespace Utils
{
    public class BaseModel
    {
        //[IgnoreDataMember]
        public DateTime? OpenDate { get; set; }
        //[IgnoreDataMember]
        public string Maker { get; set; }
        //[IgnoreDataMember]
        public DateTime? MakerOnDate { get; set; }
        //[IgnoreDataMember]
        public string Approver { get; set; }
        //[IgnoreDataMember]
        public DateTime? ApproverOnDate { get; set; }
    }

    public class BaseCRUDModel
    {
        public string Status { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string ApproverBy { get; set; }
        public DateTime ApproverDate { get; set; }
    }
}
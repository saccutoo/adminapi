using System;
using System.Collections.Generic;
using System.Reflection;

namespace Utils
{
    #region Response

    /// <summary>
    /// Đối tượng phản hồi
    /// </summary>
    public class Response
    {
        public Response(StatusCode code, string message)
        {
            StatusCode = code;
            Message = message;
        }
        public Response(string message)
        {
            Message = message;
        }
        public Response()
        {
        }
        public StatusCode StatusCode { get; set; } = StatusCode.Success;
        public string Message { get; set; } = "Thành công";
        public decimal TotalCount { get; set; } = 0;
        public decimal TotalPage { get; set; } = 0;
        public decimal TotalRecord { get; set; } = 0;
    }

    /// <summary>
    /// Phản hồi lỗi
    /// </summary>
    public class ResponseError : Response
    {
        public ResponseError(StatusCode code, string message, IList<Dictionary<string, string>> errorDetail = null) : base(
            code,
            message)
        {
            ErrorDetail = errorDetail;
        }
        public IList<Dictionary<string, string>> ErrorDetail { get; set; }
    }

    /// <summary>
    /// Phản hồi dạng đối tượng
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseObject<T> : Response
    {
        public ResponseObject(T data)
        {
            Data = data;
        }
        public ResponseObject(T data, decimal totalCount, decimal totalPage)
        {
            Data = data;
            TotalCount = totalCount;
            TotalPage = totalPage;
        }
        public ResponseObject(T data, decimal totalCount, decimal totalPage, decimal totalRecord)
        {
            Data = data;
            TotalCount = totalCount;
            TotalPage = totalPage;
            TotalRecord = totalRecord;
        }
        public ResponseObject(T data, string message)
        {
            Data = data;
            Message = message;
        }
        public ResponseObject(T data, string message, StatusCode code)
        {
            StatusCode = code;
            Data = data;
            Message = message;
        }
        public ResponseObject(T data, string message, StatusCode code, decimal totalCount, decimal totalPage)
        {
            StatusCode = code;
            Data = data;
            Message = message;
            TotalPage = totalPage;
            TotalCount = totalCount;
        }
        public ResponseObject(T data, string message, StatusCode code, decimal totalCount, decimal totalPage, decimal totalRecord)
        {
            StatusCode = code;
            Data = data;
            Message = message;
            TotalPage = totalPage;
            TotalCount = totalCount;
            TotalRecord = totalRecord;
        }
        public T Data { get; set; }
    }
    #endregion


    /// <summary>
    /// Đối tượng mã trả về
    /// </summary>
   
    public enum StatusCode
    {
        Success = 0,
        FailRequired = 1,
        FailRole = -1,
        Fail = 99,
        FailSession = -99,
        FailApp = -98,
        FailFunction = -97,
        FailPermisstions = -96
    }

    public enum TypeInput
    {
        [StringValue("INPUT")]
        In = 0,
        [StringValue("OUTPUT")]
        Out = 1
    }

    /// <summary>
    /// Phản hồi kết quả xóa dữ liệu
    /// </summary>
    public class ResponseDelete : Response
    {
        public ResponseDelete(decimal id, string name)
        {
            Data = new ResponseDeleteModel { Id = id, Name = name };
        }
        public ResponseDelete(StatusCode code, string message, decimal id, string name) : base(code, message)
        {
            Data = new ResponseDeleteModel { Id = id, Name = name };
        }
        public ResponseDelete()
        {
        }
        public ResponseDeleteModel Data { get; set; }
    }

    /// <summary>
    /// Phản hồi kết quả xóa nhiều dữ liệu
    /// </summary>
    public class ResponseDeleteMulti : Response
    {
        public ResponseDeleteMulti(IList<ResponseDelete> data)
        {
            Data = data;
        }
        public ResponseDeleteMulti()
        {
        }
        public IList<ResponseDelete> Data { get; set; }
    }

    /// <summary>
    /// Đối tượng kết quả xóa
    /// </summary>
    public class ResponseDeleteModel
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
    }

    #region ORACLE
    public class ResponseOracle
    {
        public string STATUS_CODE { get; set; }
        public decimal ID { get; set; }
        public string NAME { get; set; }
    }

    public class ResponseModel
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
    public class StringValue : System.Attribute
    {
        private readonly string _value;

        public StringValue(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }

    }
    public static class StringEnum
    {
        public static string GetStringValue(Enum value)
        {
            string output = null;
            Type type = value.GetType();
            FieldInfo fi = type.GetField(value.ToString());
            StringValue[] attrs =
               fi.GetCustomAttributes(typeof(StringValue),
                                       false) as StringValue[];
            if (attrs.Length > 0)
            {
                output = attrs[0].Value;
            }

            return output;
        }
    }
    #endregion
}
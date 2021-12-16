using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Utils;

namespace API.BussinessLogic
{
    public class ContactModel 
    {
        public decimal Id { get; set; }
        public string UserName { get; set; }
        [JsonProperty(PropertyName = "EmployeeId")]
        public string Emp_Id { get; set; }
        public string FullName { get; set; }
        public string DepartmentName { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string DepartmentTel { get; set; }
        public string Ext { get; set; }
        public string Avatar { get; set; }
        public DateTime? Birthday { get; set; }
    }
    public class ContactGroupModel
    {
        public string GroupName { get; set; }
        public List<ContactModel> ListContact { get; set; }
    }
    public class ContactUpdateModel
    {
        //public IFormFile UploadFile { get; set; }
        public string AvatarUrl { get; set; }
        public string DepartmentTel { get; set; }
        public string Ext { get; set; }
        public string Phone { get; set; }
        public string LastModifiedBy { get; set; }
    }
    
    public class ContactQueryModel : PaginationRequest
    {
        public decimal? DepartmentId { get; set; }
    }    
}

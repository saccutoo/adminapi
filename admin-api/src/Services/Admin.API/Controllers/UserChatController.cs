using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.BussinessLogic;
using Utils;
using System.Net.Http;
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/contact/user")]
    [ApiController]
    public class UserChatController : ControllerBase
    {
        private readonly IUserContactHandler _userContactInterfaceHandler;
        public UserChatController(IUserContactHandler userContactInterfaceHandler)
        {
            _userContactInterfaceHandler = userContactInterfaceHandler;
        }

        /// <summary>
        /// Lấy danh bạ điện thoại cho ChatBot
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetContact()
        {
            string jsonString = "{}";
            try
            {
                //using (HttpContent content = Request.Content)
                //{
                // ... Read the string.
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                //return await reader.ReadToEndAsync();
                jsonString = await reader.ReadToEndAsync();
                //jsonString = rawString;
                //}
                //jsonString = Request.Content.ReadAsStringAsync().ToString();
            }
            catch (Exception ex)
            {
                jsonString = ex.Message;
            }
            //logger.Debug("Body Json: " + jsonString);
            string name = ""; string center = "";
            ContactFptAiInput user = JsonConvert.DeserializeObject<ContactFptAiInput>(jsonString);
            name = user.input_ten_nv;
            center = user.input_phongban;
            ContactFptMessage objList = new ContactFptMessage();
            FptResponse itemResponse = null;
            try
            {
                List<FptResponse> objmessage = new List<FptResponse>();
                var resultFromHandler = await _userContactInterfaceHandler.GetContactByNameAndDepartment(name, center);
                if (resultFromHandler.TotalCount == 0)
                {
                    itemResponse = new FptResponse();
                    FptContent contents = new FptContent();
                    contents.buttons = new List<object>();
                    itemResponse.type = "text";
                    contents.text = "Xin lỗi chúng tôi không tim thấy thông tin!";
                    itemResponse.content = contents;
                    objmessage.Add(itemResponse);
                    objList.messages = objmessage;
                }
                else
                {
                    ResponseObject<List<UserContactResponseModel>> responseObject = (ResponseObject<List<UserContactResponseModel>>)resultFromHandler;
                    for (int i = 0; i < responseObject.Data.Count; i++)
                    {
                        UserContactResponseModel itemUserContact = responseObject.Data[i];

                        itemResponse = new FptResponse();
                        FptContent contents = new FptContent();
                        contents.buttons = new List<object>();
                        itemResponse.type = "text";
                        //contents.text = "Tên: " + itemUserContact.FullName + "\n" + drDivision + "-" + drCenter + "\nDi động: " + drPhone + "\nSố máy bàn: " + drTEL + " - " + drEXT;
                        contents.text = "Tên: " + itemUserContact.FullName + "\n" + itemUserContact.Trung_Tam + "\nDi động: " + itemUserContact.Dien_Thoai + "\nSố máy bàn: " + itemUserContact.Tel + " - " + itemUserContact.Ext + "\nEmail: "+ itemUserContact.Email;
                        itemResponse.content = contents;
                        objmessage.Add(itemResponse);
                    }
                    objList.messages = objmessage;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            String jsonContent = JsonConvert.SerializeObject(objList);
            //logger.Debug("jsonContent: " + jsonContent);
            /*var resp = new HttpResponseMessage()
            {

                Content = new StringContent(jsonContent)

            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");*/
            return  new ObjectResult(objList) { StatusCode = 200 }; ;
        }

    }
}

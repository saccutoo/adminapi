namespace Utils
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;

    public class ApiVersionOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var apiGroupNames = context.ApiDescription.ControllerAttributes()
                .OfType<ApiExplorerSettingsAttribute>()
                .Where(x => !x.IgnoreApi)
                .Select(x => x.GroupName)
                .ToList();
#pragma warning restore CS0618 // Type or member is obsolete
            if (apiGroupNames.Count != 0) operation.Tags = apiGroupNames.ToList();


            var apiVersion = context.ApiDescription.GetApiVersion();

            // If the api explorer did not capture an API version for this operation then the action must be API
            // version-neutral, so there's nothing to add.
            if (apiVersion == null)
            {
                return;
            }

            var parameters = operation.Parameters;

            if (parameters == null) operation.Parameters = parameters = new List<IParameter>();

            #region

            //operation.Responses.Add("401", new Swashbuckle.AspNetCore.Swagger.Response { Description = "Người dùng chưa được xác thực" });
            //operation.Responses.Add("400", new Swashbuckle.AspNetCore.Swagger.Response { Description = "Lỗi yêu cầu tài nguyên" });
            //operation.Responses.Add("403", new Swashbuckle.AspNetCore.Swagger.Response { Description = "Không có quyền yêu cầu tài nguyên" });
            //operation.Responses.Add("404", new Swashbuckle.AspNetCore.Swagger.Response { Description = "Không tìm thấy tài nguyên yêu cầu" });
            //operation.Responses.Add("405", new Swashbuckle.AspNetCore.Swagger.Response { Description = "Không hỗ trợ phương thức yêu cầu" });
            //operation.Responses.Add("500", new Swashbuckle.AspNetCore.Swagger.Response { Description = "Có lỗi trong quá trình xử lý" });

            #endregion

            //var apiversion = parameters.FirstOrDefault(p => p.Name == "api-version");
            //if (apiversion != null)
            //{
            //    parameters.Remove(apiversion);
            //}

            #region Add Header

            // parameters.Add(new NonBodyParameter()
            // {
            //     Name = "X-UserId",
            //     Description = "Id of User",
            //     In = "header",
            //     Required = true,
            //     Type = "string",
            //     Default = UserConstants.ADMINISTRATOR_ID
            // });
            parameters.Add(new NonBodyParameter
            {
                Name = "X-ApplicationId",
                Description = "Id of App",
                In = "header",
                Required = true,
                Type = "string",
                Default = AppConstants.RootAppId
            });
            // parameters.Add(new NonBodyParameter()
            // {
            //     Name = "X-Token",
            //     Description = "Token authorize",
            //     In = "header",
            //     Required = false,
            //     Type = "string"
            // });

            #endregion

            #region Add Upload

            var files = operation.Parameters.Where(s => s.Name == "files").FirstOrDefault();
            if (files != null)
            {
                operation.Parameters.Remove(files);
                operation.Parameters.Add(new NonBodyParameter
                {
                    //Name = "files",
                    //Description = "Upload multiple file",
                    //Required = true,
                    //Type = "array",
                    //In = "formData",
                    //Items = new PartialSchema
                    //{
                    //    Type = "file",
                    //    Format = "formData",

                    //    CollectionFormat = "multi"
                    //},

                    Name = "files",
                    Description = "Upload file",
                    Required = true,
                    Type = "file",
                    In = "formData"

                });
                operation.Consumes.Add("multipart/form-data");
            }

            var file = operation.Parameters.Where(s => s.Name == "file").FirstOrDefault();
            if (file != null)
            {
                operation.Parameters.Remove(file);
                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = "file",
                    Description = "Upload file",
                    Required = true,
                    Type = "file",
                    In = "formData"
                });
                operation.Consumes.Add("multipart/form-data");
            }

            #endregion
        }
    }
}
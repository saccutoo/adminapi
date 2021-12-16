using System.Threading.Tasks;
using Utils;
using API.Infrastructure.Repositories;
using Admin.API.Infrastructure.Migrations;
using Dapper.Oracle;
using System.Data;
using System;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace API.BussinessLogic
{
    public class ContactsHandler : IContactsHandler
    {
        private readonly RepositoryHandler<SaUsers, ContactModel, ContactQueryModel> _contactRepositoryHandler = new RepositoryHandler<SaUsers, ContactModel, ContactQueryModel>();
        private readonly ILogger<ContactsHandler> _logger;
        private readonly string _rootPath;
        private readonly string _avatarDefault;
        private readonly decimal _defaultRootDepartmentId;
        public ContactsHandler(ILogger<ContactsHandler> logger = null)
        {
            _avatarDefault = Helpers.GetConfig("StaticFiles:AvatarDefault");
            _rootPath = Helpers.GetConfig("StaticFiles:Folder");
            _defaultRootDepartmentId = decimal.Parse(Helpers.GetConfig("Other:DefaultRootDepartmentId"));
            _logger = logger;
        }

        public async Task<Response> UpdateAsync(decimal id, ContactUpdateModel model)
        {
            try
            {
                if (!(await FindAsync(id) is ResponseObject<ContactModel> existContact))
                {
                    return new ResponseError(StatusCode.Fail, "Không tìm thấy bản ghi để cập nhật");
                }

                var procName = "PKG_SA_USERS.UPDATE_CONTACT";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PPHONE", !string.IsNullOrEmpty(model.Phone) ? model.Phone : existContact.Data.Phone, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PEXT", !string.IsNullOrEmpty(model.Ext) ? model.Ext : existContact.Data.Ext, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PMAKER", model.LastModifiedBy, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pMAKERONDATE", DateTime.Now, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("PID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("pDEPARTMENTTEL", !string.IsNullOrEmpty(model.DepartmentTel) ? model.DepartmentTel : "N/A", OracleMappingType.Varchar2, ParameterDirection.Input);
                // Upload file
                //byte[] contentUploadFile;
                //string fileName = existContact.Data.Avatar;
                //if (model.UploadFile != null)
                //{
                //    using (var memoryStream = new MemoryStream())
                //    {
                //        await model.UploadFile.CopyToAsync(memoryStream);
                //        contentUploadFile = memoryStream.ToArray();
                //    }

                //    // Tao duong dan file
                //    fileName = Guid.NewGuid().ToString() + ".jpg";
                //    string folderPath = string.Join('/', new string[] { _rootPath, "avatar" });
                //    string fullPath = string.Join('/', new string[] { folderPath, fileName });
                //    // Tao thu muc neu chua co
                //    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                //    // Xoa file cu
                //    if (!string.IsNullOrEmpty(existContact.Data.Avatar))
                //    {
                //        try
                //        {
                //            string fileOld = string.Join('/', new string[] { folderPath, existContact.Data.Avatar });
                //            File.Delete(fileOld);
                //        }
                //        catch { }
                //    }

                //    // Ghi file 
                //    File.WriteAllBytes(fullPath, contentUploadFile);
                //}
                dyParam.Add("PAVATAR", !string.IsNullOrEmpty(model.AvatarUrl) ? model.AvatarUrl : existContact.Data.Avatar, OracleMappingType.Varchar2, ParameterDirection.Input);

                return await _contactRepositoryHandler.ExecuteProcOracle(procName, dyParam);

            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ");
                }
                else throw ex;
            }
        }
        public async Task<Response> FindAsync(decimal id)
        {
            try
            {
                var procName = "PKG_SA_USERS.GET_CONTACT_BY_ID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                var existUser = await _contactRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, false) as ResponseObject<ContactModel>;

                if (existUser.StatusCode == StatusCode.Success)
                {
                    if (string.IsNullOrEmpty(existUser.Data.Avatar)) existUser.Data.Avatar = _avatarDefault;
                    return new ResponseObject<ContactModel>(existUser.Data, "Thành công", StatusCode.Success);
                }
                else return new ResponseObject<ContactModel>(null, "Không thành công", StatusCode.Fail);
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ");
                }
                else throw ex;
            }
        }
        public async Task<Response> GetContactByFilterAsync(ContactQueryModel model)
        {
            try
            {
                var procName = "PKG_SA_USERS.GET_CONTACT_BY_FILTER";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PPAGESIZE", model.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PPAGEINDEX", model.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PFULLTEXTSEARCH", !string.IsNullOrEmpty(model.FullTextSearch) ? model.FullTextSearch : null, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PDEPARTMENTID", (!model.DepartmentId.HasValue || model.DepartmentId == -1) ? _defaultRootDepartmentId : model.DepartmentId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _contactRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ");
                }
                else throw ex;
            }
        }
        public async Task<Response> GetContactGroupByFilterAsync(ContactQueryModel model)
        {
            try
            {
                var procName = "PKG_SA_USERS.GET_CONTACT_BY_FILTER";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PPAGESIZE", model.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PPAGEINDEX", model.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PFULLTEXTSEARCH", !string.IsNullOrEmpty(model.FullTextSearch) ? model.FullTextSearch : null, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PDEPARTMENTID", (!model.DepartmentId.HasValue || model.DepartmentId == -1) ? _defaultRootDepartmentId : model.DepartmentId, OracleMappingType.Decimal, ParameterDirection.Input);


                if (await _contactRepositoryHandler.ExecuteProcOracleReturnRow(procName, dyParam, true) is ResponseObject<List<ContactModel>> lstContact && lstContact.Data.Count > 0)
                {
                    var newResult = new ResponseObject<List<ContactGroupModel>>(new List<ContactGroupModel>(), "Không có dữ liệu", StatusCode.Success, lstContact.TotalCount, lstContact.TotalPage, lstContact.TotalRecord);
                    var lstContactResult = lstContact.Data;

                    if (lstContactResult != null && lstContactResult.Count > 0)
                    {
                        // Lay cac nhom phong ban trong ket qua
                        var lstGroupPhongBan = (from pb in lstContactResult
                                                group pb by pb.DepartmentName into g
                                                select new ContactGroupModel
                                                {
                                                    GroupName = g.Key,
                                                    ListContact = g.Select(p => p).ToList()
                                                }).ToList();
                        if (lstGroupPhongBan.Count > 0)
                        {
                            newResult.Message = "Thành công";
                            newResult.Data = lstGroupPhongBan;
                        }
                    }
                    return newResult;
                }
                return new ResponseObject<List<ContactGroupModel>>(null, "Không tìm thấy dữ liệu", StatusCode.Success);
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ");
                }
                else throw ex;
            }
        }
    }
}

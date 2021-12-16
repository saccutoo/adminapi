using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace API.Infrastructure.Repositories
{
    public class RepositoryHandler<TDbModel, TResultModel, TQueryModel>
        where TDbModel : class
        where TResultModel : class
        where TQueryModel : PaginationRequest
    {
        public static RepositoryHandler<TDbModel, TResultModel, TQueryModel> Instance { get; } =
            new RepositoryHandler<TDbModel, TResultModel, TQueryModel>();

        public async Task<bool> CheckPropValidate(TDbModel request, bool isUpdate, Guid? appId = null,
            Guid? actorId = null, params string[] propValidate)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    if (propValidate.Length == 0)
                    {
                        return false;
                    }
                    var checkQuery = unitOfWork.GetRepository<TDbModel>().GetAll();
                    if (isUpdate)
                    {
                        var compare = request.GetPropValue("Id");
                        checkQuery = checkQuery.Where("Id", compare, ExtensionHelpers.ExpressionOption.NotEqual);
                    }

                    foreach (var prop in propValidate)
                    {
                        var compare = request.GetPropValue(prop);
                        checkQuery = checkQuery.Where(prop, compare, ExtensionHelpers.ExpressionOption.Equal);
                    }

                    var result = await checkQuery.AnyAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #region CRUD 

        public async Task<Response> FindAsync(decimal id, Guid? appId = null, Guid? actorId = null)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var checkExiest = await unitOfWork.GetRepository<TDbModel>().FindAsync(id);
                    if (checkExiest == null)
                    {
                        return new ResponseError(StatusCode.Fail, "Id không tồn tại");
                    }

                    return new ResponseObject<TResultModel>(
                        AutoMapperHelpers.AutoMap<TDbModel, TResultModel>(checkExiest));
                }
            }
            catch (Exception ex)
            {
                return new ResponseError(StatusCode.Fail, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        public async Task<Response> CreateAsync(TDbModel request, params string[] propValidate)
        {
            return await CreateAsync(request, null, null, propValidate);
        }

        public async Task<Response> CreateAsync(TDbModel request, Guid? appId = null, Guid? actorId = null,
            params string[] propValidate)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    if (propValidate != null)
                    {
                        var check = await CheckPropValidate(request, false, appId, actorId, propValidate);
                        if (check)
                        {
                            return new ResponseError(StatusCode.Fail,
                                "Trùng thông tin. Vui lòng kiểm tra thuộc tính: " + string.Join(", ", propValidate));
                        }
                    }

                    unitOfWork.GetRepository<TDbModel>().Add(request);
                    if (await unitOfWork.SaveAsync() > 0)
                        return new ResponseObject<TResultModel>(
                            AutoMapperHelpers.AutoMap<TDbModel, TResultModel>(request));

                    return new ResponseError(StatusCode.Fail, "Câu lệnh sql không thể thực thi");
                }
            }
            catch (Exception ex)
            {
                return new ResponseError(StatusCode.Fail, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        public async Task<Response> UpdateAsync(decimal id, TDbModel request, params string[] propValidate)
        {
            return await UpdateAsync(id, request, null, null, propValidate);
        }

        public async Task<Response> UpdateAsync(decimal id, TDbModel request, Guid? appId = null, Guid? actorId = null,
            params string[] propValidate)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    if (propValidate != null)
                    {
                        var check = await CheckPropValidate(request, true, appId, actorId, propValidate);
                        if (check)
                        {
                            return new ResponseError(StatusCode.Fail,
                                "Trùng thông tin. Vui lòng kiểm tra thuộc tính: " + string.Join(", ", propValidate));
                        }
                    }

                    var checkExiest = await unitOfWork.GetRepository<TDbModel>().AnyAsync(id);
                    if (!checkExiest)
                    {
                        return new ResponseError(StatusCode.Fail, "Id không tồn tại");
                    }

                    unitOfWork.GetRepository<TDbModel>().Update(request);
                    if (await unitOfWork.SaveAsync() > 0)
                        return new ResponseObject<TResultModel>(
                            AutoMapperHelpers.AutoMap<TDbModel, TResultModel>(request));

                    return new ResponseError(StatusCode.Fail, "Câu lệnh sql không thể thực thi");
                }
            }
            catch (Exception ex)
            {
                return new ResponseError(StatusCode.Fail, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        public async Task<Response> DeleteAsync(decimal id, Guid? appId = null, Guid? actorId = null)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var checkExiest = await unitOfWork.GetRepository<TDbModel>().FindAsync(id);
                    if (checkExiest == null)
                    {
                        return new ResponseError(StatusCode.Fail, "Id không tồn tại");
                    }

                    var name = "";
                    try
                    {
                        name = checkExiest.GetPropValue("Name").ToString();
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    unitOfWork.GetRepository<TDbModel>().Delete(checkExiest);
                    if (await unitOfWork.SaveAsync() > 0)
                        return new ResponseDelete(id, name);

                    return new ResponseError(StatusCode.Fail, "Câu lệnh sql không thể thực thi");
                }
            }
            catch (Exception ex)
            {
                return new ResponseError(StatusCode.Fail, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        public async Task<Response> DeleteRangeAsync(IList<decimal> listId, string idName, Guid? appId = null,
            Guid? actorId = null)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var result = new List<ResponseDelete>();
                    var listCurrent = await unitOfWork.GetRepository<TDbModel>().GetAll().WhereContains(idName, listId)
                        .ToListAsync();
                    if (listCurrent.Count == 0)
                    {
                        return new ResponseError(StatusCode.Fail, "danh sách Id không tồn tại");
                    }

                    foreach (var id in listId)
                    {
                        var current = listCurrent.AsQueryable()
                            .Where(idName, id, ExtensionHelpers.ExpressionOption.Equal).FirstOrDefault();
                        if (current != null)
                        {
                            var name = "";
                            try
                            {
                                name = current.GetPropValue("Name").ToString();
                            }
                            catch (Exception)
                            {
                                // ignored
                            }

                            result.Add(new ResponseDelete(id, name));
                        }
                        else
                        {
                            result.Add(new ResponseDelete(StatusCode.Fail, "Id không tồn tại", id, ""));
                        }
                    }

                    unitOfWork.GetRepository<TDbModel>().DeleteRange(listCurrent);
                    if (await unitOfWork.SaveAsync() > 0)
                        return new ResponseDeleteMulti(result);

                    return new ResponseError(StatusCode.Fail, "Câu lệnh sql không thể thực thi");
                }
            }
            catch (Exception ex)
            {
                return new ResponseError(StatusCode.Fail, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        public async Task<Response> GetAllAsync(Expression<Func<TDbModel, bool>> predicate, Guid? appId = null,
            Guid? actorId = null)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var result = await unitOfWork.GetRepository<TDbModel>().GetListAsync(predicate);
                    if (result != null)
                        return new ResponseObject<List<TResultModel>>(
                            AutoMapperHelpers.AutoMap<TDbModel, TResultModel>(result));

                    return new ResponseError(StatusCode.Fail, "Không tìm thấy dữ liệu");
                }
            }
            catch (Exception ex)
            {
                return new ResponseError(StatusCode.Fail, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        public Response GetAll(Guid? appId = null, Guid? actorId = null)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var result = unitOfWork.GetRepository<TDbModel>().GetAll().AsNoTracking();
                    if (result != null)
                        return new ResponseObject<List<TResultModel>>(
                            AutoMapperHelpers.AutoMap<TDbModel, TResultModel>(result.ToList()));

                    return new ResponseError(StatusCode.Fail, "Không tìm thấy dữ liệu");
                }
            }
            catch (Exception ex)
            {
                return new ResponseError(StatusCode.Fail, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        #endregion

        #region ORACLE HANDLER
        /// <summary>
        /// Thực thi thủ tục và trả về danh sách kết quả
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="dyParam"></param>
        /// <param name="returnList"></param>
        /// <returns></returns>
        public async Task<Response> ExecuteProcOracleReturnRow(string procName, object dyParam = null, bool returnList = true)
        {
            try
            {
                using (var unitOfWorkOracle = new UnitOfWorkOracle())
                {
                    var iConn = unitOfWorkOracle.GetConnection();
                    var result = await SqlMapper.QueryAsync<TDbModel>(iConn, procName, param: dyParam, commandType: CommandType.StoredProcedure);
                    if (result != null && result.Count() > 0 && returnList)
                    {
                        var totalRecord = result.FirstOrDefault().GetPropValue("TotalRecord") == null ? -1 : (decimal)result.FirstOrDefault().GetPropValue("TotalRecord");
                        var totalPage = result.FirstOrDefault().GetPropValue("TotalPage") == null ? -1 : (decimal)result.FirstOrDefault().GetPropValue("TotalPage");
                        var totalCount = result.Count();
                        return new ResponseObject<List<TResultModel>>(
                            AutoMapperHelpers.AutoMap<TDbModel, TResultModel>(result.ToList()), totalCount, totalPage, totalRecord);
                    }
                    else if (result != null && !returnList)
                        return new ResponseObject<TResultModel>(
                            AutoMapperHelpers.AutoMap<TDbModel, TResultModel>(result.FirstOrDefault()));
                    //return new ResponseError(StatusCode.Fail, "Không tìm thấy dữ liệu");
                    return new ResponseObject<TResultModel>(null, "Không tìm thấy dữ liệu", StatusCode.Fail);
                }
            }
            catch (Exception ex)
            {
                return new ResponseError(StatusCode.Fail, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }


        /// <summary>
        /// Thực thi thủ tục và trả về trạng thái thực thi
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="dyParam"></param>
        /// <returns></returns>
        public async Task<Response> ExecuteProcOracle(string procName, object dyParam = null)
        {
            try
            {
                using (var unitOfWorkOracle = new UnitOfWorkOracle())
                {
                    var iConn = unitOfWorkOracle.GetConnection();
                    var result = await SqlMapper.QueryFirstAsync<ResponseOracle>(iConn, procName, param: dyParam, commandType: CommandType.StoredProcedure);
                    if (result != null)
                    {
                        var res = new ResponseObject<ResponseOracle>(
                            AutoMapperHelpers.AutoMap<ResponseOracle, ResponseOracle>(result));

                        return new ResponseObject<ResponseModel>(new ResponseModel() {
                            Id = res.Data.ID,
                            Name = res.Data.NAME,
                            Status = res.Data.STATUS_CODE,
                            Message = res.Data.STATUS_CODE.Equals("00") ? "Thành công" : "Không thành công"
                        }, 
                        res.Data.STATUS_CODE.Equals("00") ? "Thành công" : "Không thành công",
                        res.Data.STATUS_CODE.Equals("00") ? StatusCode.Success : StatusCode.Fail);
                    }
                        
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Thực thi thủ tục và trả về trạng thái thực thi
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="dyParam"></param>
        /// <returns></returns>
        public async Task<Response> ExecuteProcOracle(string procName, UnitOfWorkOracle unitOfWorkOracle, object dyParam = null)
        {
            try
            {
                var iConn = unitOfWorkOracle.GetConnection();
                var result = await SqlMapper.QueryFirstAsync<ResponseOracle>(iConn, procName, param: dyParam, commandType: CommandType.StoredProcedure);
                if (result != null)
                    return new ResponseObject<ResponseOracle>(
                        AutoMapperHelpers.AutoMap<ResponseOracle, ResponseOracle>(result));
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
    }
}
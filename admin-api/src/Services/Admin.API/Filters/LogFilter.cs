using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Text;
using Utils;
using System.IO;
using Microsoft.Extensions.Primitives;
using System.Runtime;

namespace API.Filters
{
    public class LogFilter : IActionFilter
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(LogFilter));

        private readonly ILogger<LogFilter> _logger;
        public string repositoryId = Guid.NewGuid().ToString();
        public LogFilter(ILogger<LogFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            try
            {
                context.HttpContext.Request.Headers.TryGetValue("X-UserId", out StringValues UserNameClient);
                context.HttpContext.Request.Headers.TryGetValue("Host", out StringValues URL);
                context.HttpContext.Request.Headers.TryGetValue("User-Agent", out StringValues UserAgent);
                string IPClient = GetFirstIP(context.HttpContext.Connection.RemoteIpAddress.ToString());
                LogInfo.WriteLogInfo(_log4net, repositoryId, UserNameClient, URL, UserAgent, IPClient, "LogFilter.OnActionExecuted", TypeInput.Out, DateTime.Now, context);
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex, "OnActionExecuted Error");
            }
            try
            {
                GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                GC.Collect();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "OnActionExecuted Error");
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {          
            try
            {
                context.HttpContext.Response.Headers.Add("repositoryId", repositoryId);
                context.HttpContext.Request.Headers.TryGetValue("X-UserId", out StringValues UserNameClient);
                context.HttpContext.Request.Headers.TryGetValue("Host", out StringValues URL);
                context.HttpContext.Request.Headers.TryGetValue("User-Agent", out StringValues UserAgent);
                string IPClient = GetFirstIP(context.HttpContext.Connection.RemoteIpAddress.ToString());

                LogInfo.WriteLogInfo(_log4net, repositoryId, UserNameClient , URL, UserAgent, IPClient, "LogFilter.OnActionExecuted", TypeInput.In, DateTime.Now , context);
            }
            catch(Exception ex)
            {                              
                _logger.LogError(ex, "OnActionExecuting Error");
            }
        }

        public string GetFirstIP(string ip)
        {
            try
            {
                if (ip.ToString().LastIndexOf(".") > 0)
                {
                    int i = ip.ToString().LastIndexOf(".");
                    ip = ip.ToString().Substring(i + 1);
                }
                else
                    ip = ip.ToString();
            }
            catch
            {

            }

            return ip;
        }
    }
}

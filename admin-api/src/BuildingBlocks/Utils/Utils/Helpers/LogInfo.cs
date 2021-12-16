using log4net;
using log4net.Appender;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utils;

namespace Utils
{
    public static class LogInfo
    {        
        public static void WriteLogInfo(log4net.ILog l4NC, string RepositoryId,string UserNameClient, string URL, string UserAgent, string IPClient, string FunctionName, TypeInput OutOrInt, DateTime dateNow, object input)
        {
            LogModel<object> objLog = new LogModel<object>();
            try
            {
                objLog.RepositoryId = RepositoryId;
                objLog.ChannelCode = Config.AppSettings.KEY_CHANNEL;
                objLog.ApplicationCode = Config.AppSettings.KEY_APPLICATION;
                objLog.FucntionCode = FunctionName;
                objLog.TypePut = OutOrInt.ToString();
                objLog.Key = UserNameClient;
                objLog.Url = URL;
                objLog.UserAgent = UserAgent;
                objLog.IPClient = IPClient;
                objLog.TimePush = dateNow;
                objLog.Data = input;
                l4NC.Info(objLog);
            }
            catch (Exception ex)
            {
                objLog.ExceptionMessage = ex.Message;
                objLog.Description = ex.ToString();
                WriteFileLog(JsonConvert.SerializeObject(objLog));
            }
        }
        public static void WriteLogError(log4net.ILog l4NC, string RepositoryId, string UserNameClient, string URL, string UserAgent, string IPClient, string FunctionName, TypeInput OutOrInt, DateTime dateNow, object input)
        {
            LogModel<object> objLog = new LogModel<object>();
            try
            {
                objLog.RepositoryId = RepositoryId;
                objLog.ChannelCode = Config.AppSettings.KEY_CHANNEL;
                objLog.ApplicationCode = Config.AppSettings.KEY_APPLICATION;
                objLog.FucntionCode = FunctionName;
                objLog.TypePut = OutOrInt.ToString();
                objLog.Key = UserNameClient;
                objLog.Url = URL;
                objLog.UserAgent = UserAgent;
                objLog.IPClient = IPClient;
                objLog.TimePush = dateNow;
                //objLog.Data = input;
                l4NC.Error(objLog);
            }
            catch (Exception ex)
            {
                objLog.ExceptionMessage = ex.Message;
                objLog.Description = ex.ToString();
                WriteFileLog(JsonConvert.SerializeObject(objLog));
            }
        }
        public static void WriteLogWarn(log4net.ILog l4NC, string RepositoryId, string UserNameClient, string URL, string UserAgent, string IPClient, string FunctionName, TypeInput OutOrInt, DateTime dateNow, object input)
        {
            LogModel<object> objLog = new LogModel<object>();
            try
            {
                objLog.RepositoryId = RepositoryId;
                objLog.ChannelCode = Config.AppSettings.KEY_CHANNEL;
                objLog.ApplicationCode = Config.AppSettings.KEY_APPLICATION;
                objLog.FucntionCode = FunctionName;
                objLog.TypePut = OutOrInt.ToString();
                objLog.Key = UserNameClient;
                objLog.Url = URL;
                objLog.UserAgent = UserAgent;
                objLog.IPClient = IPClient;
                objLog.TimePush = dateNow;
                //objLog.Data = input;
                l4NC.Warn(objLog);
            }
            catch (Exception ex)
            {
                objLog.ExceptionMessage = ex.Message;
                objLog.Description = ex.ToString();
                WriteFileLog(JsonConvert.SerializeObject(objLog));
            }
        }             
       

        public static void WriteFileLog(string errCode)
        {
            try
            {            
                string path = Config.AppSettings.KEY_FOLDERLOG + Path.DirectorySeparatorChar + DateTime.Now.Year.ToString() + Path.DirectorySeparatorChar + DateTime.Now.Month.ToString();
                if (!errCode.Equals(""))
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    FileStream file = new FileStream(path + Path.DirectorySeparatorChar + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_" + Guid.NewGuid().ToString() + ".json", FileMode.Create, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(file);
                    sw.Write(errCode);
                    sw.Close();
                    file.Close();
                }
            }
            catch
            {

            }
        }
    }
}

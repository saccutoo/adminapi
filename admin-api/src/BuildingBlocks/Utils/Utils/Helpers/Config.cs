using System;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    public class Config
    {
        public class AppSettings
        {
            public static string KEY_DEVICELOGIN = Helpers.GetConfig("Config:KEY_DEVICELOGIN");
            public static string KEY_CHANNEL = Helpers.GetConfig("Config:KEY_CHANNEL");
            public static string KEY_APPLICATION = Helpers.GetConfig("Config:KEY_APPLICATION"); 
            public static string KEY_FOLDERLOG = Helpers.GetConfig("Config:KEY_FOLDERLOG");
        }

        public class ConnectionDefautl
        {
            public static string HOST = Helpers.GetConfig("ConnectionDefautl:HOST");
            public static string PORT = Helpers.GetConfig("ConnectionDefautl:PORT");
            public static string SERVICENAME = Helpers.GetConfig("ConnectionDefautl:SERVICENAME");
            public static string USERNAME = Helpers.GetConfig("ConnectionDefautl:USERNAME");
            public static string PASSWORD = Helpers.GetConfig("ConnectionDefautl:PASSWORD");
            public static string KEY = Helpers.GetConfig("ConnectionDefautl:KEY");
        }
    }
}

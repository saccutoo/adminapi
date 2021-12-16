using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Utils
{
    public class LogModel<T> where T : class
    {
        public string RepositoryId { get; set; }
        public string ChannelCode { get; set; }
        public string ApplicationCode { get; set; }
        public string FucntionCode { get; set; }
        public string TypePut { get; set; }
        public string Key { get; set; }
        public string Url { get; set; }
        public string UserAgent { get; set; }
        public string IPClient { get; set; }
        public DateTime TimePush { get; set; }
        public T Data { get; set; }
        public string ExceptionMessage { get; set; }
        public string Description { get; set; }
    }

    public class LogServerVariables
    {
        public string ALL_HTTP { get; set; }
        public string ALL_RAW { get; set; }
        public string APP_POOL_ID { get; set; }
        public string APPL_MD_PATH { get; set; }
        public string APPL_PHYSICAL_PATH { get; set; }
        public string AUTH_TYPE { get; set; }
        public string AUTH_USER { get; set; }
        public string CACHE_URL { get; set; }
        public string AUTH_PASSWORD { get; set; }
        public string LOGON_USER { get; set; }
        public string REMOTE_USER { get; set; }
        public string CERT_COOKIE { get; set; }
        public string CERT_FLAGS { get; set; }
        public string CERT_ISSUER { get; set; }
        public string CERT_KEYSIZE { get; set; }
        public string CERT_SECRETKEYSIZE { get; set; }
        public string CERT_SERIALNUMBER { get; set; }
        public string CERT_SERVER_ISSUER { get; set; }
        public string CERT_SERVER_SUBJECT { get; set; }
        public string CERT_SUBJECT { get; set; }
        public string CONTENT_LENGTH { get; set; }
        public string CONTENT_TYPE { get; set; }
        public string GATEWAY_INTERFACE { get; set; }
        public string HTTPS { get; set; }
        public string HTTPS_KEYSIZE { get; set; }
        public string HTTPS_SECRETKEYSIZE { get; set; }
        public string HTTPS_SERVER_ISSUER { get; set; }
        public string HTTPS_SERVER_SUBJECT { get; set; }
        public string INSTANCE_ID { get; set; }
        public string INSTANCE_META_PATH { get; set; }
        public string LOCAL_ADDR { get; set; }
        public string PATH_INFO { get; set; }
        public string PATH_TRANSLATED { get; set; }
        public string QUERY_STRING { get; set; }
        public string REMOTE_ADDR { get; set; }
        public string REMOTE_HOST { get; set; }
        public string REMOTE_PORT { get; set; }
        public string REQUEST_METHOD { get; set; }
        public string SCRIPT_NAME { get; set; }
        public string SERVER_NAME { get; set; }
        public string SERVER_PORT { get; set; }
        public string SERVER_PORT_SECURE { get; set; }
        public string SERVER_PROTOCOL { get; set; }
        public string SERVER_SOFTWARE { get; set; }
        public string URL { get; set; }
        public string HTTP_CACHE_CONTROL { get; set; }
        public string HTTP_CONNECTION { get; set; }
        public string HTTP_ACCEPT { get; set; }
        public string HTTP_ACCEPT_ENCODING { get; set; }
        public string HTTP_ACCEPT_LANGUAGE { get; set; }
        public string HTTP_COOKIE { get; set; }
        public string HTTP_HOST { get; set; }
        public string HTTP_METHOD { get; set; }
        public string HTTP_URL { get; set; }
        public string HTTP_VERSION { get; set; }
        public string HTTP_REFERER { get; set; }
        public string HTTP_USER_AGENT { get; set; }
        public string HTTP_UPGRADE_INSECURE_REQUESTS { get; set; }
        public string HTTP_SEC_FETCH_SITE { get; set; }
        public string HTTP_SEC_FETCH_MODE { get; set; }
        public string HTTP_SEC_FETCH_USER { get; set; }
        public string HTTP_SEC_FETCH_DEST { get; set; }
        public string HTTP_SEC_CH_UA { get; set; }
        public string HTTP_SEC_CH_UA_MOBILE { get; set; }
        public string SCRIPT_TRANSLATED { get; set; }
        public string SSI_EXEC_DISABLED { get; set; }
        public string UNENCODED_URL { get; set; }
        public string UNMAPPED_REMOTE_USER { get; set; }
        public string URL_PATH_INFO { get; set; }
    }
}
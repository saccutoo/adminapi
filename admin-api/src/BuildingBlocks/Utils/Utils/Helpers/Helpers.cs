using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Hosting;

namespace Utils
{

    public static class AsyncHelper
    {
        // AsyncHelper.RunSync(() => DoAsyncStuff());  
        private static readonly TaskFactory _taskFactory = new
            TaskFactory(CancellationToken.None,
                        TaskCreationOptions.None,
                        TaskContinuationOptions.None,
                        TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
            => _taskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();

        public static void RunSync(Func<Task> func)
            => _taskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
    }
    public class ObjectScore
    {
        public object Value { get; set; }
        public int Score { get; set; }
    }
    public partial class Helpers
    {
        public static bool IsValidJsonObject(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return false;
            }

            var value = stringValue.Trim();

            if (value.StartsWith("{") && value.EndsWith("}"))
            {
                try
                {
                    JToken.Parse(value);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
            }

            return false;
        }
        public static bool IsValidJsonArray(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return false;
            }

            var value = stringValue.Trim();

            if (value.StartsWith("[") && value.EndsWith("]"))
            {
                try
                {
                    JToken.Parse(value);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
            }

            return false;
        }
        public static string GetConfig(string code)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                                         .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: false)
                                                                         //.AddJsonFile($"appsettings.{AppConstants.EnvironmentName}.json", optional: false, reloadOnChange: false)
                                                                         .Build();

            var value = configuration[code];
            return value;
        }
        public static string ReOrderPemission(string permissionString)
        {

            var listPermission = permissionString.ToList();
            var nuberPermission = new List<ObjectScore>();
            foreach (var item in listPermission)
            {
                switch (item)
                {
                    case 'r':
                        nuberPermission.Add(new ObjectScore
                        {
                            Score = 1,
                            Value = item
                        });
                        break;
                    case 'w':
                        nuberPermission.Add(new ObjectScore
                        {
                            Score = 1,
                            Value = item
                        });
                        break;
                    case 'e':
                        nuberPermission.Add(new ObjectScore
                        {
                            Score = 1,
                            Value = item
                        });
                        break;
                    case 'd':
                        nuberPermission.Add(new ObjectScore
                        {
                            Score = 1,
                            Value = item
                        });
                        break;
                    case 'f':
                        nuberPermission.Add(new ObjectScore
                        {
                            Score = 1,
                            Value = item
                        });
                        break;
                    default:
                        break;
                }
            }
            nuberPermission.OrderBy(s => s.Score);
            return string.Join("", nuberPermission.Select(s => s.Value.ToString()));
        }
        private static readonly string[] VietnameseSigns = new string[]
        {
        "aAeEoOuUiIdDyY-",
        "áàạảãâấầậẩẫăắằặẳẵ",
        "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
        "éèẹẻẽêếềệểễ",
        "ÉÈẸẺẼÊẾỀỆỂỄ",
        "óòọỏõôốồộổỗơớờợởỡ",
        "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
        "úùụủũưứừựửữ",
        "ÚÙỤỦŨƯỨỪỰỬỮ",
        "íìịỉĩ",
        "ÍÌỊỈĨ",
        "đ",
        "Đ",
        "ýỳỵỷỹ",
        "ÝỲỴỶỸ",
        " "
        };
        public static string RemoveVietnameseSign(string str)
        {

            for (int i = 1; i < VietnameseSigns.Length; i++)
            {

                for (int j = 0; j < VietnameseSigns[i].Length; j++)

                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);

            }

            return str;

        }
        public static string BuildVietnameseSign(string str)
        {

            for (int i = 1; i < VietnameseSigns.Length; i++)
            {

                for (int j = 0; j < VietnameseSigns[i].Length; j++)

                    str = str.Replace(VietnameseSigns[0][i - 1], VietnameseSigns[i][j]);

            }

            return str;

        }
        /// <summary>
        /// Convert url title
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ConvertToUrlTitle(string name)
        {
            string strNewName = name;

            #region Replace unicode chars
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = name.Normalize(NormalizationForm.FormD);
            strNewName = regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
            #endregion

            #region Replace special chars
            string strSpecialString = "~\"“”#%&*:;<>?/\\{|}.+_@$^()[]`,!-'";

            foreach (char c in strSpecialString.ToCharArray())
            {
                strNewName = strNewName.Replace(c, ' ');
            }
            #endregion

            #region Replace space

            // Create the Regex.
            var r = new Regex(@"\s+");
            // Strip multiple spaces.
            strNewName = r.Replace(strNewName, @" ").Replace(" ", "-").Trim('-');

            #endregion)

            return strNewName;
        }
        /// <summary>
        /// Check if a string is a guid or not
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static bool IsGuid(string inputString)
        {
            try
            {
                var guid = new Guid(inputString);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool IsNumber(string inputString)
        {
            try
            {
                var guid = int.Parse(inputString);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static string GeneratePageUrl(string pageTitle)
        {
            var result = RemoveVietnameseSign(pageTitle);

            // Replace spaces
            result = result.Replace(" ", "-");

            // Replace double spaces
            result = result.Replace("--", "-");

            // Remove triple spaces
            result = result.Replace("---", "-");

            return result;

        }
        /// <summary>
        /// Tạo chuỗi 6 chữ số
        /// </summary>
        /// <returns></returns>
        public static string GenerateNewRandom()
        {
            Random generator = new Random();
            String r = generator.Next(0, 1000000).ToString("D6");
            if (r.Distinct().Count() == 1)
            {
                r = GenerateNewRandom();
            }
            return r;
        }
        public static string PasswordRandomString(int size, bool lowerCase)
        {
            var builder = new StringBuilder();
            var random = new Random();
            for (int i = 0; i < size; i++)
            {
                char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }
        public static string PasswordCreateSalt512()
        {
            var message = PasswordRandomString(512, false);
            return BitConverter.ToString((new SHA512Managed()).ComputeHash(Encoding.ASCII.GetBytes(message))).Replace("-", "");
        }
        public static string RandomPassword(int numericLength, int lCaseLength, int uCaseLength, int specialLength)
        {
            Random random = new Random();

            //char set random
            string PASSWORD_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";
            string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
            string PASSWORD_CHARS_NUMERIC = "1234567890";
            string PASSWORD_CHARS_SPECIAL = "!@#$%^&*()-+<>?";
            if ((numericLength + lCaseLength + uCaseLength + specialLength) < 8)
                return string.Empty;
            else
            {
                //get char
                var strNumeric = new string(Enumerable.Repeat(PASSWORD_CHARS_NUMERIC, numericLength)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                var strUper = new string(Enumerable.Repeat(PASSWORD_CHARS_UCASE, uCaseLength)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                var strSpecial = new string(Enumerable.Repeat(PASSWORD_CHARS_SPECIAL, specialLength)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                var strLower = new string(Enumerable.Repeat(PASSWORD_CHARS_LCASE, lCaseLength)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                //result : ký tự số + chữ hoa + chữ thường + các ký tự đặc biệt > 8
                var strResult = strNumeric + strUper + strSpecial + strLower;
                return strResult;
            }
        }
        public static string PasswordGenerateHmac(string clearMessage, string secretKeyString)
        {
            var encoder = new ASCIIEncoding();
            var messageBytes = encoder.GetBytes(clearMessage);
            var secretKeyBytes = new byte[secretKeyString.Length / 2];
            for (int index = 0; index < secretKeyBytes.Length; index++)
            {
                string byteValue = secretKeyString.Substring(index * 2, 2);
                secretKeyBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
            var hmacsha512 = new HMACSHA512(secretKeyBytes);
            byte[] hashValue = hmacsha512.ComputeHash(messageBytes);
            string hmac = "";
            foreach (byte x in hashValue)
            {
                hmac += String.Format("{0:x2}", x);
            }
            return hmac.ToUpper();
        }
        public static Expression<Func<T, bool>> PredicateByName<T>(string propName, object propValue)
        {
            var parameterExpression = Expression.Parameter(typeof(T));
            var propertyOrField = Expression.PropertyOrField(parameterExpression, propName);
            var binaryExpression = Expression.GreaterThan(propertyOrField, Expression.Constant(propValue));
            return Expression.Lambda<Func<T, bool>>(binaryExpression, parameterExpression);
        }
        public static string GetValueByKey(Dictionary<string, string> categoriesConstants, string key)
        {
            string value = "Không xác định";
            if (categoriesConstants != null && categoriesConstants.Count > 0)
            {
                var existConstant = categoriesConstants.TryGetValue(key, out value);
            }
            return value;
        }
        //public static string TikaExtractor(string filePath)
        //{
        //    var textExtractor = new TextExtractor();

        //    return textExtractor.Extract(filePath).Text;
        //}
        //public static string TikaExtractorFromUe(Uri uri)
        //{
        //    var textExtractor = new TextExtractor();
        //    return textExtractor.Extract(uri).Text;
        //}
    }
    public class TokenRequest
    {
        public string Token { get; set; }
        public string Password { get; set; }
    }
    public class TokenInfo
    {
        public Guid ObjectId { get; set; }
        public int Level { get; set; }
        public long Tick { get; set; }
        public DateTime DateTimeExpired { get; set; }
    }
    public static class TokenHelpers
    {
        #region basic token

        /// <summary>
        /// Tạo token theo key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string CreateBasicToken(string key)
        {
            try
            {
                string token = string.Empty;

                byte[] keyData = Encoding.UTF8.GetBytes(key);

                // Token chứa mã đối tượng tải về
                if (keyData != null) token = Convert.ToBase64String(keyData.ToArray());
                //Safe URl
                token = Base64UrlEncoder.Encode(token);
                return token;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Lấy key theo token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string GetKeyFromBasicToken(string token)
        {
            try
            {
                //Safe URl
                token = Base64UrlEncoder.Decode(token);
                string key = string.Empty;

                if (IsBase64(token))
                {
                    byte[] dataToken = Convert.FromBase64String(token);

                    if (dataToken != null) key = Encoding.UTF8.GetString(dataToken);
                }
                return key;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion 

        #region token download

        /// <summary>
        /// Tạo token chứa mã đối tượng, thời gian hiệu lực
        /// </summary>
        /// <param name="objectId">mã đối tượng</param>
        /// <param name="ticks">thời gian hiệu lực</param>
        /// <param name="keyEncrypt">khóa mã hóa</param>
        /// <returns></returns>
        public static string CreateUniqueTokenDownload(string objectId, long ticks, string keyEncrypt)
        {
            try
            {
                string token = string.Empty;

                byte[] key = Encoding.UTF8.GetBytes(objectId);
                byte[] time = Encoding.UTF8.GetBytes(ticks.ToString());

                // Token chứa thông tin thời gian hết hạn và mã đối tượng tải về
                if (time.Concat(key) != null) token = Convert.ToBase64String(key.Concat(time).ToArray());

                // Mã hóa token
                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(keyEncrypt)) token = Encrypt.EncryptText(token, keyEncrypt);
                //Safe URl
                token = Base64UrlEncoder.Encode(token);
                return token;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Lấy thời gian hết hạn theo token
        /// </summary>
        /// <param name="token">mã đối tượng</param>
        /// <param name="keyEncrypt">khóa mã hóa</param>
        /// <returns></returns>
        public static DateTime? GetDateTimeExpiredDownload(string token, string keyEncrypt)
        {
            try
            {
                //Safe URl
                token = Base64UrlEncoder.Decode(token);
                // Giải mã chuỗi token nếu dùng mã hóa
                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(keyEncrypt)) token = Encrypt.DecryptText(token, keyEncrypt);
                token = token.Replace("\0", string.Empty);
                DateTime unixYear0 = new DateTime(1970, 1, 1, 0, 0, 1);
                DateTime dateTimeExpired = DateTime.Now;

                string timeTicksExpiredString = string.Empty;

                if (IsBase64(token))
                {
                    byte[] dataToken = Convert.FromBase64String(token);
                    if (dataToken != null)
                    {
                        byte[] dataTick = new byte[dataToken.Length - 36];

                        Array.Copy(dataToken, 36, dataTick, 0, dataToken.Length - 36);
                        if (dataTick != null) timeTicksExpiredString = Encoding.UTF8.GetString(dataTick);
                        if (!string.IsNullOrEmpty(timeTicksExpiredString))
                        {
                            long ticks = long.Parse(timeTicksExpiredString);
                            dateTimeExpired = new DateTime(unixYear0.Ticks + ticks);
                        }
                    }
                    return dateTimeExpired;
                }
                return null;

            }
            catch (Exception)
            {

                throw;
            }

        }
        /// <summary>
        /// Lấy mã đối tượng theo token
        /// </summary>
        /// <param name="token">mã đối tượng</param>
        /// <param name="keyEncrypt">khóa mã hóa</param>
        /// <returns></returns>
        public static Guid GetObjectIdDownload(string token, string keyEncrypt)
        {
            try
            {
                //Safe URl
                token = Base64UrlEncoder.Decode(token);
                // Giải mã chuỗi token nếu sử dụng mã hóa
                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(keyEncrypt)) token = Encrypt.DecryptText(token, keyEncrypt);
                token = token.Replace("\0", string.Empty);
                Guid objectId = Guid.Empty;

                if (IsBase64(token))
                {
                    string objectStringId = string.Empty;
                    byte[] dataToken = Convert.FromBase64String(token);
                    byte[] dataGuid = new byte[36];
                    Array.Copy(dataToken, 0, dataGuid, 0, 36);
                    if (dataGuid != null) objectStringId = Encoding.UTF8.GetString(dataGuid);

                    if (!string.IsNullOrEmpty(objectStringId) && Helpers.IsGuid(objectStringId))
                    {
                        objectId = new Guid(objectStringId);
                    }
                }


                return objectId;
            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion

        #region token tokenInfo
        /// <summary>
        /// Tạo token
        /// </summary>
        /// <param name="tokenInfo"></param>
        /// <param name="keyEncrypt"></param>
        /// <returns></returns>
        public static string CreateToken(TokenInfo tokenInfo, string keyEncrypt)
        {
            try
            {
                string token = string.Empty;

                byte[] objectId = Encoding.UTF8.GetBytes(tokenInfo.ObjectId.ToString());
                byte[] level = Encoding.UTF8.GetBytes(tokenInfo.Level.ToString());
                byte[] tick = Encoding.UTF8.GetBytes(tokenInfo.Tick.ToString());

                // Token chứa thông tin thời gian hết hạn và mã đối tượng tải về
                if (level.Concat(objectId).Concat(tick) != null) token = Convert.ToBase64String(level.Concat(objectId).Concat(tick).ToArray());
                // Mã hóa token
                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(keyEncrypt)) token = Encrypt.EncryptText(token, keyEncrypt);
                //Safe URl
                token = Base64UrlEncoder.Encode(token);
                return token;
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// Lấy token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="keyEncrypt"></param>
        /// <returns></returns>
        public static TokenInfo GetToken(string token, string keyEncrypt)
        {
            try
            {
                //Safe URl
                token = Base64UrlEncoder.Decode(token);
                // Giải mã chuỗi token nếu dùng mã hóa
                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(keyEncrypt)) token = Encrypt.DecryptText(token, keyEncrypt);
                token = token.Replace("\0", string.Empty);
                if (IsBase64(token))
                {
                    byte[] dataToken = Convert.FromBase64String(token);
                    if (dataToken != null)
                    {
                        var result = new TokenInfo();
                        byte[] dataLevel = new byte[1];
                        Array.Copy(dataToken, 0, dataLevel, 0, 1);
                        byte[] dataGuid = new byte[36];
                        Array.Copy(dataToken, 1, dataGuid, 0, 36);
                        byte[] dataTick = new byte[dataToken.Length - 37];
                        Array.Copy(dataToken, 37, dataTick, 0, dataToken.Length - 37);
                        if (dataLevel != null && dataGuid != null && dataTick != null)
                        {
                            result.ObjectId = new Guid(Encoding.UTF8.GetString(dataGuid));
                            result.Level = Convert.ToInt16(Encoding.UTF8.GetString(dataLevel));
                            result.Tick = long.Parse(Encoding.UTF8.GetString(dataTick));
                            DateTime unixYear0 = new DateTime(1970, 1, 1, 0, 0, 1);
                            DateTime dateTimeExpired = DateTime.Now;
                            string timeTicksExpiredString = string.Empty;
                            dateTimeExpired = new DateTime(unixYear0.Ticks + result.Tick);
                            result.DateTimeExpired = dateTimeExpired;
                        }
                        return result;
                    }
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }

        }
        #endregion
        public static bool IsBase64(this string base64String)
        {
            if (base64String == null || base64String.Length == 0 || base64String.Length % 4 != 0
               || base64String.Contains(" ") || base64String.Contains("\t") || base64String.Contains("\r") || base64String.Contains("\n"))
                return false;

            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch (Exception)
            {
                // Handle the exception
            }
            return false;
        }

    }
    public static class Encrypt
    {
        #region Encrypt Function

        public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;
                    AES.Padding = PaddingMode.Zeros;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }
        public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;
                    AES.Padding = PaddingMode.Zeros;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }
        public static string DecryptText(string input, string password)
        {
            // Get the bytes of the string
            byte[] bytesToBeDecrypted = Convert.FromBase64String(input);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            string result = Encoding.UTF8.GetString(bytesDecrypted);

            return result;
        }
        public static string EncryptText(string input, string password)
        {
            // Get the bytes of the string
            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(input);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            string result = Convert.ToBase64String(bytesEncrypted);

            return result;
        }

        #endregion   
    }
    public class EncryptedString
    {
        /// <summary>
        /// Encrpyts the sourceString, returns this result as an Aes encrpyted, BASE64 encoded string
        /// </summary>
        /// <param name="plainSourceStringToEncrypt">a plain, Framework string (ASCII, null terminated)</param>
        /// <param name="passPhrase">The pass phrase.</param>
        /// <returns>
        /// returns an Aes encrypted, BASE64 encoded string
        /// </returns>
        public static string EncryptString(string plainSourceStringToEncrypt, string passPhrase)
        {
            try
            {
                //Set up the encryption objects
                using (AesCryptoServiceProvider acsp = GetProvider(Encoding.Default.GetBytes(passPhrase)))
                {
                    byte[] sourceBytes = Encoding.ASCII.GetBytes(plainSourceStringToEncrypt);
                    ICryptoTransform ictE = acsp.CreateEncryptor();

                    //Set up stream to contain the encryption
                    MemoryStream msS = new MemoryStream();

                    //Perform the encrpytion, storing output into the stream
                    CryptoStream csS = new CryptoStream(msS, ictE, CryptoStreamMode.Write);
                    csS.Write(sourceBytes, 0, sourceBytes.Length);
                    csS.FlushFinalBlock();

                    //sourceBytes are now encrypted as an array of secure bytes
                    byte[] encryptedBytes = msS.ToArray(); //.ToArray() is important, don't mess with the buffer

                    //return the encrypted bytes as a BASE64 encoded string
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
            catch
            {
                return "";
            }
        }


        /// <summary>
        /// Decrypts a BASE64 encoded string of encrypted data, returns a plain string
        /// </summary>
        /// <param name="base64StringToDecrypt">an Aes encrypted AND base64 encoded string</param>
        /// <param name="passphrase">The passphrase.</param>
        /// <returns>returns a plain string</returns>
        public static string DecryptString(string base64StringToDecrypt, string passphrase)
        {
            try
            {
                //Set up the encryption objects
                using (AesCryptoServiceProvider acsp = GetProvider(Encoding.Default.GetBytes(passphrase)))
                {
                    byte[] RawBytes = Convert.FromBase64String(base64StringToDecrypt);
                    ICryptoTransform ictD = acsp.CreateDecryptor();

                    //RawBytes now contains original byte array, still in Encrypted state

                    //Decrypt into stream
                    MemoryStream msD = new MemoryStream(RawBytes, 0, RawBytes.Length);
                    CryptoStream csD = new CryptoStream(msD, ictD, CryptoStreamMode.Read);
                    //csD now contains original byte array, fully decrypted

                    //return the content of msD as a regular string
                    return (new StreamReader(csD)).ReadToEnd();
                }
            }
            catch
            {
                return "";
            }
        }

        private static AesCryptoServiceProvider GetProvider(byte[] key)
        {
            AesCryptoServiceProvider result = new AesCryptoServiceProvider();
            result.BlockSize = 128;
            result.KeySize = 128;
            result.Mode = CipherMode.CBC;
            result.Padding = PaddingMode.PKCS7;

            result.GenerateIV();
            result.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            byte[] RealKey = GetKey(key, result);
            result.Key = RealKey;
            // result.IV = RealKey;
            return result;
        }

        private static byte[] GetKey(byte[] suggestedKey, SymmetricAlgorithm p)
        {
            byte[] kRaw = suggestedKey;
            List<byte> kList = new List<byte>();

            for (int i = 0; i < p.LegalKeySizes[0].MinSize; i += 8)
            {
                kList.Add(kRaw[(i / 8) % kRaw.Length]);
            }
            byte[] k = kList.ToArray();
            return k;
        }
    }
    public static class Security
    {
        #region Check sum
        public static class Algorithms
        {
            public static readonly HashAlgorithm MD5 = new MD5CryptoServiceProvider();
            public static readonly HashAlgorithm SHA1 = new SHA1Managed();
            public static readonly HashAlgorithm SHA256 = new SHA256Managed();
            public static readonly HashAlgorithm SHA384 = new SHA384Managed();
            public static readonly HashAlgorithm SHA512 = new SHA512Managed();
            //public static readonly HashAlgorithm RIPEMD160 = new RIPEMD160Managed();
        }
        public static string GetHashFromFile(string fileName, HashAlgorithm algorithm)
        {
            using (var stream = new BufferedStream(File.OpenRead(fileName), 100000))
            {
                return BitConverter.ToString(algorithm.ComputeHash(stream)).Replace("-", string.Empty);
            }
        }
        public static bool VerifyHashFromFile(string fileName, HashAlgorithm algorithm, string hashInput)
        {
            bool verify = false;
            string hashResult = "";

            using (var stream = new BufferedStream(File.OpenRead(fileName), 100000))
            {
                hashResult = BitConverter.ToString(algorithm.ComputeHash(stream)).Replace("-", string.Empty);
                if (hashResult.SequenceEqual(hashInput)) verify = true;
            }

            return verify;
        }
        #endregion      
    }
    public class RequestHelpers
    {
        public static IActionResult TransformData(Response data)
        {
            //var result = new ObjectResult(data) { StatusCode = (int)data.Code };
            var result = new ObjectResult(data) { StatusCode = 200 };
            return result;
        }
        public static RequestUser GetRequestInfo(HttpRequest request)
        {

            var result = new RequestUser
            {
                UserId = -1,
                UserName = string.Empty,
                PermissionToken = string.Empty
            };
            request.Headers.TryGetValue("X-UserId", out StringValues userId);
            request.Headers.TryGetValue("X-UserName", out StringValues userName);
            request.Headers.TryGetValue("X-PermissionToken", out StringValues permissionToken);
            if (!string.IsNullOrEmpty(userId) && Helpers.IsNumber(userId))
            {
                result.UserId = int.Parse(userId);
            }
            if (!string.IsNullOrEmpty(userName))
            {
                result.UserName = userName;
            }
            if (!string.IsNullOrEmpty(permissionToken))
            {
                result.PermissionToken = permissionToken;
                // Giai ma token theo khoa 
                var permissionTokenDecrypt = EncryptedString.DecryptString(permissionToken, Helpers.GetConfig("Encrypt:Key"));
                // Chuyen doi thanh model
                PermissionTokenModel permissionTokenModel = JsonConvert.DeserializeObject<PermissionTokenModel>(permissionTokenDecrypt);
                result.CurrentUserId = permissionTokenModel.UserId;
            }
            return result;
        }
    }
    public class RequestUser
    {
        public decimal UserId { get; set; }
        public decimal CurrentUserId { get; set; }
        public string UserName { get; set; }
        public string PermissionToken { get; set; }
    }

    /// <summary>
    /// Model chứa thông tin quyền hạn truy cập của 
    /// người dùng, thông tin được mã hóa và truyền lên trên header của request
    /// </summary>
    public class PermissionTokenModel
    {
        public Guid Id { get; set; }
        public decimal ExpiredAfter { get; set; }
        public DateTime ExpiredIn { get; set; }
        public decimal UserId { get; set; }
        public string UserName { get; set; }
        public List<FuncLoginModel> ListFuncPerms { get; set; }
    }

    public class FuncLoginModel
    {
        public decimal? FunctionId { get; set; }
        public string FuncPermCode { get; set; }
    }
}

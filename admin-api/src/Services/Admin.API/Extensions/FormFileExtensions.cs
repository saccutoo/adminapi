using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class FormFileExtensions
    {
        public const int FileMaximumBytes = 50 * 1024 * 1024;
        public const int IMAGE_FILE_MAX_SIZE = 2 * 1024 * 1024;
        public static bool IsPDFValid(this IFormFile postedFile)
        {
            //-------------------------------------------
            //  Check the pdf is null
            //-------------------------------------------
            if (postedFile == null)
            {
                return false;
            }
            //-------------------------------------------
            //  Check the pdf mime types
            //-------------------------------------------
            if (postedFile.ContentType.ToLower() != "application/pdf"
                && postedFile.ContentType.ToLower() != "application/octet-stream")
            {
                return false;
            }

            //-------------------------------------------
            //  Check the pdf extension
            //-------------------------------------------
            if (Path.GetExtension(postedFile.FileName).ToLower() != ".pdf")
            {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------
            try
            {
                if (!postedFile.OpenReadStream().CanRead)
                {
                    return false;
                }
                //------------------------------------------
                //check whether the pdf size exceeding the limit or not
                //------------------------------------------ 
                if (postedFile.Length > FileMaximumBytes)
                {
                    return false;
                }

                byte[] buffer = new byte[FileMaximumBytes];
                postedFile.OpenReadStream().Read(buffer, 0, FileMaximumBytes);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        public static bool IsImageValid(this IFormFile postedFile)
        {
            //-------------------------------------------
            //  Check the pdf is null
            //-------------------------------------------
            if (postedFile == null)
            {
                return false;
            }
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (postedFile.ContentType.ToLower() != "image/jpeg" && postedFile.ContentType.ToLower() != "image/bmp"
                && postedFile.ContentType.ToLower() != "image/png")
            {
                return false;
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            if (Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg" && Path.GetExtension(postedFile.FileName).ToLower() != ".png"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".jpe" && Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".bmp")
            {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------
            try
            {
                if (!postedFile.OpenReadStream().CanRead)
                {
                    return false;
                }
                //------------------------------------------
                //check whether the pdf size exceeding the limit or not
                //------------------------------------------ 
                if (postedFile.Length > IMAGE_FILE_MAX_SIZE)
                {
                    return false;
                }

                byte[] buffer = new byte[IMAGE_FILE_MAX_SIZE];
                postedFile.OpenReadStream().Read(buffer, 0, IMAGE_FILE_MAX_SIZE);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}

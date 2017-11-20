using Bango;
using System.Web.Http;
using Bango.Controllers;
using Bango.Base.List;
using Bango.Responses;
using System.Net.Http;
using System.Web.Mvc;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Text;
using Newtonsoft.Json;
using System;

namespace Test.FileUpload
{
    [AllowCrossSiteJson]
    public class FileUploadController : PrivateController
    {
        [System.Web.Mvc.HttpPost]
        public async Task<HttpResponseMessage> PostFile()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string temp_root = FileBox.GetWebAppRoot() + "temp\\" + SessionData.client_id + "\\";
            string temp_url = "//temp//" + SessionData.client_id + "//";

            if (!System.IO.Directory.Exists(temp_root))
                System.IO.Directory.CreateDirectory(temp_root);

            MultipartFormDataStreamProvider provider = new MultipartFormDataStreamProvider(temp_root);
            DynamicDictionary data = new DynamicDictionary();
            ResponseModel resp = new ResponseModel(false, data);

            try
            {
                StringBuilder sb = new StringBuilder(); // Holds the response body
                // Read the form data and return an async task.
                var bodyparts = await Request.Content.ReadAsMultipartAsync(provider);
                //rename the file to proper-name

                // This illustrates how to get the form data.
                foreach (var key in provider.FormData.AllKeys)
                {
                    foreach (var val in provider.FormData.GetValues(key))
                    {
                        data.Add(key, val);
                    }
                }

                //for logo upload
                string logo_name = data.GetValueAsString("logo_name");
                string extension = string.Empty;

                if (logo_name != "")
                {
                    #region for logo upload
                    string basePath = FileBox.GetWebAppRoot() + "filebox\\CommonInput\\logo\\" + SessionData.client_id + "\\";
                    if (!System.IO.Directory.Exists(basePath))
                        System.IO.Directory.CreateDirectory(basePath);

                    //check if file already exist and delete if exist
                    string[] Logo_path = Directory.GetFiles(basePath, logo_name + ".*");
                    string[] filepath = Directory.GetFiles(basePath, "*.*");
                    if (Logo_path.Length > 0)
                    {
                        System.IO.File.Delete(Logo_path[0]);
                    }
                    //check if file already exist and delete if exist

                    foreach (var file in provider.FileData)
                    {
                        string[] arr = file.Headers.ContentDisposition.FileName.Trim('"').Split('.');
                        if (arr.Length >= 1)
                            extension = "." + arr[arr.Length - 1];

                        System.IO.File.Move(file.LocalFileName, basePath + logo_name + extension);
                        data.Add("temp_file_url", basePath + logo_name + extension);
                    }
                    #endregion
                }
                else
                {
                    string url = HttpContext.Current.Request.Url.ToString().Substring(0, HttpContext.Current.Request.Url.ToString().IndexOf("/api/"));
                    // This illustrates how to get the file names for uploaded files.
                    string new_file_name = "";  // System.DateTime.Today.ToString("yyyymmddhhmmss") + "_" + getRandomID() + "_" + SessionData.user_id;
                    foreach (var file in provider.FileData)
                    {
                        FileInfo fileInfo = new FileInfo(file.LocalFileName);
                        string[] arr = file.Headers.ContentDisposition.FileName.Trim('"').Split('.');

                        if (arr.Length >= 1)
                            extension = "." + arr[arr.Length - 1];

                        new_file_name = fileInfo.Name;

                        if (data.ContainsKey("temp_file_path"))
                            data.SetValue("temp_file_path", temp_root + new_file_name + extension);
                        else
                            data.Add("temp_file_path", temp_root + new_file_name + extension);

                        if (data.ContainsKey("file_name"))
                            data.SetValue("file_name", new_file_name + extension);
                        else
                            data.Add("file_name", new_file_name + extension);

                        //mime_type_id
                        if (data.ContainsKey("mime_type_id"))
                            data.SetValue("mime_type_id", extension);
                        else
                            data.Add("mime_type_id", extension);

                        if (data.ContainsKey("file_size_bytes"))
                            data.SetValue("file_size_bytes", fileInfo.Length);
                        else
                            data.Add("file_size_bytes", fileInfo.Length);

                        data.Add("temp_file_url", url + temp_url + new_file_name + extension);
                        data.Add("temp_file_url", temp_url + new_file_name + extension);

                        System.IO.File.Move(file.LocalFileName, temp_root + new_file_name + extension);
                    }
                }
                resp.success = true;
                resp.message = "File uploaded successfully";
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(resp))
                };
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetFile()
        {
            int? client_id = SessionData.client_id;
            object filePaths = "";
            if (Directory.Exists($@"D:\ERP\ERP\ERP-MunTax\ERP-MunTax\ERP.Tax.EndPoint\filebox\CommonInput\logo\{client_id}") == true)
            {
                filePaths = Directory.GetFiles($@"D:\ERP\ERP\ERP-MunTax\ERP-MunTax\ERP.Tax.EndPoint\filebox\CommonInput\logo\{client_id}");
            }
            DynamicDictionary data = new DynamicDictionary();
            ResponseModel resp = new ResponseModel(false, data);
            data.Add("temp_file_url", filePaths);
            try
            {
                resp.success = true;
                resp.message = "Files successfully loaded.";
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(resp))
                };
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }


        }
    }
}
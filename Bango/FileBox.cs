using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;

namespace Bango
{
    public class FileBox
    {
        public string WebAppRoot { get; set; }
        public string AppPath { get; set; }
        public string FileBoxRootPath { get; set; }
        public string RootUrl { get; set; }
        public string TempPath { get; set; }
        public string ExportPath { get; set; }
        public string TransferPath { get; set; }
        public string WorkEfficiencyPath { get; set; }
        public string WorkEffPrintPath { get; set; }
        public string TransferRecommendPath { get; set; }
        public string PersonnelActivitiesPath { get; set; }
        public string RelativeExportFilename { get; set; }
        public string CharacterLetterPath { get; set; }
        public FileBox()
        {
            WebAppRoot = GetWebAppRoot();
            AppPath = WebAppRoot + "bin\\";

            FileBoxRootPath = WebAppRoot + "filebox\\";
            if (!System.IO.Directory.Exists(FileBoxRootPath))
                System.IO.Directory.CreateDirectory(FileBoxRootPath);
            //RootUrl = HttpContext.Current.Request.Url.AbsoluteUri.Remove(HttpContext.Current.Request.Url.AbsoluteUri.IndexOf(HttpContext.Current.Request.Url.AbsolutePath));
            //if (HttpContext.Current != null)
            //{
            //    RootUrl = HttpContext.Current.Request.Url.ToString().Substring(0, HttpContext.Current.Request.Url.ToString().IndexOf("/api/"));
            //}

            //RootUrl = System.Web.HttpContext.Current.Session["request_url"].ToString();
            TempPath = WebAppRoot + "temp\\";
            if (!System.IO.Directory.Exists(TempPath))
                System.IO.Directory.CreateDirectory(TempPath);
            ExportPath = FileBoxRootPath + "ExportHistory\\";
            if (!System.IO.Directory.Exists(ExportPath))
                System.IO.Directory.CreateDirectory(ExportPath);
            TransferPath = FileBoxRootPath + "TransferRequest\\";
            if (!System.IO.Directory.Exists(TransferPath))
                System.IO.Directory.CreateDirectory(TransferPath);
            TransferRecommendPath = FileBoxRootPath + "TransferRecommend\\";
            if (!System.IO.Directory.Exists(TransferRecommendPath))
                System.IO.Directory.CreateDirectory(TransferRecommendPath);
            WorkEfficiencyPath = FileBoxRootPath + "WorkEfficiency\\";
            if (!System.IO.Directory.Exists(WorkEfficiencyPath))
                System.IO.Directory.CreateDirectory(WorkEfficiencyPath);
            WorkEffPrintPath = FileBoxRootPath + "WorkEfficiencyPrint\\";
            if (!System.IO.Directory.Exists(WorkEffPrintPath))
                System.IO.Directory.CreateDirectory(WorkEffPrintPath);
            PersonnelActivitiesPath = FileBoxRootPath + "PersonnelActivities\\";
            if (!System.IO.Directory.Exists(PersonnelActivitiesPath))
                System.IO.Directory.CreateDirectory(PersonnelActivitiesPath);
            CharacterLetterPath = FileBoxRootPath + "CharacterLetter\\";
            if (!System.IO.Directory.Exists(CharacterLetterPath))
                System.IO.Directory.CreateDirectory(CharacterLetterPath);
        }

        public bool MoveFile(string srcFilePath, string code, string filetype, out string dbFilePath)
        {
            string destFilePath = PrepareFolder(code, filetype);
            FileInfo info = new FileInfo(srcFilePath);
            dbFilePath = string.Empty;
            if (info.Exists)
            {
                destFilePath += "/" + info.Name;// +"." + info.Extension;
                FileInfo destInfo = new FileInfo(destFilePath);
                if (destInfo.Exists)
                {
                    destInfo.Delete();
                    //destination file already exists
                    return false;
                }
                info.MoveTo(destFilePath);
                dbFilePath = code + "/" + filetype + "/" + info.Name;// +"." + info.Extension;
                return true;
            }

            //source file doesnot exists
            return false;
        }

        public string GetFileUrl(string relativePath)
        {
            return App.BaseUrl + "filebox/" + relativePath;
        }
        public string GetHistoryPdfFileName()
        {
            return RelativeExportFilename;
        }
        public string GetHistoryRelativeFilePath(string fileName)
        {
            return "ExportHistory/" + fileName;
        }
        public string GetTransferRelativeFilePath(string fileName)
        {
            return "TransferRequest/" + fileName;
        }
        public string GetTransferRecommendRelativeFilePath(string fileName)
        {
            return "TransferRecommend/" + fileName;
        }
        public string GetActivitiesRelativeFilePath(string fileName)
        {
            return "PersonnelActivities/" + fileName;
        }
        public string GetWorkEfficiencyFilePath(string fileName)
        {
            return "WorkEfficiency/" + fileName;
        }
        public string GetWorkEffPrintFilePath(string fileName)
        {
            return "WorkEfficiencyPrint/" + fileName;
        }
        public string GetCharacterLetterFilePath(string fileName)
        {
            return "CharacterLetter/" + fileName;
        }
        public string PrepareFolder(string code, string filetype)
        {
            if (!Directory.Exists(FileBoxRootPath))
                Directory.CreateDirectory(FileBoxRootPath);
            if (!Directory.Exists(FileBoxRootPath + code))
                Directory.CreateDirectory(FileBoxRootPath + code);
            if (!Directory.Exists(FileBoxRootPath + code + "/" + filetype))
                Directory.CreateDirectory(FileBoxRootPath + code + "/" + filetype);
            return FileBoxRootPath + code + "/" + filetype;
        }

        public string PrepareTempFolder(string type)
        {
            if (!Directory.Exists(TempPath))
                Directory.CreateDirectory(TempPath);
            if (!Directory.Exists(TempPath + type))
                Directory.CreateDirectory(TempPath + type);
            return TempPath + type;
        }
        public string PrepareExportFolder(string type)
        {
            if (!Directory.Exists(ExportPath))
                Directory.CreateDirectory(ExportPath);
            if (!Directory.Exists(ExportPath + type))
                Directory.CreateDirectory(ExportPath + type);
            return ExportPath + type;
        }
        public string GetExportFileName(string type, string fileNameInitial, string extension, string exportType)
        {
            string newpath = string.Empty;
            if (exportType == "e")
            {
                newpath = ExportPath;
            }
            else if (exportType == "t")
            {
                newpath = TransferPath;
            }
            else if (exportType == "tr")
            {
                newpath = TransferRecommendPath;
            }
            else if (exportType == "w")
            {
                newpath = WorkEfficiencyPath;
            }
            else if (exportType == "we")
            {
                newpath = WorkEffPrintPath;
            }
            else if (exportType == "pa")
            {
                newpath = PersonnelActivitiesPath;
            }
            else if (exportType == "cl")
            {
                newpath = CharacterLetterPath;
            }
            if (!Directory.Exists(newpath))
                Directory.CreateDirectory(newpath);
            if (!Directory.Exists(newpath + type))
                Directory.CreateDirectory(newpath + type);

            string filename = fileNameInitial + DateTime.Now.ToString("yyyMMdd_hhmmss_fffff") + "." + extension;
            RelativeExportFilename = filename;
            string path = newpath + type + "/" + filename;
            return path;
        }

        public string GetTempFileName(string type, string fileNameInitial, string extension)
        {
            if (!Directory.Exists(TempPath))
                Directory.CreateDirectory(TempPath);
            if (!Directory.Exists(TempPath + type))
                Directory.CreateDirectory(TempPath + type);
            string path = TempPath + type + "/" + fileNameInitial + DateTime.Now.ToString("yyyMMdd_hhmmss_fffff") + "." + extension;
            return path;
        }

        /// <summary>
        /// Saves post data in a file & return file name in which the content is saved.
        /// </summary>
        /// <param name="postData"></param>
        /// <returns>File name</returns>
        public string SavePostData(string postData)
        {
            if (!Directory.Exists(TempPath))
                Directory.CreateDirectory(TempPath);
            if (!Directory.Exists(TempPath + "post_data"))
                Directory.CreateDirectory(TempPath + "post_data");
            string file = GetFileName(TempPath + "post_data", "post_data", "json");
            try
            {
                File.WriteAllText(file, postData);
                return file;
            }
            catch (Exception ex)
            {
            }
            return "NG";
        }

        public static string GetAdDateFile(int year)
        {
            FileBox fb = new FileBox();
            if (!Directory.Exists(fb.WebAppRoot + "DateMap\\"))
                Directory.CreateDirectory(fb.WebAppRoot + "DateMap\\");
            if (!Directory.Exists(fb.WebAppRoot + "DateMap\\AD\\"))
                Directory.CreateDirectory(fb.WebAppRoot + "DateMap\\AD\\");
            return fb.WebAppRoot + "DateMap\\AD\\" + year.ToString() + ".csv";
        }

        public static string GetBsDateJsFile(string path)
        {
            FileBox fb = new FileBox();
            if (!Directory.Exists(fb.WebAppRoot + "DateMap\\"))
                Directory.CreateDirectory(fb.WebAppRoot + "DateMap\\");
            return fb.WebAppRoot + "DateMap\\" + path.ToString() + ".js";
        }
        public static string GetBsDateFile(int year)
        {
            FileBox fb = new FileBox();
            if (!Directory.Exists(fb.WebAppRoot + "DateMap\\"))
                Directory.CreateDirectory(fb.WebAppRoot + "DateMap\\");
            if (!Directory.Exists(fb.WebAppRoot + "DateMap\\BS\\"))
                Directory.CreateDirectory(fb.WebAppRoot + "DateMap\\BS\\");
            return fb.WebAppRoot + "DateMap\\BS\\" + year.ToString() + ".csv";
        }

        public string GetFileName(string path, string fileNameInitial, string extension)
        {
            if (!(path.EndsWith("/") || path.EndsWith(@"\")))
                path += "\\";
            return path + fileNameInitial + DateTime.Now.ToString("yyyMMdd_hhmmss_fffff") + "." + extension;
        }

        public static string PmisExportAppPath
        {
            get
            {
                FileBox fb = new FileBox();
                return fb.AppPath + "pmis.export.exe";
            }
        }

        public static string BinPath
        {
            get
            {
                return GetWebAppRoot() + "bin\\";
            }
        }

        public static string GetWebAppRoot()
        {
            if (HttpContext.Current != null)
                return HttpContext.Current.Server.MapPath("~/");
            else
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "..\\";
                DirectoryInfo dInfo = new DirectoryInfo(path);
                return dInfo.FullName;
            }
        }

        public static string TemplateRootPath
        {
            get
            {
                return GetWebAppRoot() + "formats\\";
            }
        }

        public static string TemplateReportsPath
        {
            get
            {
                return TemplateRootPath + "reports\\";
            }
        }

        public static string TemplateExcelPath
        {
            get
            {
                return TemplateRootPath + "excel\\";
            }
        }

        public static string SystemParamFilePath
        {
            get
            {
                return GetWebAppRoot() + "config\\";
            }
        }

        public static string JsonFilePath
        {
            get
            {
                return SystemParamFilePath + "relation_settings.json";
            }
        }


        //public static string JsonCategoryFilePath
        //{
        //    get
        //    {
        //        return SystemParamFilePath + "category_setting.json";
        //    }
        //}
        public static string SystemLangFilePath
        {
            get
            {
                return GetWebAppRoot() + "Lang\\";
            }
        }

        public static string NpLangFilePath
        {
            get
            {
                return SystemLangFilePath + "np\\";
            }
        }

        public static string JsonCategoryFilePath
        {
            get
            {
                return NpLangFilePath + "institute_category.json";
            }
        }
        public static string JsonAuthFilePath
        {
            get
            {
                return NpLangFilePath + "Auth_type.json";
            }
        }
    }
}

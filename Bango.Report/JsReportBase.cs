using jsreport.Client;
using jsreport.Client.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Bango.Base.List;

namespace Bango.Report
{
    public class JsReportBase : IReportBase
    {
        public delegate DynamicDictionary loadSettingDelegate();
        public string ReportServiceUrl
        {
            get
            {
                return JsReportConfig.JsReportServiceUrl;
            }
        }
        public static string appBasePath
        {
            get
            {
                return FileBox.GetWebAppRoot();
            }
        }
        public string Error { get; set; }
        public int Timeout { get; set; } = 1;
        public string ReportBodyTemplatePath { get; set; }
        public string ReportHeaderTemplatePath { get; set; }
        public string ReportFooterTemplatePath { get; set; }
        public string PageFooterTemplatePath { get; set; }
        public string PageHeaderTemplatePath { get; set; }
        public string ReportHelperPath { get; set; }
        public string GridHeaderJsonPath { get; set; }
        public string RenderEngine { get; set; } = "handlebars";
        public string OutputRecepie { get; set; } = "phantom-pdf";
        public string ExportRecepie { get; set; } = "html-to-xlsx";
        public string orientation { get; set; } = "portrait";
        public string headerHeight { get; set; } = "250";
        public string footerHeight { get; set; } = "120";

        DynamicDictionary _setting = null;
        //properties for office info
        public DynamicDictionary Setting
        {
            get
            {
                if (_setting == null)
                {
                    _setting = LoadSetting();
                }
                return _setting;
            }
        }
        //DynamicDictionary setting = ERP.AppSettings.Helper.Setting.LoadSetting(true);
        public string OfficeName { get; set; }
        public string OfficeAddress { get; set; }
        public string OfficeLogo { get; set; }
        public string GovernmentLogo { get; set; }
        public string TIALogo { get; set; }

        public string JsReportStoragePath
        {
            get { return JsReportConfig.JsReportStoragePath; }
        }
        public virtual string ReportStoragePath
        {
            get { return JsReportConfig.ReportOutputStoragePath; }
        }
        public string ReportUrl
        {
            get { return JsReportConfig.ReportOutputUrlBase; }
        }
        public string DestinationFileStorageDirectory { get; set; }
        public string GeneratedFilePath { get; set; }
        public string GeneratedFileUrl { get; set; }
        public string GeneratedFileName { get; set; }
        public string ReportName { get; set; } = "ReportName";
        public string ReportFileNameFormat { get; set; } = "{reportname}_{datetime}_{uniquename}";
        public object ReportData { get; set; }
        public string PermanentLink { get; set; }
        public string ActualFileName { get; set; }

        public JsReportBase()
        {
            ReportBodyTemplatePath = JsReportConfig.JsReportTemplateBasePath + @"default\body.html";
            ReportHeaderTemplatePath = JsReportConfig.JsReportTemplateBasePath + @"default\header.html";
            ReportFooterTemplatePath = JsReportConfig.JsReportTemplateBasePath + @"default\footer.html";
            ReportHelperPath = JsReportConfig.JsReportTemplateBasePath + @"default\helper.js";
            GridHeaderJsonPath = JsReportConfig.JsReportTemplateBasePath + @"default\header.json";

            string logoBasePath = appBasePath + @"\filebox\CommonInput\logo\" + SessionData.client_id + @"\";
            string[] gov_logo = Directory.Exists(logoBasePath) ? Directory.GetFiles(logoBasePath, "Government_Logo.*") : new string[1];
            string[] office_logo = Directory.Exists(logoBasePath) ? Directory.GetFiles(logoBasePath, "Municipality_Logo.*") : new string[1];
            GovernmentLogo = gov_logo[0];
            OfficeLogo = office_logo[0];
            OfficeName = Setting.GetValueAsString("office_name");
            OfficeAddress = Setting.GetValueAsString("office_address");
            TIALogo = appBasePath + @"\Reports\Templates\Image\logo.png";
        }

        public virtual Dictionary<string, object> GetOfficeInfo()
        {
            Dictionary<string, object> items = new Dictionary<string, object>();
            items.Add("officeName", OfficeName);
            items.Add("officeAddress", OfficeAddress);
            items.Add("officeLogo", OfficeLogo != null ? OfficeLogo.Replace("\\", "/") : null);
            items.Add("governmentLogo", GovernmentLogo != null ? GovernmentLogo.Replace("\\", "/") : null);
            items.Add("tialogo", TIALogo.Replace("\\", "/"));
            return items;
        }

        public virtual object FetchData()
        {
            return ReportData;
        }

        public virtual bool MoveRenameFile(jsreport.Client.Report report)
        {
            string[] fileNameArr = report.PermanentLink.Split('/');
            string jsReportFileName = string.Empty;
            string jsReportFileExtension = string.Empty;
            if (fileNameArr.Length > 0)
            {
                jsReportFileName = fileNameArr[fileNameArr.Length - 2];
                jsReportFileName += "." + report.FileExtension;
                jsReportFileExtension = report.FileExtension;
            }

            if (File.Exists(JsReportStoragePath + jsReportFileName))
            {
                GenerateFileName(jsReportFileName, jsReportFileExtension);
                File.Move(JsReportStoragePath + jsReportFileName, GeneratedFilePath);
                return true;
            }
            else
            {
                Error = "File not found";
                return false;
            }
        }

        public virtual bool GenerateFileName(string jsReportFileName, string jsReportFileExtension)
        {
            /* check if the folder exist or not then generate 
                 the fileName and fileUrl along with folder structure*/
            string fileName = string.Empty;
            string filePath = ReportStoragePath + jsReportFileExtension;
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            fileName = ReportFileNameFormat;
            fileName = fileName.Replace("{reportname}", ReportName);
            fileName = fileName.Replace("{datetime}", DateTime.Now.ToString("yyyyMMddHHmmss"));
            fileName = fileName.Replace("{uniquename}", jsReportFileName);
            GeneratedFileName = fileName;
            GeneratedFilePath = string.Format("{0}\\{1}", filePath, fileName);
            GeneratedFileUrl = string.Format("{0}\\{1}\\{2}", ReportUrl, jsReportFileExtension, fileName);
            return true;
        }

        protected virtual string GetFileContent(string path)
        {
            /*-check if the file extension is .json or any others(may be .html) then, read the file content
            -remove the unused curly braces if the extension is .Json (mostly used for header.json file)*/
            string Content = path != null ? File.ReadAllText(path) : null;
            string extension = path != null ? Path.GetExtension(path) : null;
            if (extension == ".json")
            {
                char[] escapeChars = new[] { '\n', '\r' };
                char[] firstBraces = { '{' };
                string CleanedJson = new string(Content.Where(c => !escapeChars.Contains(c)).ToArray());
                CleanedJson = CleanedJson.TrimStart(firstBraces);
                CleanedJson = CleanedJson.Remove(CleanedJson.Length - 1);
                return CleanedJson;
            }
            else
            {
                return Content;
            }
        }

        public virtual string GetBodyTemplate()
        {
            return GetFileContent(ReportBodyTemplatePath);
        }

        public virtual string GetReportFooterTemplate()
        {
            return GetFileContent(ReportFooterTemplatePath);
        }
        public virtual string GetReportHeaderTemplate()
        {
            return GetFileContent(ReportHeaderTemplatePath);
        }
        public virtual string GetScriptHelper()
        {
            return GetFileContent(ReportHelperPath);
        }
        public virtual string GetGridHeader()
        {
            return GetFileContent(GridHeaderJsonPath);
        }

        public virtual async Task<jsreport.Client.Report> RenderReport()
        {
            ReportingService _reportingService = new ReportingService(ReportServiceUrl);
            _reportingService.HttpClientTimeout = TimeSpan.FromMinutes(Timeout);
            var report = await _reportingService.RenderAsync(new RenderRequest()
            {
                template = new Template()
                {
                    recipe = OutputRecepie,
                    content = GetBodyTemplate(),
                    engine = RenderEngine,
                    helpers = GetScriptHelper(),
                    phantom = new Phantom
                    {
                        header = GetReportHeaderTemplate(),
                        headerHeight = headerHeight,
                        footer = GetReportFooterTemplate(),
                        footerHeight = footerHeight,
                        orientation = orientation,
                        margin = "5"
                    }
                },

                data = FetchData(),
                options = new RenderOptions()
                {
                    preview = true,
                    timeout = Timeout,
                    additional = new
                    {
                        reports = new { save = true },
                        script = new
                        {
                            //content = Script
                        }
                    }
                }
            });
            return report;
        }

        public virtual async Task<jsreport.Client.Report> ExportExcel()
        {
            ReportingService _reportingService = new ReportingService(ReportServiceUrl);
            _reportingService.HttpClientTimeout = TimeSpan.FromMinutes(Timeout);
            var report = await _reportingService.RenderAsync(new RenderRequest()
            {
                template = new Template()
                {
                    recipe = ExportRecepie,
                    content = GetBodyTemplate(),
                    engine = RenderEngine,
                    helpers = GetScriptHelper()
                },
                data = FetchData(),
                options = new RenderOptions()
                {
                    timeout = Timeout,
                    additional = new
                    {
                        reports = new { save = true }
                    }
                }
            });
            return report;
        }

        public virtual async Task<bool> GenerateReport(string reportType, string reportName)
        {
            jsreport.Client.Report report;
            if (reportType == "pdf")
            {
                report = await RenderReport();
            }
            else
            {
                report = await ExportExcel();
            }
            /*
            check if report is generated or not
            if report is generated the rename and move the report
                - return status
            if report is not generated return false
            */
            if (report.PermanentLink.Trim().Length > 0)
            {
                //report generated successfully  
                ReportName = reportName;
                return MoveRenameFile(report);
            }
            else
            {
                return false;
            }
        }

        public static loadSettingDelegate LoadSetting { get; set; }

    }
}
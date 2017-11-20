using Bango.Base.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Report
{
    public class JsReportConfig
    {
        static JsReportConfig()
        {
            ReportOutputUrlBase = Bango.App.BaseUrl + @"Reports/Outputs";
            loadSetting();
        }

        private static void loadSetting()
        {
            string temp = string.Empty;
            string url = (new AppSetting<string>("JsReport:ServiceUrl")).Value,
                port = (new AppSetting<string>("JsReport:PortNo")).Value;
            //JsReportServiceUrl = (new AppSetting<string>("JsReport:ServiceUrl")).Value + (new AppSetting<string>("JsReport:PortNo")).Value;
            JsReportServiceUrl = url.Trim() + (port.Trim().Length > 0 ? ":" : string.Empty) + port;
            temp = appBasePath + (new AppSetting<string>("JsReport:StoragePath")).Value;
            JsReportStoragePath = temp.Contains(@":\") ? temp : appBasePath + temp;
            temp = (new AppSetting<string>("JsReport:TemplateBasePath")).Value;
            JsReportTemplateBasePath = temp.Contains(@":\") ? temp : appBasePath + temp;
            temp = (new AppSetting<string>("Report:OutputUrlBase")).Value;
            ReportOutputUrlBase = temp.Contains(@"://") ? temp : App.BaseUrl + temp;
            temp = (new AppSetting<string>("Report:OutputStoragePath")).Value;
            ReportOutputStoragePath = temp.Contains(@":\") ? temp : appBasePath + temp;
            ReportMoveToOutputFolder = (new AppSettingBool("Report:MoveToOutputFolder")).Value;
        }
        public static string appBasePath
        {
            get
            {
                return FileBox.GetWebAppRoot();
            }
        }
        public static string JsReportServiceUrl { get; set; }
        public static string JsReportStoragePath { get; set; }
        public static string JsReportTemplateBasePath { get; set; }
        public static bool ReportMoveToOutputFolder { get; set; }
        /// <summary>
        /// The href or url based to access the generated report
        /// </summary>
        public static string ReportOutputUrlBase { get; set; }
        /// <summary>
        /// The location where the reports will be moved after the report is generated
        /// </summary>
        public static string ReportOutputStoragePath { get; set; } = appBasePath + @"fielbox\common_ouput\";
    }
}
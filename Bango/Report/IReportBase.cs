using Bango.Base.List;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bango.Report
{
    public interface IReportBase
    {
        Task<bool> GenerateReport(string reportType, string reportName);
        string GetBodyTemplate();
        string headerHeight { get; set; }
        string footerHeight { get; set; }
        string orientation { get; set; }
        string GetGridHeader();
        string GetReportFooterTemplate();
        string GetReportHeaderTemplate();
        string ReportBodyTemplatePath { get; set; }
        string ReportHeaderTemplatePath { get; set; }
        string ReportHelperPath { get; set; }
        string ReportFooterTemplatePath { get; set; }
        string GetScriptHelper();
        object ReportData { get; set; }
        string GeneratedFileUrl { get; set; }
        string GeneratedFileName { get; set; }
        string Error { get; set; }
        Dictionary<string, object> GetOfficeInfo();
    }
}
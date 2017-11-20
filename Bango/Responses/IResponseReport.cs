using System;
namespace Bango.Responses
{
    public interface IResponseReport : IResponseBase
    {
        string report_url { get; set; }
        string report_name { get; set; }
        bool isData { get; set; }
    }
}

using System;
namespace Bango.Responses
{
    public interface IResponseBase
    {
        string error_code { get; set; }
        string message { get; set; }
        bool success { get; set; }
    }
}

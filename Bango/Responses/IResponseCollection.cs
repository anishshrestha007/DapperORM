using Bango.Base.List;
using System;
namespace Bango.Responses
{
    public interface IResponseCollection : IResponseBase
    {
        System.Collections.Generic.IEnumerable<object> data { get; set; }
        DynamicDictionary grid_total { get; set; }
        int nextPage { get; set; }
        int page { get; set; }
        int previousPage { get; set; }
        int totalPages { get; set; }
        int total { get; set; }
    }
}

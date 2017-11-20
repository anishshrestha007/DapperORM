using Bango.Models;
using System;
namespace Bango.Responses
{
    public interface IResponseTree<TNode> : IResponseBase
    {
        System.Collections.Generic.IEnumerable<TNode> children { get; set; }
        //int nextPage { get; set; }
        //int page { get; set; }
        //int previousPage { get; set; }
        //int totalPages { get; set; }
        int total { get; set; }
    }
}

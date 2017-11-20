using System;
using Bango.Responses;

namespace Bango.Rbac
{
    public interface IResponseAuth : IResponseBase
    {
        string token { get; set; }
        int? user_id { get; set; }
    }
}

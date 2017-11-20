using System;
namespace Bango.Responses
{
    public interface IResponseModel : IResponseBase
    {
        object data { get; set; }
        Bango.Base.List.DynamicDictionary validation_errors { get; set; }
    }
}

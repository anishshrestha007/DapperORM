using Bango.Base.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;
using Bango.Responses;


namespace Bango.Attributes
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ModelState.IsValid == false)
            {
                ResponseModel resp = new ResponseModel(false, null, "Data validation failed.");
                resp.validation_errors = new DynamicDictionary();
                string[] keyArr = new string[0];
                foreach (KeyValuePair<string, ModelState> state in actionContext.ModelState)
                {
                    if (state.Value.Errors.Count > 0)
                    {
                        keyArr = state.Key.Split('.');
                        resp.validation_errors.Add(keyArr[keyArr.Length - 1], state.Value.Errors[0].ErrorMessage);
                    }
                }
                MediaTypeFormatterCollection formatters = new MediaTypeFormatterCollection();
                actionContext.Response = new HttpResponseMessage();
                actionContext.Response.Content = new ObjectContent<ResponseModel>(resp, formatters.JsonFormatter);
            }
        }
    }
}

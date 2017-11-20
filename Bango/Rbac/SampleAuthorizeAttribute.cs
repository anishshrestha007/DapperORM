using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Bango.Rbac
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class SampleAuthorizeAttribute : AuthorizeOnlyAttribute
    {
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            base.HandleUnauthorizedRequest(actionContext);
        }
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            //actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.OK);
            //base.OnAuthorization(actionContext);
            Bango.HeaderData.token = "testing";
            return;
        }
        //protected override bool IsAuthorized(HttpActionContext actionContext)
        //{
        //    return true;
        //}
    }
}

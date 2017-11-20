using Bango;
using Bango.Base.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Bango.Rbac
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RightsAuthorizeAttribute : AuthorizeOnlyAttribute
    {

        public RightsAuthorizeAttribute(string rightsCode)
           : this(null, rightsCode)
        {
            oneparameter = true;
        }
        public RightsAuthorizeAttribute(string prefix, string rightsCode)
        {
            _rightsCode = rightsCode;
            _prefix = prefix;

        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (!App.CheckToken)
                return true;
            //return true;
            if (AuthenticationFromDB(actionContext, _token, _user_id) == true)
            {
                //check for valid rights
                return true;
            }
            else
                return false;
        }

    }
}
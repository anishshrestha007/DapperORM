using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Routing;

namespace Bango.Rbac
{
    // Summary:
    //     Annotates a controller with a route prefix that applies to all actions within
    //     the controller.
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RightsPrefixAttribute : Attribute
    {
        // Summary:
        //     Initializes a new instance of the System.Web.Http.RoutePrefixAttribute class.
        public RightsPrefixAttribute()
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the System.Web.Http.RoutePrefixAttribute class.
        //
        // Parameters:
        //   prefix:
        //     The route prefix for the controller.
        public RightsPrefixAttribute(string prefix)
        {
            this._prefix = prefix;
        }
        private string _prefix = string.Empty;
        // Summary:
        //     Gets the route prefix.
        public virtual string Prefix
        {
            get
            {
                return _prefix;
            }
        }
    }
}
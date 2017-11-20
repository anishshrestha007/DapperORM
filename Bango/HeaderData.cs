using Bango.Base;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace Bango
{
    public static class HeaderData
    {
        public static NameValueCollection Headers
        {
            get
            {
                return HttpContext.Current.Request.Headers;
            }
        }
        public static HttpCookieCollection Cookies
        {
            get
            {
                return HttpContext.Current.Request.Cookies;
            }
        }
        public static string token
        {
            get
            {
                return Headers.GetValues("token").FirstOrDefault();
            }
            set
            {
                Headers.Set("token", value);
            }
        }

        public static int user_id
        {
            get
            {
                return Conversion.ToInt32(Headers.GetValues("user_id").FirstOrDefault());
            }
            set
            {
                Headers.Set("user_id", Conversion.ToString(value));
            }
        }

        public static int client_id
        {
            get
            {
                return Base.Conversion.ToInt32(Headers.GetValues("client_id").FirstOrDefault());
            }
            set
            {
                Headers.Set("client_id", Conversion.ToString(value));
            }
        }

        public static string asp_net_session_id
        {
            get
            {
                if(Cookies["ASP.NET_SessionId"] != null)
                {
                    return Cookies["ASP.NET_SessionId"].Value.ToString();
                }
                return string.Empty;
            }
        }
    }
}

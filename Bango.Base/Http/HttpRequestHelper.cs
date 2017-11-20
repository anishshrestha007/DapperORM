using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace Bango.Base.Http
{
    public static class HttpRequestHelper
    {
        /// <summary>
        /// Todo:Shivashwor for there host and ip property there client comp name and client ipaddress store..
        /// </summary>
        static System_Information Sys_Info = new System_Information();
        public static string GetClientIpAddress__(this HttpRequestMessage request)
        {

            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextBase)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }

            if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                return ((RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name]).Address;
            }

            //if (request.Properties.ContainsKey("MS_OwinContext"))
            //{
            //    return ((OwinContext)request.Properties["MS_OwinContext"]).Request.RemoteIpAddress;
            //}

            return "IP Address not available";
        }
        public static string GetClientIpAddress()
        {
            string ipaddress;
            HttpRequest request = HttpContext.Current.Request;
            ipaddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (ipaddress == "" || ipaddress == null)
                ipaddress = request.ServerVariables["REMOTE_ADDR"];
            //TtxtIP.Text = ipaddress;
            return ipaddress;
            //HttpRequest request = HttpContext.Current.Request;
            //string IP4Address = String.Empty;

            //foreach (IPAddress IPA in Dns.GetHostAddresses(request.ServerVariables["REMOTE_ADDR"].ToString()))
            //{
            //    if (IPA.AddressFamily.ToString() == "InterNetwork")
            //    {
            //        IP4Address = IPA.ToString();
            //        break;
            //    }
            //}

            //if (IP4Address != String.Empty)
            //{
            //    return IP4Address;
            //}

            //foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            //{
            //    if (IPA.AddressFamily.ToString() == "InterNetwork")
            //    {
            //        IP4Address = IPA.ToString();
            //        break;
            //    }
            //}

            //return IP4Address;
        }

        public static string GetClientHostName()
        {

            //return Sys_Info.Host;
            return Dns.GetHostName();

        }

        public static string GetClientBrowser()
        {
            return HttpContext.Current.Request.Browser.Browser;
        }

    }

    public class System_Information
    {
        private string host = "";
        private string ip = "";

        #region Property
        public string Host
        {
            get { return host; }
            set { host = value; }
        }


        public string IP
        {
            get { return ip; }
            set { ip = value; }
        }
        #endregion
    }
}

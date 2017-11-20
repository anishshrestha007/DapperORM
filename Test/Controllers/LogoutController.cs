using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bango;
using Bango.Rbac;
using LightInject;
using System.Web.Security;

namespace Test.Controllers
{
    public class LogoutController:Bango.Controllers.PrivateController
    {

        public bool PostLogout()
        {
            IAuthService authSrvc = App.Container.GetInstance<IAuthService>();
            authSrvc.Logout();

            //todo:shiva 24 march 2016 
            if (HttpContext.Current.Request.Cookies.Get("token") != null)
                HttpContext.Current.Request.Cookies["token"].Value = "";

            if (HttpContext.Current.Request.Cookies.Get("user_id") != null)
                HttpContext.Current.Request.Cookies["user_id"].Value = "";

            // clean auth cookie
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, string.Empty);
            authCookie.Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Add(authCookie);

            // clean session cookie    
            HttpCookie sessionCookie = new HttpCookie("ASP.NET_SessionId", string.Empty);
            sessionCookie.Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Add(sessionCookie);

            return true;
        }
    }
}
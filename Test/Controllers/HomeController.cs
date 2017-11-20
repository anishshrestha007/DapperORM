using Bango;
using Bango.Rbac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LightInject;
using System.Web.Security;
using System.Web.Http.Cors;

namespace Test.Controllers
{
    [EnableCors(origins: "http://localhost:8100", headers: "*", methods: "*")]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        public bool Logout()
        {
            IAuthService authSrvc =  App.Container.GetInstance<Bango.Rbac.IAuthService>();
            authSrvc.Logout();

            //todo:shiva 24 march 2016 
            if (Request.Cookies.Get("token") != null)
                Request.Cookies["token"].Value = "";

            if (Request.Cookies.Get("user_id") != null)
                Request.Cookies["user_id"].Value = "";

            // clean auth cookie
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, string.Empty);
            authCookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(authCookie);

            // clean session cookie    
            HttpCookie sessionCookie = new HttpCookie("ASP.NET_SessionId", string.Empty);
            sessionCookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(sessionCookie);

            return true;
        }
    }
}

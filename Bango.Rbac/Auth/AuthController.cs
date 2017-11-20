using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bango;
using Bango.Base.List;
using Bango.Responses;
using Bango.Rbac;
using Bango.Services;
using Bango.Models;
using Bango.Controllers;
using System.Web.Http;
using LightInject;
namespace Bango.Rbac.Auth
{
    public class AuthController : AuthBaseController<AuthModel, Bango.Rbac.AuthService, int?>
    {
        public AuthController()
            : base()
        {

        }
        public ResponseAuth authenticate()
        {
            IAuthService authSrvc = App.Container.GetInstance<IAuthService>();
            return authSrvc.Authenticate(GetJosnRequestAsDictionary());
        }

        public ResponseAuth GetLogout()
        {
            IAuthService authSrvc = App.Container.GetInstance<IAuthService>();
            authSrvc.Logout();
            ResponseAuth resp = new ResponseAuth();
            resp.success = true;
            resp.message = "Logout performed successfully";
            return resp;
        }
    }
}
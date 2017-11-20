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
using LightInject;
namespace Bango.Rbac
{
    public enum AuthorizationStatus
    {
        NotLoggedIn,
        SessionExpired,
        UnAuthorized, //logged in but invalid rights
        Authorized
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AuthorizeOnlyAttribute : AuthorizeAttribute
    {
        public AuthorizeOnlyAttribute()
            : this(null, string.Empty)
        {

        }
        public AuthorizeOnlyAttribute(string rightsCode)
           : this(null, rightsCode)
        {
            oneparameter = true;
        }
        public AuthorizeOnlyAttribute(string prefix, string rightsCode)
        {
            _rightsCode = rightsCode;
            _prefix = prefix;

        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            base.HandleUnauthorizedRequest(actionContext);
        }

        protected bool oneparameter = false;
        protected string _rightsCode;
        protected string _prefix;
        protected int _user_id = 0;
        protected int _client_id = 0;
        protected string _rightName;
        protected bool session_expired = false;
        protected string _token = string.Empty;
        protected bool _isAuthorized = false;
        public AuthorizationStatus Status { get; set; } = AuthorizationStatus.UnAuthorized;
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (!App.CheckToken)
                return;
            //return;
            if (actionContext.Request.Headers.Contains("token"))
            {
                if (actionContext.Request.Headers.GetValues("token") != null)
                {
                    if (oneparameter == true)
                    {
                        if ((int)actionContext.ControllerContext.Controller.GetType().GetCustomAttributes(typeof(RightsPrefixAttribute), true).Length > 0)
                        {
                            string RightsPrefix = ((RightsPrefixAttribute)actionContext.ControllerContext.Controller.GetType().GetCustomAttributes(typeof(RightsPrefixAttribute), true)[0]).Prefix;
                            _rightName = RightsPrefix + "." + _rightsCode;
                        }
                    }
                    else
                    {
                        _rightName = _prefix + "." + _rightsCode;
                    }


                    // get value from header
                    _token = Convert.ToString(actionContext.Request.Headers.GetValues("token").FirstOrDefault());
                    _user_id = Convert.ToInt32(actionContext.Request.Headers.GetValues("user_id").FirstOrDefault());
                    //_client_id = 1;
                    //_client_id = Convert.ToInt32(actionContext.Request.Headers.GetValues("_client_id").FirstOrDefault());


                    // base.OnAuthorization(actionContext);
                    _isAuthorized = IsAuthorized(actionContext);

                    SetAuthorizationDetail(actionContext);

                    if (_isAuthorized)
                    {
                        
                        //do somethig which need to be done when authorized
                    }
                    return;
                }
            }
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            actionContext.Response.ReasonPhrase = "authentication token not found";
        }

        protected void SetAuthorizationDetail(HttpActionContext actionContext)
        {
            switch (Status)
            {
                case AuthorizationStatus.Authorized:
                  
                   SessionData.SetSessionData(_token, _user_id, "np");
                    //Bango.SessionData.client_id = _client_id;
                   SessionData.language = "np";

                    HttpContext.Current.Response.Headers.Add("token", _token);
                    HttpContext.Current.Response.Headers.Add("user_id", Base.Conversion.ToString(_user_id));
                    //HttpContext.Current.Response.Headers.Add("client_id", Base.Conversion.ToString(Bango.SessionData.client_id));
                    HttpContext.Current.Response.AddHeader("AuthenticationStatus", "Authorized");
                    break;
                case AuthorizationStatus.UnAuthorized:
                    HttpContext.Current.Response.AddHeader("token", _token);
                    HttpContext.Current.Response.AddHeader("AuthenticationStatus", "UnAuthorized");
                    HttpContext.Current.Response.AddHeader("AuthenticationMessage", "UnAuthorized Area :: Access Denied");
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                    break;
                case AuthorizationStatus.SessionExpired:
                    HttpContext.Current.Response.AddHeader("token", _token);
                    HttpContext.Current.Response.AddHeader("AuthenticationStatus", "SessionExpired");
                    HttpContext.Current.Response.AddHeader("AuthenticationMessage", "Session expired please re-login.");
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                    break;
                case AuthorizationStatus.NotLoggedIn:
                default:
                    HttpContext.Current.Response.Headers.Add("token", _token);
                    HttpContext.Current.Response.AddHeader("AuthenticationStatus", "NotLoggedIn");
                    HttpContext.Current.Response.Headers.Add("AuthenticationMessage", "Authentication Required. Please provide login to access the resources.");
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                    break;
            }
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
        protected bool AuthenticationFromSession(HttpActionContext actionContext, string token, int user_id)
        {
            bool isAuthenticated = false;

            var session = HttpContext.Current.Session;
            if (session["token"] == null)
            {
                session_expired = true;
            }

            if (session["token"].ToString() == token 
                && Int32.Parse(session["user_id"].ToString()) == user_id)
            {
                isAuthenticated = true;
            }

            int user_role = Int32.Parse(session["role_id"].ToString());
            if (isAuthenticated && user_role == 1)
            {
                isAuthenticated = true;
            }

            if (session_expired == true)
            {
                //HttpContext.Current.Response.AddHeader("token", token);
                //HttpContext.Current.Response.AddHeader("AuthenticationStatus", "NotAuthorized  or Session expired please re-login.");            
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                actionContext.Response.Headers.Add("token", token);
                actionContext.Response.Headers.Add("AuthenticationStatus", "NotAuthorized  or Session expired please re-login.");
            }

            //if (isAuthorized == false)
            //{
            //    HttpContext.Current.Response.AddHeader("token", token);
            //    HttpContext.Current.Response.AddHeader("AuthenticationStatus", "NotAuthorized");
            //    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
            //    return;
            //}
            //TODO :: need to check if the user has proper authentication or not
            //RightsRepo repo = new RightsRepo();
            //if (repo.HasRights(rightsCode, user_id))
            //{
            //    isAuthenticated = true;
            //}
            return isAuthenticated;
        }
        

        protected bool AuthenticationFromDB(HttpActionContext actionContext, string token, int user_id)
        {
            if (!App.CheckToken)
                return true;
            IAuthService authSrvc = App.Container.GetInstance<Rbac.IAuthService>();
            DynamicDictionary tokenDetail = authSrvc.GetTokenDetail(token, user_id);

            if (tokenDetail == null || tokenDetail.GetCount() == 0)
            {
                Status = AuthorizationStatus.NotLoggedIn;
                return false;
            }
            if (tokenDetail.ContainsKey("expire_datetime"))
            {

                if (!String.IsNullOrEmpty(tokenDetail["expire_datetime"].ToString()))
                {
                    DateTime expiryDate = Convert.ToDateTime(tokenDetail["expire_datetime"]);
                    DateTime current_date = DateTime.Now;
                    TimeSpan difference = expiryDate - current_date;
                    if (difference.TotalMinutes < 0)
                    {
                        Status = AuthorizationStatus.SessionExpired;
                        return false;
                    }
                    else
                    {
                        int? session_id = tokenDetail.GetValueAsInt("id");
                        _client_id = (int)tokenDetail.GetValueAsInt("client_id");
                        SessionData.client_id = _client_id;
                        DynamicDictionary data_param = new DynamicDictionary();
                        data_param.Add("expire_datetime", AuthService.GetExpirtyDateTime(DateTime.Now));
                        data_param.Add("id", session_id);
                        SessionLogService logSrvc = new SessionLogService();
                        logSrvc.Update(session_id, data_param);
                        Status = AuthorizationStatus.Authorized;
                        return true;
                    }
                }
            }
            return false;
        }


    }
}
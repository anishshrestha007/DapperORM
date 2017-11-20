﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bango;
using Bango.Base.List;
using System.Text;

namespace Bango.Rbac
{
    public class AuthService : Services.CrudService<AuthModel, int?>, IAuthService
    {
        public AuthService()
            : base()
        {
            CheckClientID = false;
        }
        public AuthorizationStatus Status { get; set; } = AuthorizationStatus.UnAuthorized;
        public virtual ResponseAuth Authenticate(DynamicDictionary data_param)
        {
            int client_id = (int)data_param.GetValueAsInt("client_id");
            string username = data_param.GetValueAsString("username");
            string password = data_param.GetValueAsString("password");
            return Authenticate(client_id, username, password);
        }
        public static string GenerateToken()
        {
            return Guid.NewGuid().ToString();
        }
        public static DateTime GetExpirtyDateTime(DateTime datetime)
        {
            return datetime.AddMinutes(30);
        }
        public virtual Rbac.IUserService<UserModel, int?> GetUserServiceInstance()
        {
            return (UserService<UserModel, int?>)App.Container.GetInstance(typeof(IUserService<UserModel, int?>));
        }
        public virtual  ResponseAuth Authenticate(int client_id, string username, string password)
        {
            //validate if data is passed with all 3 parameters.
            ResponseAuth resp = new ResponseAuth();
            if (client_id < 1)
            {
                resp.message = "Please select a Client.";
                return resp;
            }
            if (username.Trim().Length == 0)
            {
                resp.message = "Please enter a Username.";
                return resp;
            }
            if (password.Trim().Length == 0)
            {
                resp.message = "Please enter a Password.";
                return resp;
            }
            //var usermodel = Bango.Container.GetInstance<IUserModel>();
            //UserService<UserModel, int?> userSrvc = (UserService<UserModel, int?>)Bango.Container.GetInstance(typeof(IUserService<UserModel, int?>));
            var userSrvc = GetUserServiceInstance();
            using (DbConnect con = new DbConnect())
            {
                resp = userSrvc.AuthenticateUserNamePasword(con, client_id, username, password);
                //generate token
                string token = string.Empty;
                if(resp.success)
                    token = GenerateToken();
                resp.token = token;
                //save data in session & generate
                SessionLogService sessionSrvc = new SessionLogService();
                DynamicDictionary data_param = new DynamicDictionary();
                data_param.Add("client_id", client_id);
                data_param.Add("user_id", resp.user_id);
                DateTime login_datetime = DateTime.Now;
                data_param.Add("login_datetime", login_datetime);
                data_param.Add("expire_datetime", GetExpirtyDateTime(login_datetime));
                data_param.Add("token", token);

                sessionSrvc.Insert(con, data_param);
                //SessionLogModel 
            }

            return resp;
        }

        public DynamicDictionary ValidateToken(string token, int user_id)
        {
            using(DbConnect con = new DbConnect())
            {
                return ValidateToken(con, token, user_id);
            }
        }
        public DynamicDictionary ValidateToken(DbConnect con, string token, int user_id)
        {
            DynamicDictionary data = new DynamicDictionary();
            SessionLogService sessionSrvc = new SessionLogService();

            DynamicDictionary data_param = new DynamicDictionary();
            data_param.Add("token", token);
            data_param.Add("user_id", user_id);
            data = sessionSrvc.CrudRepo.Get(con, data_param);

            return data;
        }

        public DynamicDictionary GetTokenDetail(DbConnect con, string token, int user_id)
        {
            return ValidateToken(con, token, user_id);
        }
        public DynamicDictionary GetTokenDetail(string token, int user_id)
        {
            using (DbConnect con = new DbConnect())
            {
                return GetTokenDetail(con, token, user_id);
            }
        }
        public DynamicDictionary GetSessionLogDetail(DbConnect con, string token, int user_id)
        {
            return ValidateToken(con, token, user_id);
        }
        public DynamicDictionary GetSessionLogDetail(string token, int user_id)
        {
            using (DbConnect con = new DbConnect())
            {
                return GetSessionLogDetail(con, token, user_id);
            }
        }
        public void Logout()
        {
            DynamicDictionary authDetail = null;
            if(SessionData.token != null)
                if (SessionData.user_id != null)
                    authDetail = GetTokenDetail(SessionData.token, (int)SessionData.user_id);
            if(authDetail != null)
            {
                int? session_id = authDetail.GetValueAsInt("id");
                //Delete(session_id);
                SessionLogService srvc = new SessionLogService();
                srvc.Delete(session_id);
            }
            if (HttpContext.Current.Response.Cookies.AllKeys.Contains("token"))
                HttpContext.Current.Response.Cookies.Remove("token");
            if (HttpContext.Current.Response.Cookies.AllKeys.Contains("user_id"))
                HttpContext.Current.Response.Cookies.Remove("user_id");
            SessionData.Session.Clear();
        }
        public bool IsValidSession(string token, int user_id)
        { 
            using(DbConnect con = new DbConnect())
            {
                return IsValidSession(con, token, user_id);
            }
        }
        public bool IsValidSession(DbConnect con, string token, int user_id)
        {
            if (!App.CheckToken)
                return true;
            DynamicDictionary tokenDetail = GetTokenDetail(token, user_id);

            if (tokenDetail == null || tokenDetail.GetCount() == 0)
            {
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
                        SessionData.client_id = tokenDetail.GetValueAsInt("client_id");
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
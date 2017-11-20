using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Bango.Rbac.User
{
    [Bango.Rbac.RightsPrefix("rbac_user")]
    public class UserController : Bango.Controllers.CrudController<UserModel, UserService, int?>
    {
        [HttpPost]
        public Bango.Responses.ResponseCollection PostAssignedRights(string assigned_rights)
        {
            Bango.Responses.ResponseCollection resp = new Bango.Responses.ResponseCollection();
            UserService srvc = new UserService();

            resp.data = srvc.LoadAssignedRights(2);
            return resp;
        }

        [HttpPost]
        public Bango.Responses.ResponseCollection PostAssignedRoles(string assigned_roles)
        {
            Bango.Responses.ResponseCollection resp = new Bango.Responses.ResponseCollection();
            UserService srvc = new UserService();

            resp.data = srvc.LoadAssignedRoles(2);
            return resp;
        }

        [HttpPost]
        [Route("api/User/changepassword")]
        public Bango.Responses.ResponseCollection changepassword(Bango.Base.List.DynamicDictionary item)
        {
            Bango.Responses.ResponseCollection resp = new Bango.Responses.ResponseCollection();
            string clientnewpassword = Convert.ToString(item["password"]);
            string clientconfirmpassword = Convert.ToString(item["confirmpassword"]);
            if (clientnewpassword == clientconfirmpassword)
            {
                int? user_id = 0;
                bool FrmUserWin = true;
                user_id = item.GetValueAsInt("user_id");
                if (user_id == 0)
                {
                    user_id = Bango.SessionData.user_id;
                    FrmUserWin = false;
                }

                UserModel model = new UserModel();
                dynamic user_data = this.Get(user_id);
                string old_password = Convert.ToString(user_data.data.password);
                string OLD = Convert.ToString(item["oldPassword"]);
                int? session_client_id = Bango.SessionData.client_id;
                int? client_id = Convert.ToInt16(user_data.data.client_id);
                UserService us = new UserService();
                item.Add("userProfile", "false");
                if (client_id == session_client_id || session_client_id == 1)
                {
                    if (FrmUserWin == true || old_password == us.EncryptPassword(OLD))
                    {
                        this.Put(user_id, item);
                        resp.success = true;
                    }
                }
                else
                {
                    resp.message = "invilid your current password";
                    resp.success = false;
                }

            }
            return resp;
        }

        [HttpPost]
        [Route("api/User/changeprofile")]
        public Bango.Responses.ResponseCollection changeprofile(Bango.Base.List.DynamicDictionary item)
        {

            Bango.Responses.ResponseCollection resp = new Bango.Responses.ResponseCollection();
            string username_en = Convert.ToString(item["name_en"]);

            string username_np = Convert.ToString(item["name_np"]);

            string desc = Convert.ToString(item["description"]);

            string photopath = Convert.ToString(item["new_file_name"]);

            item.Add("userProfile", "true");



            int? user_id = Bango.SessionData.user_id;
            UserModel model = new UserModel();
            dynamic user_data = this.Get(user_id);

            //string servername_en = Convert.ToString(user_data.data.name_en);
            //string servername_np = Convert.ToString(user_data.data.name_np);
            //string servername_desc = Convert.ToString(user_data.data.description);

            this.Put(user_id, item);
            resp.success = true;
            return resp;


        }

    }
}
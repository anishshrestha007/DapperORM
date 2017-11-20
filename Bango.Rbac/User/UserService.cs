using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bango;
using Bango.Responses;
using Bango.Base.List;
using Bango.Repo;


namespace Bango.Rbac.User
{
    public class UserService : Bango.Rbac.UserService<UserModel, int?>
    {
        public UserService()
        {
            LoadItemAfterSave = true;
        }
        public override ResponseModel Insert(DbConnect con, DynamicDictionary item)
        {
            UserModel model = new UserModel();
            //setting the client_id before inserting record if client_id field exists.
            if (model.GetType().GetProperty("client_id") != null)
            {
                if (item.GetValueAsInt("client_id") == null)
                    item.Add("client_id", SessionData.client_id);
                else
                    item.Add("client_id", item.GetValueAsInt("client_id"));
            }
            string new_file_name = item.GetValueAsString("new_file_name");
            if (new_file_name != "")
            {
                string reltive_path = "temp/";
                item.SetValue("photo_path", reltive_path + new_file_name);
            }
            ResponseModel resp = base.Insert(con, item);
            if (resp.success)
            {
                int user_id = (int)((DynamicDictionary)resp.data).GetValueAsInt("id");
                resp.data = user_id;
                return resp;
            }
            return resp;
        }

        public override ISearchRepo<UserModel, int?> InitSearchRepo()
        {
            return new UserSerchRepo();
        }

        public override ICrudRepo<UserModel, int?> InitCrudRepo()
        {
            return new UserCrudRepo();
        }
        public override ResponseModel Update(int? id, DynamicDictionary item)
        {
            string message = string.Empty;
            bool success = false;
            object data = item;
            ResponseModel resp = new ResponseModel();
            UserModel Model = new UserModel();
            LoadItemAfterSave = true;
            using (DbConnect con = new DbConnect())
            {
                con.DB.BeginTransaction();
                try
                {
                    string new_file_name = item.GetValueAsString("new_file_name");
                    string user_file_name = item.GetValueAsString("userfilename");
                    string relative_path = "temp/";
                    if (user_file_name == null)
                    {
                        if (new_file_name != null)
                        {
                            item.SetValue("photo_path", relative_path + new_file_name);
                        }
                    }
                    else
                    {
                        if (new_file_name != "")
                        {
                            item.SetValue("photo_path", relative_path + new_file_name);
                        }
                        else
                        {
                            item.SetValue("photo_path", "");
                            string filePath = FileBox.GetWebAppRoot();
                            if (System.IO.File.Exists(filePath + user_file_name)) //if file exists than delete.
                                System.IO.File.Delete(filePath + user_file_name);
                        }
                    }
                    if (SessionData.client_id == 1)
                    {
                        CheckClientID = false;
                    }

                    resp = base.Update(id, item);

                    #region User Profile Window
                    if (resp.success && item.GetValueAsString("userProfile") == "true")
                    {
                        string file_name = item.GetValueAsString("new_file_name");
                        string filePath = FileBox.GetWebAppRoot() + "temp/";
                        int? photo_id = item.GetValueAsInt("photo_id");
                        if (file_name == "")
                        {
                            var photoPath = item.GetValueAsString("user_file_name");
                            if (photoPath == "")
                            {
                                return null;
                            }
                            if (System.IO.File.Exists(filePath + new_file_name)) //if file exists than delete.
                                System.IO.File.Delete(filePath + new_file_name);

                            int? tax_photo_id = item.GetValueAsInt("photo_id");
                            item.SetValue("photo_path", "");
                            resp = base.Update(id, item);
                        }
                        else
                        {
                            if (photo_id == 0)
                            {
                                item.SetValue("photo_path", relative_path + new_file_name);
                            }
                            else
                            {
                                item.SetValue("photo_path", relative_path + new_file_name);
                            }
                            resp = base.Update(id, item);
                        }
                    }
                    #endregion
                    else
                    {

                    }
                    if (resp.success)
                    {
                        con.DB.CommitTransaction();
                        message = "Data added successfully.";
                        success = true;
                        if (resp.success)
                        {
                            DynamicDictionary respdata = (DynamicDictionary)resp.data;
                            string confirmpassword = respdata.GetValueAsString("confirmpassword");
                            if (item.GetValueAsString("userProfile") == "" || item.GetValueAsString("userProfile") == null)
                            {
                                if (confirmpassword == "" || confirmpassword == null)
                                {
                                    int user_id = (int)((DynamicDictionary)resp.data).GetValueAsInt("id");
                                    resp.data = user_id;
                                    return resp;
                                }
                            }

                        }
                        return resp;
                    }
                    else
                    {

                        con.DB.RollbackTransaction();

                        if (resp.validation_errors.GetCount() > 0)
                            message = string.Join(",", resp.error_code);
                        else
                            message = "Data add failed, please try again later.";
                    }

                }
                catch (Exception)
                {

                    con.DB.RollbackTransaction();
                    message = "Data add failed, Rollback Transaction.";
                }


            }
            return new ResponseModel(success, item.GetValue("photo_id"), message);

        }
        public override ResponseCollection GetComboItems(DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null)
        {
            return base.GetComboItems(data_param, page, pageSize, sort_by);
        }
    }
}
using Bango;
using Bango.Helpers;
using Bango.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bango.Base.List;

namespace Bango.Rbac.Roles
{
    public class RoleSearchRepo : Bango.Repo.SearchRepo<RolesModel,int?>
    {
        //public override MyroCommand AfterBindingParameter(SearchScenario searchFor, DbConnect con, MyroCommand cmd, DynamicDictionary data_param, bool count = false, string tableAlias = null)
        //{
        //    //Myro.Base.List.DynamicDictionary sessionData = MyApp.LoadSessionDataForClient();
        //    //string role_id = sessionData.GetValueAsString("assigned_role_ids").Replace("[", "").Replace("]", "").Replace(@"""", "");
        //    User.UserService srvc = new User.UserService();
        //    string role_id = Newtonsoft.Json.JsonConvert.SerializeObject(srvc.LoadAssignedRoles(con, (int)SessionData.user_id)); ;
        //    role_id = role_id.Replace("[", "").Replace("]", "").Replace(@"""", "");

        //    string[] lst_rols = role_id.Split(',');
        //    bool rol_exists = lst_rols.Contains("1"); //not "Administrator"


        //    if (rol_exists==false) //not "Administrator"
        //    {
        //        if (role_id.Trim().Length > 0)
        //            data_param.Add("id", role_id);
        //    }

        //    MyroCommand cmd1 = cmd;

        //    if (data_param.ContainsKey("id"))
        //    {
        //        MyroCommand test = new MyroCommand(MyroCommandTypes.SqlBuilder);

        //        cmd1.Template = test.SqlBuilder.AddTemplate(string.Format(@"    
        //                    {0} 
        //                    AND c.id IN (WITH RECURSIVE roles(role_id) as (
		      //                      SELECT assigned_role_id role_id 
        //                            FROM RBAC_USER_ROLES 
        //                            where is_deleted = false AND status = true AND user_id = {1}
	       //                     UNION ALL
		      //                      SELECT c.assigned_role_id role_id
		      //                      FROM rbac_role_roles as  c			
			     //                       JOIN roles nd ON c.role_id = nd.role_id
        //                                    AND c.is_deleted = false AND c.status = true
        //                    )
        //                    SELECT DISTINCT role_id FROM roles ) ", cmd.FinalSql.Replace("@status", "true"),  SessionData.user_id ));
        //    }

        //    //Todo:Pending Task 01 Jun 2016 Shivashwor ...
        //    //cmd1 = DbServiceUtility.BindParameter(cmd, _model.GetType().GetProperty("id"), data_param, "c", SearchTypes.IN_Search); //IS NOT NULL
        //    return cmd1;
        //}

        public override BangoCommand AfterBindingParameter(SearchScenario searchFor, DbConnect con, BangoCommand cmd, DynamicDictionary data_param, bool count = false, string tableAlias = null)
        {
            //Myro.Base.List.DynamicDictionary sessionData = MyApp.LoadSessionDataForClient();
            //string role_id = sessionData.GetValueAsString("assigned_role_ids").Replace("[", "").Replace("]", "").Replace(@"""", "");
            User.UserService srvc = new User.UserService();
            string role_id = Newtonsoft.Json.JsonConvert.SerializeObject(srvc.LoadAssignedRoles(con, (int)SessionData.user_id)); ;
            role_id = role_id.Replace("[", "").Replace("]", "").Replace(@"""", "");

            string[] lst_rols = role_id.Split(',');
            bool rol_exists = lst_rols.Contains("1"); //not "Administrator"

  
            if (rol_exists == false) //not "Administrator"
            {
                if (role_id.Trim().Length > 0)
                    data_param.SetValue("id", role_id);
            }

            BangoCommand cmd1 = cmd;
            if (data_param.ContainsKey("id"))
            {
                // data_param.Remove("id");
                BangoCommand test = new BangoCommand(MyroCommandTypes.SqlBuilder);
                cmd1.Template = cmd1.SqlBuilder.AddTemplate(string.Format(@"  
                            {0} 
                            AND c.id >= (WITH RECURSIVE roles(role_id) as (
		                            SELECT assigned_role_id role_id 
                                    FROM RBAC_USER_ROLES 
                                    where is_deleted = false AND status = true AND user_id = {1}
	                            UNION ALL
		                            SELECT c.assigned_role_id role_id
		                            FROM rbac_role_roles as  c			
			                            JOIN roles nd ON c.role_id = nd.role_id
                                            AND c.is_deleted = false AND c.status = true
                            )
                            SELECT DISTINCT role_id FROM roles ) ", cmd.FinalSql.Replace("@status", "true"),  SessionData.user_id));


              ///  cmd1.FinalParameters.AddDynamicParams(cmd.FinalParameters.ParameterNames);
            }

            //Todo:Pending Task 01 Jun 2016 Shivashwor ...
            //cmd1 = DbServiceUtility.BindParameter(cmd, _model.GetType().GetProperty("id"), data_param, "c", SearchTypes.IN_Search); //IS NOT NULL
            return cmd1;
        }
    }
}
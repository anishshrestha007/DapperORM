using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bango;
using Bango.Helpers;
using Bango.Base.List;
using Dapper;
using Bango.Base;
using Bango.Base.Log;
using System.Text;

namespace Bango.Rbac
{
    public class UserService<TModel, TKey> : Services.CrudService<TModel, TKey>, IUserService<TModel, TKey>
        where TModel : class, IUserModel, new()
    {
        public string EncryptPassword(string password)
        {
            return password;
        }

        public string DecryptPassword(string password)
        {
            return password;
        }

        public bool HasRights(string rightsCode, int user_id)
        {
            return true;
        }
        public bool HasRights(string rightsCode)
        {
            return true;
        }
        public DynamicDictionary LoadRights(int user_id)
        {
            return new DynamicDictionary();
        }

        public DynamicDictionary LoadRoles(int user_id)
        {
            return new DynamicDictionary();
        }

        public ResponseAuth AuthenticateUserNamePasword(DbConnect con, int client_id, string username, string password)
        {
            ResponseAuth resp = new ResponseAuth();
            string template = @"
                SELECT id, client_id, username, password, name_en, name_np,email, status
                FROM rbac_user u
                /**where**/
                AND u.status=true AND u.is_deleted=false";
            //creating command & preparing command
            string alias = DbServiceUtility.GetTableAliasForTable("u");
            BangoCommand cmd = new BangoCommand(MyroCommandTypes.SqlBuilder);
            cmd.Template = cmd.SqlBuilder.AddTemplate(template);
            UserModel mdl = new UserModel();
            DbServiceUtility.BindDeleteParameter(cmd, mdl, alias);
            DynamicDictionary data_param = new DynamicDictionary();
            data_param.Add("client_id", client_id);
            data_param.Add("username", username);
            DbServiceUtility.BindParameters(cmd, mdl, data_param, alias, SearchTypes.Equal);

            //executing the command
            string finalSql = cmd.FinalSql;
            if (finalSql.Length > 0)
            {
                IEnumerable<SqlMapper.DapperRow> items = null;
                try
                {
                    items = con.DB.Query<SqlMapper.DapperRow>(finalSql, cmd.FinalParameters, true);
                }
                catch (Npgsql.NpgsqlException ex)
                {
                    LogTrace.WriteErrorLog(ex.ToString());
                    LogTrace.WriteDebugLog(string.Format("Select SQL which gave exception:\r{0}", ex.Routine));
                }

                Errors = con.DB.GetErros();
                if (items != null && items.Count() > 0)
                {
                    DynamicDictionary data = Conversion.ToDynamicDictionary(items.FirstOrDefault());
                    if (data.GetValueAsString("password") == EncryptPassword(password))
                    {
                        resp.success = true;
                        resp.user_id = data.GetValueAsInt("id");
                        resp.email = data.GetValueAsString("email");
                        resp.message = "Login successfull";
                    }
                    else
                        resp.message = "Username and/or Password is invalid.";
                }
                else
                {
                    if (Errors.Count > 0)
                        resp.message = "Technical Problem occurred.";
                    else
                        resp.message = "Please provide a valid Username.";
                }
            }

            return resp;
        }


        #region assigned rights & roles
        public virtual StringBuilder GetAssignedRolesQuery(int user_id)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"
SELECT ur.USER_ID, ur.ASSIGNED_ROLE_ID role_id
FROM RBAC_USER_ROLES ur 
WHERE COALESCE(ur.is_deleted, false) = false AND COALESCE(ur.status, true) = true 
    AND ur.user_id = {0}"
                , user_id);
            return sb;
        }

        #region image path
        public virtual StringBuilder GetPhotoPathQuery(int user_id)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"
SELECT pp.photo_path name_photo_url
FROM rbac_user pp 
WHERE pp.id = {0}", user_id);
            return sb;
        }
        #endregion

        #region client name
        public virtual StringBuilder GetClientNameQuery(int client_id)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"
SELECT cn.name_np client_name
FROM app_client cn 
WHERE COALESCE(cn.is_deleted, false) = false AND COALESCE(cn.status, true) = true 
AND cn.id = {0}", client_id);
            return sb;
        }
        #endregion


        public virtual StringBuilder GetAllAssignedRolesQuery(int user_id)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"
WITH RECURSIVE roles(role_id) as (
		SELECT assigned_role_id role_id 
        FROM RBAC_USER_ROLES 
        where is_deleted = false AND status = true AND user_id = {0}
	UNION ALL
		SELECT c.assigned_role_id role_id
		FROM rbac_role_roles as  c			
			JOIN roles nd ON c.role_id = nd.role_id
                AND c.is_deleted = false AND c.status = true
)
SELECT * FROM roles"
                , user_id);
            return sb;
        }

        public virtual StringBuilder GetAllAssigendRightsQuery(int user_id, string rightsCode = null)
        {
            rightsCode = rightsCode == null ? string.Empty : rightsCode;
            StringBuilder sb = new StringBuilder();
            rightsCode = Helpers.DbServiceUtility.SafeDBString(rightsCode);
            sb.AppendFormat(@"WITH assigned_roles AS(
{1})
--01) RIGHTS ASSINGED TO user
SELECT rm.id RIGHT_ID, RM.code 
--, rm.NAME_EN, rm.NAME_NP
FROM RBAC_USER_RIGHTS ur 
	INNER JOIN RBAC_RIGHTS_MASTER rm ON ur.ASSIGNED_RIGHT_ID = rm.id
WHERE  ur.is_deleted = false AND ur.status = true AND ur.USER_ID = {0}"
                , user_id, GetAllAssignedRolesQuery(user_id).ToString());
            if (rightsCode.Trim().Length > 0)
            {
                sb.AppendFormat(" AND rm.code = '{0}' ", rightsCode);
            }
            //            sb.AppendFormat(@"
            //UNION
            //--02) RIGHTS ASSINGED TO GROUP
            //SELECT rm.RIGHT_ID, RM.code
            //--, rm.NAME_EN, rm.NAME_NP
            //FROM 
            //	vw_RBAC_GROUP_USERS_final gu 
            //	INNER JOIN vw_RBAC_GROUP_RIGHTS_final gr ON GU.group_id = GR.GROUP_ID
            //	INNER JOIN vw_RBAC_RIGHTS_MASTER_final rm ON gr.ASSIGNED_RIGHT_ID = rm.RIGHT_ID
            //WHERE NVL(gu.is_deleted,0) = 0 AND NVL(gu.status,0) = 1 AND 
            //    NVL(gr.is_deleted,0) = 0 AND NVL(gr.status,0) = 1 AND GU.assigned_user_id = {0}"
            //                , user_id);
            //            if (rightsCode.Trim().Length > 0)
            //            {
            //                sb.AppendFormat(" AND rm.code = '{0}' ", rightsCode);
            //            }
            sb.Append(@"
UNION
-- 03) Rights assigned to USER->ROLES, ROLE->ROLES
-- LIST all roles assinged
-- STEP 1 > list the roles assigned to users
-- STEP 2 > using connect by prior list all the sub roles
-- STEP 3 > COMBINE RECORD
SELECT rm.id right_id, rm.code
FROM assigned_roles tab
	INNER JOIN RBAC_ROLE_RIGHTS rr ON tab.ROLE_ID = rr.ROLE_ID
	INNER JOIN RBAC_RIGHTS_MASTER rm on RR.assigned_right_id = rm.id");
            if (rightsCode.Trim().Length > 0)
            {
                sb.AppendFormat(" WHERE rm.code = '{0}' ", rightsCode);
            }
            sb.Append(" ORDER BY 2");
            return sb;
        }

        public virtual List<String> LoadAssignedRights(DbConnect con, int user_id)
        {
            IEnumerable<DynamicDictionary> list = DbServiceUtility.ExecuteList(con, GetAllAssigendRightsQuery(user_id).ToString());
            List<string> rights = new List<string>();
            if (list != null)
            {
                foreach (DynamicDictionary dict in list)
                {
                    rights.Add(dict.GetValueAsString("code"));
                }
            }

            return rights;
        }

        public virtual List<String> LoadAssignedRights(int user_id)
        {
            using (DbConnect con = new DbConnect())
            {
                return LoadAssignedRights(con, user_id);
            }
        }

        public virtual List<String> LoadAssignedRights()
        {
            return LoadAssignedRights((int)SessionData.user_id);
        }
        public virtual string LoadPhotoURL(DbConnect con, int user_id)
        {
            DynamicDictionary list = DbServiceUtility.ExecuteItem(con, GetPhotoPathQuery(user_id).ToString());
            if (list == null)
            {
                return null;
            }
            var photoPath = list.GetValueAsString("name_photo_url");
            if (photoPath == null)
            {
                return null;
            }
          
            string[] formatedPhotoPath = photoPath.Split('\\');

            string absolutePhotoPath = string.Join("/", formatedPhotoPath);

            return absolutePhotoPath;
        }
        public virtual string LoadPhotoURL(int user_id)
        {
            using (DbConnect con = new DbConnect())
            {
                return LoadPhotoURL(con, user_id);
            }
        }
        public virtual string LoadPhotoURL()
        {
            return LoadPhotoURL((int)SessionData.user_id);
        }

        //Client name start
        public virtual string LoadClientName(DbConnect con, int client_id)
        {
            DynamicDictionary list = DbServiceUtility.ExecuteItem(con, GetClientNameQuery(client_id).ToString());
            if (list == null)
            {
                return null;
            }
            var clientName = list.GetValueAsString("client_name");
            if (clientName == null)
            {
                return null;
            }

            return clientName;
        }
        public virtual string LoadClientName(int client_id)
        {
            using (DbConnect con = new DbConnect())
            {
                return LoadPhotoURL(con, client_id);
            }
        }
        public virtual string LoadClientName()
        {
            return LoadClientName((int)SessionData.client_id);
        }
        //Client Name End
        public virtual List<String> LoadAssignedRoles(DbConnect con, int user_id)
        {
            IEnumerable<DynamicDictionary> list = DbServiceUtility.ExecuteList(con, GetAllAssignedRolesQuery(user_id).ToString());
            List<string> rights = new List<string>();
            if (list != null)
            {
                foreach (DynamicDictionary dict in list)
                {
                    rights.Add(dict.GetValueAsString("role_id"));
                }
            }
            return rights;
        }
        public virtual List<String> LoadAssignedRoles(int user_id)
        {
            using (DbConnect con = new DbConnect())
            {
                return LoadAssignedRoles(con, user_id);
            }
        }
        public virtual List<String> LoadAssignedRoles()
        {
            return LoadAssignedRoles((int)SessionData.user_id);
        }
        #endregion assigned rights & roles
    }
}

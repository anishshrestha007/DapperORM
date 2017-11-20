using System.Text;
using Bango;
using Bango.Base.List;
using Bango.Helpers;
using Bango.Repo;
using Bango.Models.Attributes;

namespace Bango.Rbac.User
{
    public class UserSerchRepo : Bango.Repo.SearchRepo<UserModel, int?>
    {
        public override BangoCommand GetSearchCommand(SearchScenario scenario, DbConnect con, BangoCommand cmd,
            DynamicDictionary data_param, string selectClause, string orderByClause, int page = -1, int pageSize = 20,
            bool count = false, string tableAlias = null, string scenarioOthers = null)
        {
            if (SessionData.client_id == 1)
            {
                CheckClientID = false;
            }
            var cmd1 = base.GetSearchCommand(scenario, con, cmd, data_param, selectClause, orderByClause, page, pageSize,
                count, tableAlias, scenarioOthers);
            //assigned_roles_id1
            if (data_param.ContainsKey("assigned_roles_id1"))
            {
                var SQLl = string.Format(@"
                    SELECT  distinct  r.user_id FROM RBAC_USER_ROLES r
                    left join rbac_role_roles rr on r.assigned_role_id = rr.role_id
                    LEFT JOIN RBAC_USER_ROLES rs on rs.id = rr.assigned_role_id
                    where r.assigned_role_id={0} ", data_param.GetValueAsInt("assigned_roles_id1"));

                cmd1.SqlBuilder.Where(string.Format("  c.ID NOT IN ({0}) ", SQLl));
            }
            else if (data_param.ContainsKey("assigned_roles_id"))
            {
                var SQLl = string.Format(@"
                    SELECT  distinct  r.user_id FROM RBAC_USER_ROLES r
                    left join rbac_role_roles rr on r.assigned_role_id = rr.role_id
                    LEFT JOIN RBAC_USER_ROLES rs on rs.id = rr.assigned_role_id
                    where r.assigned_role_id={0} ", data_param.GetValueAsInt("assigned_roles_id"));

                cmd1.SqlBuilder.Where(string.Format("  c.ID IN ({0}) ", SQLl));
            }
            //assigned_rights_id1
            if (data_param.ContainsKey("assigned_rights_id1"))
            {
                var SQLl = string.Format(@"
            SELECT distinct  ur.user_id
            FROM RBAC_USER_RIGHTS ur 
	            INNER JOIN RBAC_RIGHTS_MASTER rm ON ur.ASSIGNED_RIGHT_ID = rm.id
            WHERE  ur.is_deleted = false 
            AND ur.status = true and ur.ASSIGNED_RIGHT_ID = {0} ", data_param.GetValueAsInt("assigned_rights_id1"));

                cmd1.SqlBuilder.Where(string.Format("  c.ID NOT IN ({0}) ", SQLl));
            }
            else if (data_param.ContainsKey("assigned_right_id"))
            {
                var SQLl = string.Format(@"
            SELECT distinct  ur.user_id
            FROM RBAC_USER_RIGHTS ur 
	            INNER JOIN RBAC_RIGHTS_MASTER rm ON ur.ASSIGNED_RIGHT_ID = rm.id
            WHERE  ur.is_deleted = false 
            AND ur.status = true and ur.ASSIGNED_RIGHT_ID = {0} ", data_param.GetValueAsInt("assigned_right_id"));

                cmd1.SqlBuilder.Where(string.Format("  c.ID IN ({0}) ", SQLl));
            }

            return cmd1;
        }

        protected override BangoCommand GetSearchCommandTemplate(string selectClause, bool count = false,
            string tableAlias = null)
        {
            //MyroCommand cmd = base.GetSearchCommandTemplate(selectClause, count, tableAlias);
            //return cmd;

            var _model = new UserModel();
            var Sql = new StringBuilder();
            var alias = DbServiceUtility.GetTableAliasForTable(tableAlias);
            var cmd = new BangoCommand(MyroCommandTypes.SqlBuilder);
            if (count)
            {
                return base.GetSearchCommandTemplate(selectClause, count, tableAlias);
            }
            cmd.Template = cmd.SqlBuilder.AddTemplate(string.Format(@"
                                                               SELECT c.*,
a.name_en as client_en, a.name_np as client_np, a.id as client_id
FROM rbac_user c 
LEFT JOIN app_client a 
ON c.client_id = a.id 
                                                                 /**where**/
                                                                /**orderby**/"
                , selectClause, _model.GetTableName(), alias));
            return cmd;
        }

        public override BangoCommand BeforeBindingParameter(SearchScenario searchFor, DbConnect con, BangoCommand cmd,
            DynamicDictionary data_param, bool count = false, string tableAlias = null)
        {
            return base.BeforeBindingParameter(searchFor, con, cmd, data_param, count, tableAlias);
        }

        public override BangoCommand AfterBindingParameter(SearchScenario searchFor, DbConnect con, BangoCommand cmd,
            DynamicDictionary data_param, bool count = false, string tableAlias = null)
        {
            //MyroCommand cm = null; // base.AfterBindingParameter(searchFor, con, cmd, data_param, count, tableAlias);
            //if (data_param.ContainsKey("assigned_roles_id1"))
            //    cm = DbServiceUtility.BindParameter(cmd, _model.GetType().GetProperty("assigned_roles_id"), data_param, "c", SearchTypes.NotEqual);

            return base.AfterBindingParameter(searchFor, con, cmd, data_param, count, tableAlias);
        }
        public override BangoCommand GetComboItemsCommand(DbConnect con, DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null, bool count = false)
        {
            sort_by = sort_by == null ? string.Empty : sort_by;

            ComboFieldsAttribute comboAttrib = _model.GetComoFields();

            if (sort_by.Trim().Length == 0 && comboAttrib.OrderBy.Length > 0)
                sort_by = DbServiceUtility.SetColumnAlias(TableDetail.Alias, comboAttrib.OrderBy);

            BangoCommand cmd = GetCombotItemsCommandTemplate(comboAttrib.ToString(), count, TableDetail.Alias);

            if (comboAttrib.Status != null || comboAttrib.Status.Length > 0)
                data_param.Add(comboAttrib.Status, true); //for Combo Status=True mains Active Recors load in ComboBOx

            cmd = GetSearchCommand(SearchScenario.Combo, con, cmd, data_param, comboAttrib.ToString()
                , sort_by, page, pageSize, count, TableDetail.Alias);


            //adding the query clause
            string or_query = string.Empty;
            Dapper.DynamicParameters param = new Dapper.DynamicParameters();
            return cmd;
        }
    }
}
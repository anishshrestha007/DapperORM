using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bango.Models;
using Bango.Services;
using Bango;
using Bango.Base.List;
using Bango.Repo;
using Bango.Helpers;

namespace Bango.Rbac.Roles.AssignedRoles
{
    public class AssignedRolesRepo :Bango.Repo.SearchRepo<AssignedRolesModel, int?>
    {
        protected override BangoCommand GetSearchCommandTemplate(string selectClause, bool count = false, string tableAlias = null)
        {
            //return base.GetSearchCommandTemplate(selectClause, count, tableAlias);
            AssignedRolesModel _model = new AssignedRolesModel();
            System.Text.StringBuilder Sql = new System.Text.StringBuilder();
            string alias = DbServiceUtility.GetTableAliasForTable(tableAlias);
            BangoCommand cmd = new BangoCommand(MyroCommandTypes.SqlBuilder);
            if (count)
            {
                return base.GetSearchCommandTemplate(selectClause, count, tableAlias);
            }
            else
            {
                cmd.Template = cmd.SqlBuilder.AddTemplate(string.Format(@"
                                                               SELECT 
	                                                                C .*, t1.code,
	                                                                t1.name_np,
	                                                                t1.name_en
                                                                FROM
	                                                                rbac_role_roles C
                                                                LEFT JOIN rbac_role_master t1 ON C .assigned_role_id = t1. ID 
                                                                 /**where**/
                                                                /**orderby**/"
                                             , selectClause, _model.GetTableName(), alias));
            }
            return cmd;
        }
      
    }
}
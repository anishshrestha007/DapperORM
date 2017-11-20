using Bango;
using Bango.Helpers;

namespace Bango.Rbac.Roles.AssignedRights
{
    public class assignedrightsRepo : Bango.Repo.SearchRepo<assignedrightsModel, int?>
    {
        protected override BangoCommand GetSearchCommandTemplate(string selectClause, bool count = false, string tableAlias = null)
        {
            //return base.GetSearchCommandTemplate(selectClause, count, tableAlias);
            assignedrightsModel _model = new assignedrightsModel();
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
                                                   rbac_role_rights C
                                                LEFT JOIN rbac_rights_master t1 ON C .assigned_right_id = t1. ID
                                                /**where**/ 
                                                /**orderby**/"
                                , selectClause, _model.GetTableName(), alias));
            }
            return cmd;
        }

    }
}
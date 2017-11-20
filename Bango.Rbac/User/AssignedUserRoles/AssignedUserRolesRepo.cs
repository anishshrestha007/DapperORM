using Bango;
using Bango.Base.List;
using Bango.Repo;
using Bango.Helpers;
namespace Bango.Rbac.User.AssignedUserRoles
{
    public class AssignedUserRolesRepo : Bango.Repo.SearchRepo<AssignedUserRolesModel, int?>
    {
        protected override BangoCommand GetSearchCommandTemplate(string selectClause, bool count = false, string tableAlias = null)
        {
            //return base.GetSearchCommandTemplate(selectClause, count, tableAlias);
            AssignedUserRolesModel _model = new AssignedUserRolesModel();
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
                                                SELECT c.id, t1.code, t1.name_np, t1.name_en 
                                                FROM rbac_user_roles c 
                                                left JOIN rbac_role_master t1 on (c.assigned_role_id=t1.id)
                                                /**where**/ 
                                                /**orderby**/ "
                                    , selectClause, _model.GetTableName(), alias));
            }
            return cmd;
        }
        public override BangoCommand GetSearchCommand(SearchScenario scenario, DbConnect con, BangoCommand cmd, DynamicDictionary data_param, string selectClause, string orderByClause, int page = -1, int pageSize = 20, bool count = false, string tableAlias = null, string scenarioOthers = null)
        {
            if (SessionData.client_id == 1)
            {
                CheckClientID = false;
            }
            return base.GetSearchCommand(scenario, con, cmd, data_param, selectClause, orderByClause, page, pageSize, count, tableAlias, scenarioOthers);
        }


    }


}

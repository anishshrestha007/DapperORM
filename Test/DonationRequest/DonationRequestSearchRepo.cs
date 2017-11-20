using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bango;
using Bango.Base.List;
using Bango.Helpers;
using Bango.Repo;

namespace Test.DonationRequest
{
    public class DonationRequestSearchRepo:Bango.Repo.SearchRepo<donation_request_links,int?>
    {
        public override BangoCommand AfterBindingParameter(SearchScenario searchFor, DbConnect con, BangoCommand cmd, DynamicDictionary data_param, bool count = false, string tableAlias = null)
        {
            cmd = base.AfterBindingParameter(searchFor, con, cmd, data_param, count, tableAlias);
            cmd = DbServiceUtility.BindParameters(cmd, new donation_request_links(), data_param, "r");
            return cmd;
        }
        protected override BangoCommand GetSearchCommandTemplate(string selectClause, bool count = false, string tableAlias = null)
        {
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
SELECT u.first_name || ' ' || u.last_name as full_name,u.contact_number,u.email from donation_details c
INNER  JOIN donation_request_links r on r.donation_id = c.id
INNER JOIN rbac_user u on u.id=r.requested_user_id 
        /**where**/
        "
              , selectClause, _model.GetTableName(), alias));
            }
            return cmd;
        }
    }
}
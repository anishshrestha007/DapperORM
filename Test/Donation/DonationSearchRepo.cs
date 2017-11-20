using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bango;
using Bango.Helpers;

namespace Test.Donation
{
    public class DonationSearchRepo:Bango.Repo.SearchRepo<DonationModel,int?>
    {
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
select u.first_name || ' ' || u.last_name as full_name,u.contact_number,u.email,
dt.name_np as donation_type_name,
c.* from donation_details c
LEFT  JOIN donation_request_links r on r.donation_id = c.id
LEFT JOIN rbac_user u on u.id=r.requested_user_id 
LEFT JOIN donation_type dt on dt.id = c.donation_type_id
        /**where**/
        "
              , selectClause, _model.GetTableName(), alias));
            }
            return cmd;
        }
    }
}
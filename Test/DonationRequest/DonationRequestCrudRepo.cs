using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bango;
using Bango.Base.List;
using Bango.Helpers;
using System.Reflection;

namespace Test.DonationRequest
{
    public class DonationRequestCrudRepo:Bango.Repo.CrudRepo<donation_request_links,int?>
    {
        public override BangoCommand GetItemCommand(DynamicDictionary data_param, string tableAlias = null)
        {
            BangoCommand cmd = new BangoCommand();
            donation_request_links mdl = new donation_request_links();

            tableAlias = tableAlias == null ? MainTableAlias : tableAlias;
            cmd.Template = cmd.SqlBuilder.AddTemplate(string.Format(@"
select u.first_name || ' ' || u.last_name as full_name,u.contact_number,u.email from donation_details c
INNER  JOIN donation_request_links r on r.donation_id = c.id
INNER JOIN rbac_user u on u.id=r.requested_user_id 
/**where**/
/**orderby**/"
                , mdl.GetTableName(), tableAlias));

            //PropertyInfo prop = mdl.GetKeyPropertyInfo();
            //cmd.SqlBuilder.Where(string.Format("{0}=@{0}", prop.Name), DbServiceUtility.ToDynamicParameter(prop.Name, id, DbServiceUtility.TypeMap[prop.PropertyType]));
            DbServiceUtility.BindDeleteParameter(cmd, mdl, tableAlias);
            DbServiceUtility.BindParameters(cmd, mdl, data_param, tableAlias, SearchTypes.Equal);

            if (CheckClientID)
            {
                PropertyInfo client_id = mdl.GetType().GetProperty("client_id");
                if (client_id != null)
                {
                    DbServiceUtility.BindClientIdParameter(cmd, mdl, tableAlias, DisplayMasterDataFromSystem);
                    //string col = DbServiceUtility.SetColumnAlias(tableAlias, "client_id");
                    //cmd.SqlBuilder.Where("(client_id = 1 OR client_id = @client_id)");
                    //DynamicParameters param = new DynamicParameters();
                    //param.Add("@client_id", SessionData.client_id, DbType.Int32);
                    //cmd.SqlBuilder.AddParameters(param);
                }
            }
            return cmd;
        }
    }
}
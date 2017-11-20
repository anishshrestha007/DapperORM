using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bango;
using Bango.Base.List;
using Bango.Helpers;
using System.Reflection;

namespace Bango.Rbac.User
{
    public class UserCrudRepo : Bango.Repo.CrudRepo<UserModel, int?>
    {
        public override BangoCommand GetItemCommand(DynamicDictionary data_param, string tableAlias = null)
        {
            CheckClientID = false;

            BangoCommand cmd = new BangoCommand();
            UserModel mdl = new UserModel();

            tableAlias = tableAlias ?? MainTableAlias;
            cmd.Template = cmd.SqlBuilder.AddTemplate(string.Format(@"select c.*,t1.photo_url name_photo_url,t1.id photo_id,t1.file_name userfilename
                                 from rbac_user c
                                LEFT JOIN tax_photos_files  t1 on (c.id = t1.ref_id)
/**where**/
AND t1.ref_id = @id 
/**orderby**/"
                , mdl.GetTableName(), tableAlias));

            //PropertyInfo prop = mdl.GetKeyPropertyInfo();
            //cmd.SqlBuilder.Where(string.Format("{0}=@{0}", prop.Name), DbServiceUtility.ToDynamicParameter(prop.Name, data_param.GetValueAsInt("id"), DbServiceUtility.TypeMap[prop.PropertyType]));
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
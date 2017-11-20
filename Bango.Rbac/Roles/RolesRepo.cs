using System.Collections.Generic;
using Bango;
using Bango.Base.List;
using Bango.Base.Log;
using Bango.Repo;
using Npgsql;
using Bango.Rbac.Roles;

namespace ERP.Rbac.Roles
{
    public class RolesRepo : Bango.Repo.SearchRepo<RolesModel, int?>, ISearchRepo<RolesModel, int?>
    {
        public override IEnumerable<dynamic> GetComboItems(DbConnect con, DynamicDictionary data_param, int page = -1, int pageSize = 20,
            string sort_by = null)
        {
            BangoCommand cmd = GetComboItemsCommand(con, data_param, page, 10000, sort_by, false);
            string finalSql = cmd.FinalSql;
            if (finalSql.Length > 0)
            {
                try
                {
                    return con.DB.Query<dynamic>(finalSql, cmd.FinalParameters);
                }
                catch (NpgsqlException ex)
                {
                    Errors.Add(ex.ToString());
                    LogTrace.WriteErrorLog(ex.ToString());
                    LogTrace.WriteDebugLog(string.Format("sql which gave exception:\r{0}", ex.Routine));
                    return null;
                }
            }
            throw new NoSqlStringProvidedException();
        }

    }

}
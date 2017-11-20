using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bango.Base.List;
using Bango.Helpers;
using System.Reflection;
using Bango.Models.Attributes;
using Dapper.Rainbow;
using Npgsql;
using Bango.Base.Log;
using System.Collections;
using Bango.Base;
using LightInject;
namespace Bango.Repo
{
    public enum SearchScenario
    {
        Search,
        Combo,
        GridFilter,
        TreeNode,
        ChildNodes,
        others
    }

    public class SearchRepo<TModel, TKey> : ISearchRepo<TModel, TKey>
        where TModel : class, Models.IModel, new()
    {
        public bool CheckClientID { get; set; } = true;
        public bool DisplayMasterDataFromSystem { get; set; } = false;
        public List<string> Errors { get; set; } = new List<string>();
        protected TModel _model = new TModel();
        protected TableDetailAttribute TableDetail { get; set; }
        public SearchRepo()
        {
            TableDetail = _model.GetTableDetail();
        }

        public string GetAllFields()
        {
            return GetAllFields(TableDetail.Alias);
        }

        public virtual string GetAllFields(string tableAlias, bool addAlias = true, bool addRowNum = true)
        {
            return _model.GetAllFields(tableAlias, addAlias, addRowNum);
        }
        public virtual IEnumerable<dynamic> GetComboItems(DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null)
        {
            using (DbConnect con = new DbConnect())
            {
                return GetComboItems(con, data_param, page, pageSize, sort_by);
            }
        }

        public virtual IEnumerable<dynamic> GetComboItems(DbConnect con, DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null)
        {
            BangoCommand cmd = GetComboItemsCommand(con, data_param, page, pageSize, sort_by, false);
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


        public virtual BangoCommand GetComboItemsCommand(DbConnect con, DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null, bool count = false)
        {
            sort_by = sort_by == null ? string.Empty : sort_by;

            ComboFieldsAttribute comboAttrib = _model.GetComoFields();

            if (sort_by.Trim().Length == 0 && comboAttrib.OrderBy.Length > 0)
                sort_by = DbServiceUtility.SetColumnAlias(TableDetail.Alias, comboAttrib.OrderBy);

            BangoCommand cmd = GetCombotItemsCommandTemplate(comboAttrib.ToString(), count, TableDetail.Alias);

            if (comboAttrib.Status!=null || comboAttrib.Status.Length >0)
                data_param.Add(comboAttrib.Status, true); //for Combo Status=True mains Active Recors load in ComboBOx

            cmd = GetSearchCommand(SearchScenario.Combo, con, cmd, data_param, comboAttrib.ToString()
                , sort_by, page, pageSize, count, TableDetail.Alias);


            //adding the query clause
            string or_query = string.Empty;
            Dapper.DynamicParameters param = new Dapper.DynamicParameters();
            if (data_param.ContainsKey("query"))
            {
                string query = data_param.GetValueAsString("query");
                if (query != null && query.Trim().Length > 0)
                {
                    var q = query;
                    string[] arr = q.Split('-');
                    string searchParams = arr[0].ToString().Trim();
                    //string name = arr[1].ToString();
                    AppendQueryAsOr(cmd, searchParams, new string[] { "code", "name_np", "name_en" });
                }
                else
                {
                    AppendQueryAsOr(cmd, query, new string[] { "code", "name_np", "name_en" });
                }
                //query = data_param.GetValueAsString("query");
            }

            return cmd;
        }

        public static BangoCommand AppendQueryAsOr(BangoCommand cmd, string query_data, string[] fields)
        {
            string or_query = string.Empty;
            Dapper.DynamicParameters param = new Dapper.DynamicParameters();
            query_data = query_data.ToLower();
            query_data = $"%{query_data}%";
            if (query_data?.Length > 0)
            {
                string[] arr = new string[fields.Length];
                string tmp = string.Empty;
                for (int i = 0; i < fields.Length; i++)
                {
                    arr[i] = $"LOWER({fields[i]}) like @query";
                }
                or_query = "( " + string.Join(" OR ", arr) + " )";
                //or_query = @LOWER(code) like @query OR LOWER(name_np) like @query OR LOWER(name_en) like @query)";
                param.Add("@query", query_data, System.Data.DbType.AnsiString);
                cmd.SqlBuilder.Where(or_query, param);
            }
            return cmd;
        }
        protected virtual BangoCommand GetCombotItemsCommandTemplate(string selectClause, bool count = false, string tableAlias = null)
        {
            string alias = DbServiceUtility.GetTableAliasForTable(tableAlias);
            BangoCommand cmd = new BangoCommand(MyroCommandTypes.SqlBuilder);
            if (count)
            {
                cmd.Template = cmd.SqlBuilder.AddTemplate(string.Format("SELECT count(1) total_records FROM {0} {1} /**where**/ ", _model.GetTableName(), alias));
            }
            else
            {
                cmd.Template = cmd.SqlBuilder.AddTemplate(string.Format("SELECT {0} FROM {1} {2} /**where**/ /**orderby**/"
                    , selectClause, _model.GetTableName(), alias));
            }
            return cmd;
        }
        public virtual IEnumerable<dynamic> GetGridFilterItems(DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null)
        {
            using (DbConnect con = new DbConnect())
            {
                return GetGridFilterItems(con, data_param, page, pageSize, sort_by);
            }
        }
        public virtual IEnumerable<dynamic> GetGridFilterItems(DbConnect con, DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null)
        {
            BangoCommand cmd = GetGridFilterItemsCommand(con, data_param, page, pageSize, sort_by, false);
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

        protected virtual BangoCommand GetGridFilterItemsCommandTemplate(string selectClause, bool count = false, string tableAlias = null)
        {
            string alias = DbServiceUtility.GetTableAliasForTable(tableAlias);
            BangoCommand cmd = new BangoCommand(MyroCommandTypes.SqlBuilder);
            if (count)
            {
                cmd.Template = cmd.SqlBuilder.AddTemplate(string.Format("SELECT count(1) total_records FROM {0} {1} /**where**/ ", _model.GetTableName(), alias));
            }
            else
            {
                cmd.Template = cmd.SqlBuilder.AddTemplate(string.Format("SELECT {0} FROM {1} {2} /**where**/ /**orderby**/"
                    , selectClause, _model.GetTableName(), alias));
            }
            return cmd;
        }

        public virtual BangoCommand GetGridFilterItemsCommand(DbConnect con, DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null, bool count = false)
        {
            sort_by = sort_by == null ? string.Empty : sort_by;
            ComboFieldsAttribute comboAttrib = _model.GetComoFields();
            if (sort_by.Trim().Length == 0 && comboAttrib.OrderBy.Length > 0)
            {
                sort_by = DbServiceUtility.SetColumnAlias(TableDetail.Alias, comboAttrib.OrderBy);
            }
            BangoCommand cmd = GetGridFilterItemsCommandTemplate(comboAttrib.ToString(), count, TableDetail.Alias);
            cmd = GetSearchCommand(SearchScenario.GridFilter, con, cmd, data_param, comboAttrib.ToString()
                , sort_by, page, pageSize, count, TableDetail.Alias);
            //adding the query clause
            string or_query = string.Empty;
            Dapper.DynamicParameters param = new Dapper.DynamicParameters();
            if (data_param.ContainsKey("q") || data_param.ContainsKey("query"))
            {
                string query = data_param.GetValueAsString("q");
                if (query.Trim().Length == 0)
                    query = data_param.GetValueAsString("query");
                AppendQueryAsOr(cmd, query, new string[] { "code", "name_np", "name_en" });
            }
            return cmd;
        }
        public virtual int GetGridFilterItemsCount(DynamicDictionary data_param, int page = -1, int pageSize = 20)
        {
            using (DbConnect con = new DbConnect())
            {
                return GetGridFilterItemsCount(con, data_param, page, pageSize);
            }
        }
        public virtual int GetGridFilterItemsCount(DbConnect con, DynamicDictionary data_param, int page = -1, int pageSize = 20)
        {
            BangoCommand cmd = GetGridFilterItemsCommand(con, data_param, page, pageSize, string.Empty, true);
            string finalSql = cmd.FinalSql;
            if (finalSql.Length > 0)
            {
                try
                {
                    return con.DB.Query<int>(finalSql, cmd.FinalParameters).FirstOrDefault();
                }
                catch (NpgsqlException ex)
                {
                    Errors.Add(ex.ToString());
                    LogTrace.WriteErrorLog(ex.ToString());
                    LogTrace.WriteDebugLog(string.Format("sql which gave exception:\r{0}", ex.Routine));
                    return -1;
                }
            }
            throw new NoSqlStringProvidedException();
        }

        public virtual IEnumerable<dynamic> GetSearchItems(DynamicDictionary data_param, int page, int pageSize, string sort_by = null)
        {
            using (DbConnect con = new DbConnect())
            {
                return GetSearchItems(con, data_param, page, pageSize, sort_by);
            }
        }

        public virtual IEnumerable<dynamic> GetSearchItems(DbConnect con, DynamicDictionary data_param, int page, int pageSize, string sort_by = null)
        {
            BangoCommand cmd = GetSearchItemsCommand(con, data_param, page, pageSize, sort_by, false);
            string finalSql = cmd.FinalSql;
            if (finalSql.Length > 0)
            {
                IEnumerable<dynamic> items = null;
                try
                {
                    items = con.DB.Query<dynamic>(finalSql, cmd.FinalParameters);
                }
                catch (Npgsql.NpgsqlException ex)
                {
                    LogTrace.WriteErrorLog(ex.ToString());
                    LogTrace.WriteDebugLog(string.Format("Select SQL which gave exception:\r{0}", ex.Routine));
                    return null;

                }
                //Unused code disabled...
                ////List<DynamicDictionary> ddList = new List<DynamicDictionary>();
                ////if (items != null && items.Count() > 0)
                ////{
                ////    foreach (dynamic item in items)
                ////    {
                ////        ddList.Add(Conversion.ToDynamicDictionary(item));
                ////    }
                ////}
                return items;
            }
            throw new NoSqlStringProvidedException();
        }



        public virtual BangoCommand GetSearchItemsCommand(DbConnect con, DynamicDictionary data_param, int page, int pageSize, string sort_by = null, bool count = false)
        {
            sort_by = sort_by == null ? string.Empty : sort_by;
            TableDetailAttribute tableDetail = _model.GetTableDetail();
            if (sort_by.Trim().Length == 0 && tableDetail.OrderByField!= null && tableDetail.OrderByField.Length > 0)
            {
                sort_by = DbServiceUtility.SetColumnAlias(tableDetail.Alias, tableDetail.OrderByField);
            }

            BangoCommand cmd = GetSearchCommandTemplate(_model.GetAllFields(TableDetail.Alias, false), count, TableDetail.Alias);
            return GetSearchCommand(SearchScenario.Search, con, cmd, data_param, GetAllFields()
                , sort_by, page, pageSize, count, TableDetail.Alias);
        }
        public virtual int GetSearchItemsCount(DynamicDictionary data_param, int page, int pageSize)
        {
            using (DbConnect con = new DbConnect())
            {
                return GetSearchItemsCount(con, data_param, page, pageSize);
            }
        }
        public virtual int GetSearchItemsCount(DbConnect con, DynamicDictionary data_param, int page, int pageSize)
        {
            BangoCommand cmd = GetSearchItemsCommand(con, data_param, page, pageSize, string.Empty, true);
            if (cmd.Template.RawSql.Length > 0)
            {
                try
                {
                    return con.DB.Query<int>(cmd.Template.RawSql, cmd.Template.Parameters).FirstOrDefault();
                }
                catch (Npgsql.NpgsqlException ex)
                {
                    LogTrace.WriteErrorLog(ex.ToString());
                    LogTrace.WriteDebugLog(string.Format("Select SQL which gave exception:\r{0}", ex.Routine));
                    return -1;

                }
            }
            throw new NoSqlStringProvidedException();
        }

        protected virtual BangoCommand GetSearchCommandTemplate(string selectClause, bool count = false, string tableAlias = null)
        {
            string alias = DbServiceUtility.GetTableAliasForTable(tableAlias);
            BangoCommand cmd = new BangoCommand(MyroCommandTypes.SqlBuilder);
            if (count)
            {
                cmd.Template = cmd.SqlBuilder.AddTemplate(string.Format("SELECT count(1) total_records FROM {0} {1} /**where**/ ", _model.GetTableName(), alias));
            }
            else
            {
                cmd.Template = cmd.SqlBuilder.AddTemplate(string.Format("SELECT {0} FROM {1} {2} /**where**/ /**orderby**/"
                    , selectClause, _model.GetTableName(), alias));
            }
            return cmd;
        }

        public virtual BangoCommand GetSearchCommand(SearchScenario scenario, DbConnect con, BangoCommand cmd, DynamicDictionary data_param, string selectClause, string orderByClause, int page = -1, int pageSize = 20, bool count = false, string tableAlias = null, string scenarioOthers = null)
        {
            TableDetailAttribute tableDetail = _model.GetTableDetail();
            //clear the params whic are empty or null
            List<string> keys = new List<string>(data_param.KeyList.Cast<String>());
            foreach (string key in keys)
            {
                object value = data_param.GetValue(key);
                if (value == null || data_param.GetValueAsString(key).Length == 0)
                {
                    data_param.Remove(key);
                }
            }

            //BangoCommand cmd = GetSearchCommandTemplate(selectClause, count, tableAlias);
            //cmd.Sql.AppendLine("FROM " + model.GetTableName());
            IDbExpression dbExp = App.Container.GetInstance<IDbExpression>();

            if (data_param.GetCount() == 0)
                return cmd;

            string append = DbServiceUtility.GetTableAliasForColumn(tableAlias);
            if (!(scenario == SearchScenario.TreeNode && count == false))
            {
                //check & adding delete flag check sql
                DbServiceUtility.BindDeleteParameter(cmd, _model, tableAlias);

                if (CheckClientID)
                    DbServiceUtility.BindClientIdParameter(cmd, _model, tableAlias, DisplayMasterDataFromSystem);

                //add remaining default search criteria
                
                cmd = BeforeBindingParameter(scenario, con, cmd, data_param, count, tableAlias);
                cmd = DbServiceUtility.BindParameters(cmd, _model, data_param, tableAlias);
                cmd = AfterBindingParameter(scenario, con, cmd, data_param, count, tableAlias);

                //check & adding order by clause
                if (count == false)
                {
                    cmd = BeforeBindingOrderBy(scenario, con, cmd, data_param, count, tableAlias);
                    cmd = DbServiceUtility.BindOrderBy(cmd, orderByClause);
                    cmd = AfterBindingOrderBy(scenario, con, cmd, data_param, count, tableAlias);
                    cmd = DbServiceUtility.BindPagination(cmd, page, pageSize);
                }

            }
            return cmd;
        }
        public virtual BangoCommand BeforeBindingParameter(SearchScenario searchFor, DbConnect con, BangoCommand cmd, DynamicDictionary data_param, bool count = false, string tableAlias = null)
        {
            return cmd;
        }
        public virtual BangoCommand AfterBindingParameter(SearchScenario searchFor, DbConnect con, BangoCommand cmd, DynamicDictionary data_param, bool count = false, string tableAlias = null)
        {
            return cmd;
        }

        public virtual BangoCommand BeforeBindingOrderBy(SearchScenario searchFor, DbConnect con, BangoCommand cmd, DynamicDictionary data_param, bool count = false, string tableAlias = null)
        {
            return cmd;
        }
        public virtual BangoCommand AfterBindingOrderBy(SearchScenario searchFor, DbConnect con, BangoCommand cmd, DynamicDictionary data_param, bool count = false, string tableAlias = null)
        {
            return cmd;
        }

    }
}

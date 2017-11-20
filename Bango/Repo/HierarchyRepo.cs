using Bango.Base.List;
using Bango.Base.Log;
using Bango.Helpers;
using Bango.Models;
using Bango.Models.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Repo
{
    public class HierarchyRepo<TModel, TKey, TNode> : SearchRepo<TModel, TKey>, IHierarchyRepo<TModel, TKey, TNode> 
        where TModel : class, Models.IModel, new()
        where TNode : class, ITreeNode<TNode>, new()
    {
        //public bool CheckClientID { get; set; } = true;

        protected override BangoCommand GetSearchCommandTemplate(string selectClause, bool count = false, string tableAlias = null)
        {
            string alias = DbServiceUtility.GetTableAliasForTable(tableAlias);
            BangoCommand cmd = new BangoCommand(MyroCommandTypes.SqlBuilder);
            if (count)
            {
                cmd.Template = cmd.SqlBuilder.AddTemplate(string.Format(@"
SELECT count(1) total_records 
FROM {0} {1}
/**where**/ "
                , _model.GetTableName(), alias));
            }
            else
            {
                cmd.Template = cmd.SqlBuilder.AddTemplate(string.Format(@"
SELECT {0}
FROM {1} {2} 
    LEFT JOIN {1} p on c.parent_id = p.id
/**where**/ 
/**orderby**/"
                    , selectClause, _model.GetTableName(), alias));
            }
            return cmd;
        }

        public virtual BangoCommand GetTreeNodeCommandTemplate(int start_node_id, bool count = false, string tableAlias = null, string sort_by = null)
        {
            string alias = DbServiceUtility.GetTableAliasForTable(tableAlias),
                columnAlias = DbServiceUtility.GetTableAliasForColumn(tableAlias);
            BangoCommand cmd = new BangoCommand(MyroCommandTypes.SqlBuilder);
            string start_condition = string.Empty;
            
            TModel model = new TModel();
            string secondary_condition = string.Empty;
            PropertyInfo delFlag = Models.ModelService.GetDeleteFieldProperty(model);
            if(delFlag != null)
            {
                start_condition = "c." + delFlag.Name + " = false";
            }

            PropertyInfo field_client_id = model.GetType().GetProperty("client_id");
            if(field_client_id != null)
            {
                start_condition += string.Format(" AND (c.client_id = 1 OR c.client_id = {0})", SessionData.client_id);
            }

            secondary_condition = start_condition;

            if (start_node_id == 0)
                start_condition += string.Format(" AND COALESCE(c.parent_id,0) = {0}", start_node_id);
            else
                start_condition += string.Format(" AND c.id = {0}", start_node_id);

            if (count)
            {
                cmd.Template = cmd.SqlBuilder.AddTemplate(string.Format(@"
SELECT count(1) total_records 
FROM {0} {1}
/**where**/ "
                , _model.GetTableName(), alias));
            }
            else
            {
                cmd.Template = cmd.SqlBuilder.AddTemplate(string.Format(@"
WITH RECURSIVE nodes(id, code, name_np, name_en, parent_id
        , node_path, parent_node_path, node_level) as(
	SELECT
		{3}id, {3}code, {3}name_np, {3}name_en, {3}parent_id
        --, null::text parent_code, null::text parent_name_np, null::text parent_name_en
		--, c.parent_id, p.code parent_code, p.name_np parent_name_np, p.name_en parent_name_en
		, ARRAY[{3}id], ARRAY[{3}id], 1
	FROM {0} {2}
	WHERE
		{1}
	UNION ALL

	SELECT
		{3}id, {3}code, {3}name_np, {3}name_en, {3}parent_id
        --, p.code parent_code, p.name_np parent_name_np, p.name_en parent_name_en
		, node_path || c.id, node_path, nd.node_level + 1
	FROM {0} {2}
		JOIN {0} p  ON c.parent_id = p.id
		JOIN nodes nd ON p.id = nd.id
    WHERE
        {4}
)

SELECT array_to_string(node_path , '-') node_path, array_to_string(parent_node_path , '-') parent_node_path, id node_id, code, name_np
    , name_en, parent_id, node_level , code || ' - ' || name_np || ' - ' || name_en as text
FROM nodes
order by node_path
/**where**/ 
/**orderby**/"
                , _model.GetTableName(), start_condition, alias, columnAlias, secondary_condition));
            }
            return cmd;
        }
        
        public virtual BangoCommand GetTreeNodeCommand(DbConnect con, DynamicDictionary data_param, int? start_node_id, string sort_by = null)
        {
            sort_by = sort_by == null ? string.Empty : sort_by;

            if(sort_by.Length == 0 && TableDetail.OrderByField.Length >0)
            {
                sort_by = DbServiceUtility.GetTableAliasForColumn(TableDetail.Alias) + TableDetail.OrderByField;
            }
            BangoCommand cmd = GetTreeNodeCommandTemplate(Convert.ToInt32(start_node_id), false, TableDetail.Alias, sort_by);
            return GetSearchCommand(SearchScenario.TreeNode, con, cmd, data_param, GetAllFields()
                , sort_by , -1, -1, false, TableDetail.Alias);
        }

        public virtual List<TNode> GetTreeNodes(DbConnect con, DynamicDictionary data_param, int? start_node_id, string sort_by = null)
        {
            start_node_id = start_node_id == null ? 0 : start_node_id;
            Errors.Clear();
            //List<TNode> tree = new List<TNode>();
            
            BangoCommand cmd = GetTreeNodeCommand(con, data_param, start_node_id, sort_by);
            string finalSql = cmd.FinalSql;
            if (finalSql.Length > 0)
            {
                try
                {
                    IEnumerable<TNode> list = null;
                    list = con.DB.Query<TNode>(finalSql, cmd.FinalParameters);
                    if(list != null)
                    {
                        return list.ToList<TNode>();
                    }
                    return null;
                }
                catch (Npgsql.NpgsqlException ex)
                {
                    Errors.Add(ex.Message);
                    LogTrace.WriteErrorLog(ex.ToString());
                    LogTrace.WriteDebugLog(string.Format("Select SQL which gave exception:\r{0}", ex.Routine));
                    return null;

                }
            }
            throw new NoSqlStringProvidedException();
        }

        protected virtual BangoCommand GetChildNodesCommandTemplate1(int parent_id, bool count = false, string tableAlias = null, string sort_by = null)
        {
            string alias = DbServiceUtility.GetTableAliasForTable(tableAlias),
                columnAlias = DbServiceUtility.GetTableAliasForColumn(tableAlias);
            BangoCommand cmd = new BangoCommand(MyroCommandTypes.SqlBuilder);

            if (count)
            {
                cmd.Template = cmd.SqlBuilder.AddTemplate(string.Format(@"
SELECT count(1) total_records 
FROM {0} {1}
/**where**/ "
                , _model.GetTableName(), alias));
            }
            else
            {
                cmd.Template = cmd.SqlBuilder.AddTemplate(string.Format(@"
SELECT
	{3}id, {3}code, {3}name_np, {3}name_en, {3}parent_id, {3}code || ' - ' || {3}name_np || ' - ' || {3}name_en as text, true as leaf
FROM {0} {2}
/**where**/ 
/**orderby**/"
                , _model.GetTableName(), string.Empty, alias, columnAlias));
            }
            return cmd;
        }

        public virtual BangoCommand GetChildNodesCommand(DbConnect con, DynamicDictionary data_param, int? parent_id, string sort_by = null)
        {
            sort_by = sort_by == null ? string.Empty : sort_by;
            if (sort_by.Length == 0 && TableDetail.OrderByField.Length > 0)
            {
                sort_by = DbServiceUtility.GetTableAliasForColumn(TableDetail.Alias) + TableDetail.OrderByField;
            }
            BangoCommand cmd = GetTreeNodeCommandTemplate(Convert.ToInt32(parent_id), false, TableDetail.Alias, sort_by);
            return GetSearchCommand(SearchScenario.ChildNodes, con, cmd, data_param, GetAllFields()
                , sort_by, -1, -1, false, TableDetail.Alias);
        }

        public virtual List<TNode> GetChildNodes(DbConnect con, DynamicDictionary data_param, int? parent_id, string sort_by = null)
        {
            parent_id = parent_id == null ? 0 : parent_id;
            Errors.Clear();
            //List<TNode> tree = new List<TNode>();

            BangoCommand cmd = GetChildNodesCommand(con, data_param, parent_id, sort_by);
            string finalSql = cmd.FinalSql;
            if (finalSql.Length > 0)
            {
                try
                {
                    IEnumerable<TNode> list = null;
                    list = con.DB.Query<TNode>(finalSql, cmd.FinalParameters);
                    if (list != null)
                    {
                        return list.ToList<TNode>();
                    }
                    return null;
                }
                catch (Npgsql.NpgsqlException ex)
                {
                    Errors.Add(ex.Message);
                    LogTrace.WriteErrorLog(ex.ToString());
                    LogTrace.WriteDebugLog(string.Format("Select SQL which gave exception:\r{0}", ex.Routine));
                    return null;

                }
            }
            throw new NoSqlStringProvidedException();
        }

    }


}

using System.Collections.Generic;
using Bango.Base.List;
using Bango.Models;

namespace Bango.Repo
{
    public interface IHierarchyRepo<TModel, TKey, TNode>
        where TModel : class, IModel, new()
        where TNode : class, ITreeNode<TNode>, new()
    {
        bool CheckClientID { get; set; }
        bool DisplayMasterDataFromSystem { get; set; }
        BangoCommand GetTreeNodeCommand(DbConnect con, DynamicDictionary data_param, int? start_node_id, string sort_by = null);
        List<TNode> GetTreeNodes(DbConnect con, DynamicDictionary data_param, int? start_node_id, string sort_by = null);

        List<TNode> GetChildNodes(DbConnect con, DynamicDictionary data_param, int? parent_id, string sort_by = null);
    }
}
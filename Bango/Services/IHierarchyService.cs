using Bango.Base.List;
using Bango.Models;
using Bango.Repo;
using Bango.Responses;

namespace Bango.Services
{
    public interface IHierarchyService<TModel, TKey, TNode>
        where TModel : class, IModel, new()
        where TNode : class, ITreeNode<TNode>, new()
    {
        bool CheckClientID { get; set; }
        bool DisplayMasterDataFromSystem { get; set; }
        IHierarchyRepo<TModel, TKey, TNode> HierarchyRepo { get; set; }

        ResponseTree<TNode> GetTreeNodes(DynamicDictionary data_param, int? start_node_id = default(int?), string sort_by = null);
        ResponseTree<TNode> GetChildNodes(DynamicDictionary data_param, int? start_node_id = default(int?), string sort_by = null);
        ISearchRepo<TModel, TKey> InitSearchRepo();
    }
}
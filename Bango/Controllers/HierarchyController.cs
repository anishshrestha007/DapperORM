using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bango.Services;
using Bango.Models;
using System.Web.Http;
using Bango.Responses;
using Bango.Base.List;

namespace Bango.Controllers
{
    public class HierarchyController<TModel, TService, TKey, TNode> : CrudController<TModel, TService, TKey>
        where TModel : class, IModel, new()
        where TService : class, ICrudService<TModel, TKey>, IHierarchyService<TModel, TKey, TNode>, new()
        where TNode : class, ITreeNode<TNode>, new()
    {
        [Route("TreeNodes/StartNode/{start_node_id}", Order = 8)]
        [Rbac.AuthorizeOnly]
        public virtual ResponseTree<TNode> GetTreeNodes(string treenodes, int? start_node_id)
        {
            DynamicDictionary paraList = GetQueryAsDictionary();

            TService service = new TService();
            return service.GetTreeNodes(paraList, start_node_id);
        }

        [Route("ChildNodes/ParentNode/{parent_id}", Order = 8)]
        [Rbac.AuthorizeOnly]
        public virtual ResponseTree<TNode> GetChildNodes(string childnodes, int? parent_id = null, int? page = 0, int? page_size = 0, string sort_by = null)
        {
            DynamicDictionary paraList = GetQueryAsDictionary();

            TService service = new TService();
            return service.GetChildNodes(paraList, parent_id);
        }
    }
}

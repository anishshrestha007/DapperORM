using Dynamitey;
using Bango.Base.List;
using Bango.Models;
using Bango.Repo;
using Bango.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Services
{
    public class HierarchyService<TModel, TKey, TNode> : CrudService<TModel, TKey>, IHierarchyService<TModel, TKey, TNode> 
        where TModel : class, IModel, new()
        where TNode : class, ITreeNode<TNode>, new()
    {
        //public bool CheckClientID { get; set; } = true;
        #region Repos

        protected IHierarchyRepo<TModel, TKey, TNode> _hierarchyRepo = null;
        public IHierarchyRepo<TModel, TKey, TNode> HierarchyRepo
        {
            get
            {
                if (_hierarchyRepo == null)
                {
                    _hierarchyRepo = InitHierarchyRepo();
                    _hierarchyRepo.CheckClientID = CheckClientID;
                    _hierarchyRepo.DisplayMasterDataFromSystem = DisplayMasterDataFromSystem;
                }
                return _hierarchyRepo;
            }
            set
            {
                _hierarchyRepo = value;
            }
        }
        public virtual IHierarchyRepo<TModel, TKey, TNode> InitHierarchyRepo()
        {
            return new HierarchyRepo<TModel, TKey, TNode>();
        }
        public override ISearchRepo<TModel, TKey> InitSearchRepo()
        {
            return new HierarchyRepo<TModel, TKey, TNode>();
        }
        #endregion repos

        public virtual ResponseTree<TNode> GetTreeNodes(DynamicDictionary data_param, int? start_node_id = null, string sort_by = null)
        {
            IEnumerable<TNode> items = null;
            ResponseTree<TNode> resp = new ResponseTree<TNode>(false, string.Empty);
            using (DbConnect con = new DbConnect())
            {
                items = HierarchyRepo.GetTreeNodes(con, data_param, start_node_id, sort_by);
            }
            if (Errors.Count > 0)
            {
                resp.message = "Tree nodes data load failed.";
                resp.PushErrors(Errors);
            }
            else
            {
                List<TNode> tree = new List<TNode>();
                if (items != null && items.Count() > 0)
                {
                    //now prepare the tree data
                    PrepareTreeNodes(items, tree, 0);
                    resp.message = "Tree nodes loaded successfully.";
                }
                else
                {
                    resp.message = "tree nodes not found as per search condition";
                }
                resp.children = tree;
                resp.success = true;
            }
            return resp;
        }

        public virtual ResponseTree<TNode> GetChildNodes(DynamicDictionary data_param, int? parent_id = null, string sort_by = null)
        {
            IEnumerable<TNode> items = null;
            ResponseTree<TNode> resp = new ResponseTree<TNode>(false, string.Empty);
            using (DbConnect con = new DbConnect())
            {
                items = HierarchyRepo.GetChildNodes(con, data_param, parent_id, sort_by);
            }
            if (Errors.Count > 0)
            {
                resp.message = "Child nodes data load failed.";
                resp.PushErrors(Errors);
            }
            else
            {
                if (items != null && items.Count() > 0)
                {
                    resp.message = "Child nodes loaded successfully.";
                }
                else
                {
                    resp.message = "Child nodes not found as per search condition";
                }
                resp.children = items;
                resp.success = true;
            }
            return resp;
        }

        protected virtual int PrepareTreeNodes(IEnumerable<TNode> source, List<TNode> destination, int currentIndx, string[] fields_to_sum = null)
        {
            return Bango.Helpers.DbServiceUtility.PrepareTreeNodes<TNode>(source, destination, currentIndx, fields_to_sum);
        }

        
    }
}

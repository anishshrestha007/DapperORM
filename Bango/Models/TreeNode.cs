using Bango.Base.List;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Models
{
    public interface ITreeNode<TNode>
    {
        string node_path { get; set; }
        int node_level { get; set; }
        bool leaf { get; set; }
        List<TNode> children
        {
            get; set;
        }

        int children_count { get; }
    }
    public class TreeNode : ITreeNode<TreeNode>
    {
        //[JsonIgnore]
        public string node_path { get; set; }
        public string parent_node_path { get; set; }

        public int node_level { get; set; }
        public int node_id { get; set; }
        public string code { get; set; }
        public string name_np { get; set; }
        public string name_en { get; set; }
        public int? parent_id { get; set; }
        public string text { get; set; }
        public double amount { get; set; }
        public virtual List<TreeNode> children
        {
            get; set;
        }
        public bool leaf { get; set; } = true;
        
        public int children_count {
            get {
                if(children == null)
                {
                    return 0;
                }
                else
                {
                    return children.Count;
                }
            }
        }
        public TreeNode()
        {
            
            //childs = new DynamicDictionary();
        }

    }
}

using Bango.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Responses
{
    public class ResponseTree<TNode> : ResponseBase, Bango.Responses.IResponseTree<TNode>
    {
        public ResponseTree()
            : this(false, string.Empty)
        {

        }
        public ResponseTree(bool success, IEnumerable<TNode> items, int totalRecords)
            : this(success, items, totalRecords, "", string.Empty)
        {
        }

        public ResponseTree(bool success, IEnumerable<TNode> items)
            : this(success, items, 0, string.Empty, string.Empty)
        {
            if (items != null && items.Count() > 0)
            {
                this.total = items.Count();
            }
        }

        public ResponseTree(bool success, IEnumerable<TNode> items, string message)
            : this(success, items, 0, message, string.Empty)
        {
            if (items != null && items.Count() > 0)
            {
                this.total = items.Count();
            }
        }
        public ResponseTree(bool success, string message)
            : this(success, null, 0, message, string.Empty)
        {

        }

        public ResponseTree(bool success, IEnumerable<TNode> items, int totalRecords, string message, string errorCode)
            : base(success, message, errorCode)
        {
            this.children = items;
            this.total = totalRecords;
        }
        //public ResponseTree(bool success, IEnumerable<object> items, int totalRecords, int nextPage, int previousPage)
        //    : this(success, "")
        //{
        //    this.data = items;
        //    this.total = totalRecords;
        //    this.nextPage = nextPage;
        //    this.previousPage = previousPage;
        //}

        public IEnumerable<TNode> children { get; set; }
        public int total { get; set; }
        //public int nextPage { get; set; }
        //public int previousPage { get; set; }
        //public int totalPages { get; set; }
        //public int page { get; set; }

        //public int totalPages { get; set; }
    }
}

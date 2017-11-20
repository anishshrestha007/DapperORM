using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Models.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class |
                       System.AttributeTargets.Struct)
    ]
    public class TableDetailAttribute : System.ComponentModel.DataAnnotations.Schema.TableAttribute
    {
        public string Sequence { get; set; }
        public string DeleteFlagField { get; set; }
        public string OrderByField { get; set; }
        public string Alias { get; set; } = "c";
        //public string StatusflagField { get; set; }
        public TableDetailAttribute(string name)
            : base (name)
        {

        }
        public TableDetailAttribute(
            string name, 
            string deleteFlagField,
            string orderByField = null, 
            string sequence = null, 
            string alias = "c" )
            //,string statusflagfield = null )
            : base (name)
        {
            DeleteFlagField = deleteFlagField == null ? string.Empty : deleteFlagField;
            OrderByField = orderByField == null ? string.Empty : orderByField;
            Sequence = sequence == null ? string.Empty : sequence;
            Alias = alias == null ? "c" : alias;
           // StatusflagField = statusflagfield == null ? "Status" : statusflagfield;
        }
    }
}

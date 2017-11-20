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
    public class DateType : System.Attribute
    {
    }


    public class ActualFieldName : Attribute
    {
        public string FieldName { get; set; }
        public ActualFieldName(string fieldName)
        {
            FieldName = fieldName;
        }

        public ActualFieldName()
            : this("name")
        {
        }
    }
}

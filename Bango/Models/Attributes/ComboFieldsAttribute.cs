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
    public class ComboFieldsAttribute : Attribute
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Others { get; set; }
        public string Text { get; set; }
        public string OrderBy { get; set; }
        public string Status { get; set; }
        public ComboFieldsAttribute(
            string id = null, 
            string code = null, 
            string name = null, 
            string text = null, 
            string others = null, 
            string orderBy = null, 
            string tableAlias = null,
            string status= null )
        {
            Id = id == null ? string.Empty : id;
            Code = code == null ? string.Empty : code;
            Name = name == null ? string.Empty : name;
            Text = text == null ? string.Empty : text;
            Others = others == null ? string.Empty : others;
            OrderBy = orderBy == null ? string.Empty : orderBy;
            Status = status == null ? "Status" : status;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Id.Length > 0)
                sb.Append($"{Id},{Id} as value,");
            if (Code.Length > 0)
                sb.Append(Code + ",");
            if (Name.Length > 0)
                sb.Append(Name + ",");
            if (Text.Length > 0)
                sb.Append(Text + ",");
            if (Others.Length > 0)
                sb.Append(Others + ",");
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }
    }

    public class GridFilterFieldsAttribute : ComboFieldsAttribute
    {
        public GridFilterFieldsAttribute(string id = null, string code = null, string name = null, string text = null, string others = null, string orderBy = null)
        {
            Id = id == null ? string.Empty : id;
            Code = code == null ? string.Empty : code;
            Name = name == null ? string.Empty : name;
            Text = text == null ? string.Empty : text;
            Others = others == null ? string.Empty : others;
            OrderBy = orderBy == null ? string.Empty : orderBy;
        }
    }

}

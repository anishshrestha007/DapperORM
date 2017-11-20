using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Models
{
    public class UniqueConstraint
    {
        public string Name { get; set; }
        public List<string> Fields { get; set; } = new List<string>();
        public string ErrorMessage { get; set; }
        public bool AutoCheck { get; set; } = true;
    }

    public class UniqueConstraintDictionary : Dictionary<string, UniqueConstraint>
        //where TModel : IModel
    {
        public void Add(string name, List<string> fields, string errorMessage, bool autoCheck = false)
        {
            this.Add(name, new UniqueConstraint()
            {
                Name = name,
                Fields = fields,
                ErrorMessage = errorMessage,
                AutoCheck = autoCheck
            });
        }

        public void Add(string name, string[] fields, string errorMessage, bool autoCheck = false)
        {
            Add(name, new List<string>(fields), errorMessage, autoCheck);
        }

        public void Add(string field, string errorMessage, bool autoCheck = false)
        {
            Add(field, new string[] { field }, errorMessage, autoCheck);
        }
        public new void Add(string key, UniqueConstraint value)
        {
            if (base.ContainsKey(key))
            {
                base[key] = value;
            }
            else
            {
                base.Add(key, value);
            }
        }

        public  BangoCommand GetCommand()
        {
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Base.Setting
{
    public class SettingBase <T> : ISettingBase<T>
    {
        public virtual T DefaultValue { get; set; }
        public bool AutoLoad { get; set; }
        protected T _value;
        
        public SettingBase()
        {
            AutoLoad = true;
            LoadValue();
        }

        public virtual bool LoadValue()
        {
            DefaultValue = default(T);
            _value = DefaultValue;
            //
            return true;
        }
        public virtual T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = Value;
            }
        }
    }
}
